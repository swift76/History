using Genetec.BookHistory.Entities.Enums;
using Genetec.BookHistory.Entities.Filters;
using Genetec.BookHistory.Entities.Orders;
using Genetec.BookHistory.Entities.Paging;
using Genetec.BookHistory.Entities.RepositoryContracts;
using Genetec.BookHistory.Entities.Responses;
using Genetec.BookHistory.PostgreRepositories.Base;
using Genetec.BookHistory.PostgreRepositories.Data;
using Genetec.BookHistory.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Genetec.BookHistory.PostgreRepositories
{
    public class EFBookHistoryRepository(string connectionString) : EFBaseRepository(connectionString), IBookHistoryRepository
    {
        public async Task<IEnumerable<BookHistoryResult>> Get(BookHistoryFilter? filter = null
           , IEnumerable<BookHistoryOrder>? orders = null
           , PagingParameters? pagingParameters = null
           , IEnumerable<BookHistoryField>? groups = null)
        {
            await using var context = CreateContext();

            IQueryable<BookHistoryData>? query = context.BookHistories.AsNoTracking();

            if (filter != null)
            {
                query = ApplyBookIdFilter(query, filter.BookIdFilter);

                query = ApplyOperationDateFilter(query, filter.OperationDateFilters);

                query = ApplyOperationTypeFilter(query, filter.OperationTypeFilter);

                query = ApplyTitleFilter(query, filter.TitleFilters);

                query = ApplyShortDescriptionFilter(query, filter.ShortDescriptionFilters);

                query = ApplyPublishDateFilter(query, filter.PublishDateFilters);

                query = ApplyAuthorsFilter(query, filter.AuthorsFilters);
            }

            if (query != null && groups != null && groups.Any())
            {
                var groupedQuery = query.GroupBy(item => new BookHistoryGroupKey
                {
                    Id = groups.Contains(BookHistoryField.Id) ? item.Id : null,
                    BookId = groups.Contains(BookHistoryField.BookId) ? item.BookId : null,
                    OperationDate = groups.Contains(BookHistoryField.OperationDate) ? item.OperationDate : null,
                    OperationId = groups.Contains(BookHistoryField.OperationId) ? item.OperationId : null,
                    Title = groups.Contains(BookHistoryField.Title) ? item.Title : null,
                    ShortDescription = groups.Contains(BookHistoryField.ShortDescription) ? item.ShortDescription : null,
                    PublishDate = groups.Contains(BookHistoryField.PublishDate) ? item.PublishDate : null,
                    AuthorsSerialized = groups.Contains(BookHistoryField.Authors) ? ListJoiner.Get(item.Authors!) : null
                })
                .Select(g => new BookHistoryGroupRow
                {
                    Key = g.Key,
                    Count = g.Count()
                });

                if (orders != null && orders.Any())
                {
                    IOrderedQueryable<BookHistoryGroupRow>? ordered = null;
                    foreach (var order in orders)
                    {
                        ordered = (order.Field, order.IsDescending) switch
                        {
                            (BookHistoryField.Id, false) =>
                                ordered == null ? groupedQuery.OrderBy(item => item.Key.Id) : ordered.ThenBy(item => item.Key.Id),
                            (BookHistoryField.Id, true) =>
                                ordered == null ? groupedQuery.OrderByDescending(item => item.Key.Id) : ordered.ThenByDescending(item => item.Key.Id),
                            (BookHistoryField.OperationDate, false) =>
                                ordered == null ? groupedQuery.OrderBy(item => item.Key.OperationDate) : ordered.ThenBy(item => item.Key.OperationDate),
                            (BookHistoryField.OperationDate, true) =>
                                ordered == null ? groupedQuery.OrderByDescending(item => item.Key.OperationDate) : ordered.ThenByDescending(item => item.Key.OperationDate),
                            (BookHistoryField.BookId, false) =>
                                ordered == null ? groupedQuery.OrderBy(item => item.Key.BookId) : ordered.ThenBy(item => item.Key.BookId),
                            (BookHistoryField.BookId, true) =>
                                ordered == null ? groupedQuery.OrderByDescending(item => item.Key.BookId) : ordered.ThenByDescending(item => item.Key.BookId),
                            (BookHistoryField.Authors, false) =>
                                ordered == null ? groupedQuery.OrderBy(item => item.Key.AuthorsSerialized) : ordered.ThenBy(item => item.Key.AuthorsSerialized),
                            (BookHistoryField.Authors, true) =>
                                ordered == null ? groupedQuery.OrderByDescending(item => item.Key.AuthorsSerialized) : ordered.ThenByDescending(item => item.Key.AuthorsSerialized),
                            _ => ordered
                        };
                    }

                    groupedQuery = ordered;
                }

                if (groupedQuery != null && pagingParameters != null)
                {
                    groupedQuery = groupedQuery.Skip((pagingParameters.PageNumber - 1) * pagingParameters.PageSize)
                        .Take(pagingParameters.PageSize);
                }

                if (groupedQuery != null)
                {
                    return await groupedQuery.Select(item => new BookHistoryResult
                    {
                        Id = item.Key.Id,
                        BookId = item.Key.BookId,
                        OperationId = (item.Key.OperationId == null ? null : (BookOperation)item.Key.OperationId),
                        OperationDate = item.Key.OperationDate,
                        Title = item.Key.Title,
                        ShortDescription = item.Key.ShortDescription,
                        PublishDate = item.Key.PublishDate,
                        Authors = item.Key.AuthorsSerialized.ToAuthorsNullableString(),
                        Count = item.Count
                    }).ToArrayAsync();
                }
            }
            else
            {
                if (query != null && orders != null && orders.Any())
                {
                    IOrderedQueryable<BookHistoryData>? ordered = null;
                    foreach (var order in orders)
                    {
                        ordered = (order.Field, order.IsDescending) switch
                        {
                            (BookHistoryField.Id, false) =>
                                ordered == null ? query.OrderBy(item => item.Id) : ordered.ThenBy(item => item.Id),
                            (BookHistoryField.Id, true) =>
                                ordered == null ? query.OrderByDescending(item => item.Id) : ordered.ThenByDescending(item => item.Id),
                            (BookHistoryField.OperationDate, false) =>
                                ordered == null ? query.OrderBy(item => item.OperationDate) : ordered.ThenBy(item => item.OperationDate),
                            (BookHistoryField.OperationDate, true) =>
                                ordered == null ? query.OrderByDescending(item => item.OperationDate) : ordered.ThenByDescending(item => item.OperationDate),
                            (BookHistoryField.BookId, false) =>
                                ordered == null ? query.OrderBy(item => item.BookId) : ordered.ThenBy(item => item.BookId),
                            (BookHistoryField.BookId, true) =>
                                ordered == null ? query.OrderByDescending(item => item.BookId) : ordered.ThenByDescending(item => item.BookId),
                            (BookHistoryField.Authors, false) =>
                                ordered == null ? query.OrderBy(item => item.Authors) : ordered.ThenBy(item => item.Authors),
                            (BookHistoryField.Authors, true) =>
                                ordered == null ? query.OrderByDescending(item => item.Authors) : ordered.ThenByDescending(item => item.Authors),
                            _ => ordered
                        };
                    }

                    query = ordered;
                }

                if (query != null && pagingParameters != null)
                {
                    query = query.Skip((pagingParameters.PageNumber - 1) * pagingParameters.PageSize)
                        .Take(pagingParameters.PageSize);
                }
                if (query != null)
                {
                    return await query.Select(item => new BookHistoryResult
                    {
                        Id = item.Id,
                        BookId = item.BookId,
                        OperationId = (item.OperationId == null ? null : (BookOperation)item.OperationId),
                        OperationDate = item.OperationDate,
                        Title = item.Title,
                        ShortDescription = item.ShortDescription,
                        PublishDate = item.PublishDate,
                        Authors = item.Authors.ToAuthorsNullableEnumerable()
                    }).ToArrayAsync();
                }
            }

            return [];
        }

        #region Auxiliary

        private static IQueryable<BookHistoryData>? ApplyBookIdFilter(IQueryable<BookHistoryData>? query, ListFilter<int>? bookIdFilter)
        {
            if (query != null && bookIdFilter?.Values?.Count() > 0)
            {
                var filterValueList = bookIdFilter.Values.ToList();
                if (bookIdFilter.IsNegation)
                {
                    return query.Where(item => !item.BookId.HasValue || !filterValueList.Contains(item.BookId.Value));
                }
                else
                {
                    return query.Where(item => item.BookId.HasValue && filterValueList.Contains(item.BookId.Value));
                }
            }

            return query;
        }

        private static IQueryable<BookHistoryData>? ApplyOperationDateFilter(IQueryable<BookHistoryData>? query, IEnumerable<RangeFilter<DateTime>>? operationDateFilters)
        {
            if (query != null && operationDateFilters?.Count() > 0)
            {
                foreach (var rangeFilter in operationDateFilters)
                {
                    if (rangeFilter.IsNegation)
                    {
                        query = query.Where(item => !item.OperationDate.HasValue || item.OperationDate.Value < rangeFilter.From || item.OperationDate.Value > rangeFilter.To);
                    }
                    else
                    {
                        query = query.Where(item => item.OperationDate.HasValue && item.OperationDate.Value >= rangeFilter.From && item.OperationDate.Value <= rangeFilter.To);
                    }
                }
            }

            return query;
        }

        private static IQueryable<BookHistoryData>? ApplyOperationTypeFilter(IQueryable<BookHistoryData>? query, ListFilter<BookOperation>? operationTypeFilter)
        {
            if (query != null && operationTypeFilter?.Values?.Count() > 0)
            {
                var filterValueList = operationTypeFilter.Values.Select(item => (byte)item).ToList();
                if (operationTypeFilter.IsNegation)
                {
                    return query.Where(item => !item.OperationId.HasValue || !filterValueList.Contains(item.OperationId.Value));
                }
                else
                {
                    return query.Where(item => item.OperationId.HasValue && filterValueList.Contains(item.OperationId.Value));
                }
            }

            return query;
        }

        private static IQueryable<BookHistoryData>? ApplyPublishDateFilter(IQueryable<BookHistoryData>? query, IEnumerable<RangeFilter<DateOnly>>? publishDateFilters)
        {
            if (query != null && publishDateFilters?.Count() > 0)
            {
                foreach (var rangeFilter in publishDateFilters)
                {
                    if (rangeFilter.IsNegation)
                    {
                        query = query.Where(item => !item.PublishDate.HasValue || item.PublishDate.Value < rangeFilter.From || item.PublishDate.Value > rangeFilter.To);
                    }
                    else
                    {
                        query = query.Where(item => item.PublishDate.HasValue && item.PublishDate.Value >= rangeFilter.From && item.PublishDate.Value <= rangeFilter.To);
                    }
                }
            }

            return query;
        }

        //TODO: Optimize and make it more generic, using expression trees

        private static IQueryable<BookHistoryData>? ApplyTitleFilter(IQueryable<BookHistoryData>? query, IEnumerable<StringFilter>? titleFilters)
        {
            if (query != null && titleFilters?.Count() > 0)
            {
                foreach (var stringFilter in titleFilters)
                {
                    if (stringFilter.IsCaseInsensitive)
                    {
                        query = stringFilter.FilterOperation switch
                        {
                            StringFilterOperation.Contains =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.Title == null ||
                                        !EF.Functions.ILike(item.Title, $"%{stringFilter.Value}%"))
                                    : query.Where(item =>
                                        item.Title != null &&
                                        EF.Functions.ILike(item.Title, $"%{stringFilter.Value}%")),
                            StringFilterOperation.StartsWith =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.Title == null ||
                                        !EF.Functions.ILike(item.Title, $"{stringFilter.Value}%"))
                                    : query.Where(item =>
                                        item.Title != null &&
                                        EF.Functions.ILike(item.Title, $"{stringFilter.Value}%")),
                            StringFilterOperation.EndsWith =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.Title == null ||
                                        !EF.Functions.ILike(item.Title, $"%{stringFilter.Value}"))
                                    : query.Where(item =>
                                        item.Title != null &&
                                        EF.Functions.ILike(item.Title, $"%{stringFilter.Value}")),
                            _ =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.Title == null ||
                                        item.Title.ToLower() != stringFilter.Value.ToLower())
                                    : query.Where(item =>
                                        item.Title != null &&
                                        item.Title.ToLower() == stringFilter.Value.ToLower())
                        };
                    }
                    else
                    {
                        query = stringFilter.FilterOperation switch
                        {
                            StringFilterOperation.Contains =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.Title == null ||
                                        !EF.Functions.Like(item.Title, $"%{stringFilter.Value}%"))
                                    : query.Where(item =>
                                        item.Title != null &&
                                        EF.Functions.Like(item.Title, $"%{stringFilter.Value}%")),
                            StringFilterOperation.StartsWith =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.Title == null ||
                                        !EF.Functions.Like(item.Title, $"{stringFilter.Value}%"))
                                    : query.Where(item =>
                                        item.Title != null &&
                                        EF.Functions.Like(item.Title, $"{stringFilter.Value}%")),
                            StringFilterOperation.EndsWith =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.Title == null ||
                                        !EF.Functions.Like(item.Title, $"%{stringFilter.Value}"))
                                    : query.Where(item =>
                                        item.Title != null &&
                                        EF.Functions.Like(item.Title, $"%{stringFilter.Value}")),
                            _ =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.Title == null ||
                                        item.Title != stringFilter.Value)
                                    : query.Where(item =>
                                        item.Title != null &&
                                        item.Title == stringFilter.Value)
                        };
                    }
                }
            }

            return query;
        }

        private static IQueryable<BookHistoryData>? ApplyShortDescriptionFilter(IQueryable<BookHistoryData>? query, IEnumerable<StringFilter>? shortDescriptionFilters)
        {
            if (query != null && shortDescriptionFilters?.Count() > 0)
            {
                foreach (var stringFilter in shortDescriptionFilters)
                {
                    if (stringFilter.IsCaseInsensitive)
                    {
                        query = stringFilter.FilterOperation switch
                        {
                            StringFilterOperation.Contains =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.ShortDescription == null ||
                                        !EF.Functions.ILike(item.ShortDescription, $"%{stringFilter.Value}%"))
                                    : query.Where(item =>
                                        item.ShortDescription != null &&
                                        EF.Functions.ILike(item.ShortDescription, $"%{stringFilter.Value}%")),
                            StringFilterOperation.StartsWith =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.ShortDescription == null ||
                                        !EF.Functions.ILike(item.ShortDescription, $"{stringFilter.Value}%"))
                                    : query.Where(item =>
                                        item.ShortDescription != null &&
                                        EF.Functions.ILike(item.ShortDescription, $"{stringFilter.Value}%")),
                            StringFilterOperation.EndsWith =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.ShortDescription == null ||
                                        !EF.Functions.ILike(item.ShortDescription, $"%{stringFilter.Value}"))
                                    : query.Where(item =>
                                        item.ShortDescription != null &&
                                        EF.Functions.ILike(item.ShortDescription, $"%{stringFilter.Value}")),
                            _ =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.ShortDescription == null ||
                                        item.ShortDescription.ToLower() != stringFilter.Value.ToLower())
                                    : query.Where(item =>
                                        item.ShortDescription != null &&
                                        item.ShortDescription.ToLower() == stringFilter.Value.ToLower())
                        };
                    }
                    else
                    {
                        query = stringFilter.FilterOperation switch
                        {
                            StringFilterOperation.Contains =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.ShortDescription == null ||
                                        !EF.Functions.Like(item.ShortDescription, $"%{stringFilter.Value}%"))
                                    : query.Where(item =>
                                        item.ShortDescription != null &&
                                        EF.Functions.Like(item.ShortDescription, $"%{stringFilter.Value}%")),
                            StringFilterOperation.StartsWith =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.ShortDescription == null ||
                                        !EF.Functions.Like(item.ShortDescription, $"{stringFilter.Value}%"))
                                    : query.Where(item =>
                                        item.ShortDescription != null &&
                                        EF.Functions.Like(item.ShortDescription, $"{stringFilter.Value}%")),
                            StringFilterOperation.EndsWith =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.ShortDescription == null ||
                                        !EF.Functions.Like(item.ShortDescription, $"%{stringFilter.Value}"))
                                    : query.Where(item =>
                                        item.ShortDescription != null &&
                                        EF.Functions.Like(item.ShortDescription, $"%{stringFilter.Value}")),
                            _ =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.ShortDescription == null ||
                                        item.ShortDescription != stringFilter.Value)
                                    : query.Where(item =>
                                        item.ShortDescription != null &&
                                        item.ShortDescription == stringFilter.Value)
                        };
                    }
                }
            }

            return query;
        }

        private static IQueryable<BookHistoryData>? ApplyAuthorsFilter(IQueryable<BookHistoryData>? query, IEnumerable<StringFilter>? authorsFilters)
        {
            if (query != null && authorsFilters?.Count() > 0)
            {
                foreach (var stringFilter in authorsFilters)
                {
                    if (stringFilter.IsCaseInsensitive)
                    {
                        query = stringFilter.FilterOperation switch
                        {
                            StringFilterOperation.Contains =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.Authors == null ||
                                        !item.Authors.Any(a => EF.Functions.ILike(a, $"%{stringFilter.Value}%")))
                                    : query.Where(item =>
                                        item.Authors != null &&
                                        item.Authors.Any(a => EF.Functions.ILike(a, $"%{stringFilter.Value}%"))),
                            StringFilterOperation.StartsWith =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.Authors == null ||
                                        !item.Authors.Any(a => EF.Functions.ILike(a, $"{stringFilter.Value}%")))
                                    : query.Where(item =>
                                        item.Authors != null &&
                                        item.Authors.Any(a => EF.Functions.ILike(a, $"{stringFilter.Value}%"))),
                            StringFilterOperation.EndsWith =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.Authors == null ||
                                        !item.Authors.Any(a => EF.Functions.ILike(a, $"%{stringFilter.Value}")))
                                    : query.Where(item =>
                                        item.Authors != null &&
                                        item.Authors.Any(a => EF.Functions.ILike(a, $"%{stringFilter.Value}"))),
                            _ =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.Authors == null ||
                                        !item.Authors.Any(a => a.ToLower() == stringFilter.Value.ToLower()))
                                    : query.Where(item =>
                                        item.Authors != null &&
                                        item.Authors.Any(a => a.ToLower() == stringFilter.Value.ToLower()))
                        };
                    }
                    else
                    {
                        query = stringFilter.FilterOperation switch
                        {
                            StringFilterOperation.Contains =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.Authors == null ||
                                        !item.Authors.Any(a => EF.Functions.Like(a, $"%{stringFilter.Value}%")))
                                    : query.Where(item =>
                                        item.Authors != null &&
                                        item.Authors.Any(a => EF.Functions.Like(a, $"%{stringFilter.Value}%"))),
                            StringFilterOperation.StartsWith =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.Authors == null ||
                                        !item.Authors.Any(a => EF.Functions.Like(a, $"{stringFilter.Value}%")))
                                    : query.Where(item =>
                                        item.Authors != null &&
                                        item.Authors.Any(a => EF.Functions.Like(a, $"{stringFilter.Value}%"))),
                            StringFilterOperation.EndsWith =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.Authors == null ||
                                        !item.Authors.Any(a => EF.Functions.Like(a, $"%{stringFilter.Value}")))
                                    : query.Where(item =>
                                        item.Authors != null &&
                                        item.Authors.Any(a => EF.Functions.Like(a, $"%{stringFilter.Value}"))),
                            _ =>
                                stringFilter.IsNegation
                                    ? query.Where(item =>
                                        item.Authors == null ||
                                        !item.Authors.Any(a => a == stringFilter.Value))
                                    : query.Where(item =>
                                        item.Authors != null &&
                                        item.Authors.Any(a => a == stringFilter.Value))
                        };
                    }
                }
            }

            return query;
        }

        #endregion
    }
}
