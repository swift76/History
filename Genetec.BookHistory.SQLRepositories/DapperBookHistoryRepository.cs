using Dapper;
using Genetec.BookHistory.Entities.Base;
using Genetec.BookHistory.Entities.Enums;
using Genetec.BookHistory.Utilities;
using Genetec.BookHistory.Utilities.Extensions;
using Genetec.BookHistory.Entities.Filters;
using Genetec.BookHistory.Entities.Orders;
using Genetec.BookHistory.Entities.Paging;
using Genetec.BookHistory.Entities.RepositoryContracts;
using Genetec.BookHistory.Entities.Responses;
using Genetec.BookHistory.SQLRepositories.Data;
using Genetec.BookHistory.SQLRepositories.Base;
using System.Data;
using System.Text;

namespace Genetec.BookHistory.SQLRepositories
{
    public class DapperBookHistoryRepository(string connectionString) : DapperBaseRepository(connectionString), IBookHistoryRepository
    {
        public async Task<IEnumerable<BookHistoryResult>> Get(BookHistoryFilter? filter = null
            , IEnumerable<BookHistoryOrder>? orders = null
            , PagingParameters? pagingParameters = null
            , IEnumerable<BookHistoryField>? groups = null)
        {
            DynamicParameters parameters = new();

            List<string> selectColumns;
            List<string>? groupColumns;
            if (groups == null || !groups.Any())
            {
                selectColumns = [.. Enum.GetNames<BookHistoryField>()];
                groupColumns = null;
            }
            else
            {
                selectColumns = [.. groups.Select(item => item.ToString())];
                groupColumns = [.. selectColumns];
                
                //Workaround for json-typed Authors, to avoid exceptions during grouping
                var indexAuthors = selectColumns.IndexOf("Authors");
                if (indexAuthors >= 0)
                {
                    var authorsConvertSql = GetJsonConvertSql("Authors");
                    selectColumns[indexAuthors] = $"{authorsConvertSql} as Authors";
                    groupColumns[indexAuthors] = authorsConvertSql;
                }

                selectColumns.Add("count(*) as Count");
            }

            var sqlBuilder = new StringBuilder($@"select {ListJoiner.Get(selectColumns)}
                from BookHistory with (nolock)");

            if (filter != null)
            {
                sqlBuilder.AppendLine();
                sqlBuilder.Append("where 0 = 0");

                if (filter.BookIdFilter?.Values?.Count() > 0)
                {
                    sqlBuilder.Append($" and {GetNegationSql(filter.BookIdFilter)}BookId in ({ListJoiner.Get(filter.BookIdFilter.Values)})");
                }

                GenerateRangeFilter(filter.OperationDateFilters, "OperationDate", ref sqlBuilder, ref parameters);

                GenerateOperationTypeFilter(filter.OperationTypeFilter, ref sqlBuilder);

                GenerateStringFilter(filter.TitleFilters, "Title", ref sqlBuilder, ref parameters);

                GenerateStringFilter(filter.ShortDescriptionFilters, "ShortDescription", ref sqlBuilder, ref parameters);

                GenerateRangeFilter(filter.PublishDateFilters, "PublishDate", ref sqlBuilder, ref parameters);

                GenerateAuthorsFilters(filter.AuthorsFilters, ref sqlBuilder, ref parameters);
            }

            if (groupColumns != null)
            {
                sqlBuilder.AppendLine();
                sqlBuilder.Append($"group by {ListJoiner.Get(groupColumns)}");
            }

            if (orders != null || pagingParameters != null)
            {
                if (orders == null || !orders.Any())
                {
                    if (groups != null && groups.Any())
                    {
                        orders = groups.Select(item => new BookHistoryOrder()
                        {
                            Field = item,
                            IsDescending = false
                        });
                    }
                    else
                    {
                        orders = [ new BookHistoryOrder() {
                            Field = BookHistoryField.Id
                        }];
                    }
                }

                sqlBuilder.AppendLine();
                sqlBuilder.Append("order by ");
                var isFirstOrder = true;
                foreach (var order in orders)
                {
                    if (isFirstOrder)
                    {
                        isFirstOrder = false;
                    }
                    else
                    {
                        sqlBuilder.Append(", ");
                    }

                    sqlBuilder.Append(order.Field.ToString());
                    if (order.IsDescending)
                    {
                        sqlBuilder.Append(" desc");
                    }
                }

                if (pagingParameters != null)
                {
                    sqlBuilder.AppendLine();
                    sqlBuilder.AppendLine($"OFFSET {(pagingParameters.PageNumber - 1) * pagingParameters.PageSize} ROWS");
                    sqlBuilder.AppendLine($"FETCH NEXT {pagingParameters.PageSize} ROWS ONLY");
                }
            }

            var result = await GetListAsync<BookHistoryData>(parameters, sqlBuilder.ToString(), cmdType: CommandType.Text);
            if (result == null)
            {
                return [];
            }

            return result.Select(item => new BookHistoryResult()
            {
                Id = item.Id,
                BookId = item.BookId,
                OperationDate = item.OperationDate,
                OperationId = (item.OperationId == null ? null : (BookOperation)item.OperationId),
                Title = item.Title,
                ShortDescription = item.ShortDescription,
                PublishDate = item.PublishDate.ConvertToDateOnly(),
                Authors = JsonSerializer.Deserialize<IEnumerable<Author>>(item.Authors),
                Count = item.Count
            });
        }

        #region Auxiliary

        private static void GenerateRangeFilter<T>(IEnumerable<RangeFilter<T>>? filters, string field, ref StringBuilder sqlBuilder, ref DynamicParameters parameters)
        {
            if (filters?.Count() > 0)
            {
                var parameterIndex = 0;
                foreach (var filter in filters)
                {
                    var parameterNameFrom = $"{field}From{parameterIndex}";
                    var parameterNameTo = $"{field}To{parameterIndex}";

                    sqlBuilder.Append($" and {GetNegationSql(filter)}{field} between @{parameterNameFrom} and @{parameterNameTo}");

                    parameters.Add(parameterNameFrom, GetDateTimeParameterValue(filter.From));
                    parameters.Add(parameterNameTo, GetDateTimeParameterValue(filter.To));

                    parameterIndex++;
                }
            }
        }

        private static void GenerateStringFilter(IEnumerable<StringFilter>? filters, string field, ref StringBuilder sqlBuilder, ref DynamicParameters parameters)
        {
            if (filters?.Count() > 0)
            {
                var parameterIndex = 0;
                foreach (var filter in filters)
                {
                    sqlBuilder.Append($" and {GetNegationSql(filter)}{GetCaseInsensitiveField(filter, field)}");

                    GenerateStringCondition(filter, field, parameterIndex, ref sqlBuilder, ref parameters);
                    parameterIndex++;
                }
            }
        }

        private static void GenerateStringCondition(StringFilter filter, string field, int parameterIndex, ref StringBuilder sqlBuilder, ref DynamicParameters parameters)
        {
            var parameterName = $"{field}{parameterIndex}";

            switch (filter.FilterOperation)
            {
                case StringFilterOperation.Contains:
                    sqlBuilder.Append($" like @{parameterName}");
                    break;
                case StringFilterOperation.StartsWith:
                    sqlBuilder.Append($" like @{parameterName}");
                    break;
                case StringFilterOperation.EndsWith:
                    sqlBuilder.Append($" like @{parameterName}");
                    break;
                default:
                    sqlBuilder.Append($" = @{parameterName}");
                    break;
            }

            parameters.Add(parameterName, SqlCondition.GetLikeValue(filter));
        }

        private static void GenerateOperationTypeFilter(ListFilter<BookOperation>? operationTypeFilter, ref StringBuilder sqlBuilder)
        {
            if (operationTypeFilter?.Values?.Count() > 0)
            {
                sqlBuilder.Append($" and {GetNegationSql(operationTypeFilter)}OperationId in ({ListJoiner.Get(operationTypeFilter.Values.Select(item => (byte)item))})");
            }
        }

        private static void GenerateAuthorsFilters(IEnumerable<StringFilter>? authorsFilters, ref StringBuilder sqlBuilder, ref DynamicParameters parameters)
        {
            if (authorsFilters?.Count() > 0)
            {
                var parameterIndex = 0;
                foreach (var authorsFilter in authorsFilters)
                {
                    sqlBuilder.AppendLine($" and {GetNegationSql(authorsFilter)}EXISTS ");
                    sqlBuilder.AppendLine("(SELECT 1 FROM OPENJSON(Authors) WITH (Name nvarchar(200) '$.Name')");
                    sqlBuilder.Append($"WHERE {GetCaseInsensitiveField(authorsFilter, "Name")}");

                    GenerateStringCondition(authorsFilter, "Name", parameterIndex, ref sqlBuilder, ref parameters);
                    sqlBuilder.AppendLine(")");
                    parameterIndex++;
                }
            }
        }

        private static DateTime GetDateTimeParameterValue<T>(T value)
        {
            if (value is DateTime datetimeValue)
            {
                return datetimeValue;
            }

            if (value is DateOnly dateonlyValue)
            {
                return dateonlyValue.ConvertToDateTime();
            }

            throw new ArgumentException($"Unsupported type: {typeof(T)}");
        }


        private static string GetNegationSql(IFilter filter)
        {
            if (filter.IsNegation)
            {
                return "not ";
            }

            return string.Empty;
        }

        private static string GetCaseInsensitiveField(StringFilter filter, string field)
        {
            if (filter.IsCaseInsensitive)
            {
                return $"lower({field})";
            }

            return field;
        }

        private static string GetJsonConvertSql(string field)
        {
            return $"convert(nvarchar(4000), {field})";
        }

        #endregion
    }
}
