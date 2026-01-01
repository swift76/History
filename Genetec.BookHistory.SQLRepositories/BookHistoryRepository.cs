using Dapper;
using Genetec.BookHistory.Entities.Base;
using Genetec.BookHistory.Entities.Enums;
using Genetec.BookHistory.Entities.Extensions;
using Genetec.BookHistory.Entities.Filters;
using Genetec.BookHistory.Entities.Orders;
using Genetec.BookHistory.Entities.Paging;
using Genetec.BookHistory.Entities.RepositoryContracts;
using Genetec.BookHistory.SQLRepositories.Base;
using System.Data;
using System.Text;

namespace Genetec.BookHistory.SQLRepositories
{
    public class BookHistoryRepository(string connectionString) : BaseRepository(connectionString), IBookHistoryRepository
    {
        public async Task<IEnumerable<BookHistoryDto>> Get(BookHistoryFilter? filter = null, IEnumerable<BookHistoryOrder>? orders = null, PagingParameters? pagingParameters = null)
        {
            DynamicParameters parameters = new();

            var sqlBuilder = new StringBuilder(@"select Id, BookId, OperationDate, OperationId, Title, ShortDescription, PublishDate, Authors
                from BookHistory with (nolock)");

            if (orders == null || !orders.Any())
            {
                orders = [ new BookHistoryOrder() {
                    Field = BookHistoryField.Id
                }];
            }

            if (filter != null)
            {
                sqlBuilder.AppendLine();
                sqlBuilder.Append("where 0 = 0");

                if (filter.BookIdFilter?.Values?.Count() > 0)
                {
                    sqlBuilder.Append($" and {GetNegationSql(filter.BookIdFilter)}BookId in ({string.Join(",", filter.BookIdFilter.Values)})");
                }

                GenerateRangeFilter(filter.OperationDateFilters, "OperationDate", ref sqlBuilder, ref parameters);

                if (filter.OperationTypeFilter?.Values?.Count() > 0)
                {
                    sqlBuilder.Append($" and {GetNegationSql(filter.OperationTypeFilter)}OperationId in ({string.Join(",", filter.OperationTypeFilter.Values.Select(item => (byte)item))})");
                }

                GenerateStringFilter(filter.TitleFilters, "Title", ref sqlBuilder, ref parameters);

                GenerateStringFilter(filter.ShortDescriptionFilters, "ShortDescription", ref sqlBuilder, ref parameters);

                GenerateRangeFilter(filter.PublishDateFilters, "PublishDate", ref sqlBuilder, ref parameters);

                if (filter.AuthorsFilters?.Count() > 0)
                {
                    int parameterIndex = 0;
                    foreach (var authorsFilter in filter.AuthorsFilters)
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

            sqlBuilder.AppendLine();
            sqlBuilder.Append("order by ");
            if (orders != null)
            {
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
            }

            if (pagingParameters != null)
            {
                sqlBuilder.AppendLine();
                sqlBuilder.AppendLine($"OFFSET {(pagingParameters.PageNumber - 1) * pagingParameters.PageSize} ROWS");
                sqlBuilder.AppendLine($"FETCH NEXT {pagingParameters.PageSize} ROWS ONLY");
            }

            return await GetListAsync<BookHistoryDto>(parameters, sqlBuilder.ToString(), cmdType: CommandType.Text);
        }

        #region Auxiliary

        private static void GenerateRangeFilter<T>(IEnumerable<RangeFilter<T>>? filters, string field, ref StringBuilder sqlBuilder, ref DynamicParameters parameters)
        {
            if (filters?.Count() > 0)
            {
                int parameterIndex = 0;
                foreach (var filter in filters)
                {
                    var parameterNameFrom = $"{field}From{parameterIndex}";
                    var parameterNameTo = $"{field}To{parameterIndex}";

                    sqlBuilder.Append($" and {GetNegationSql(filter)}{field} between @{parameterNameFrom} and @{parameterNameTo}");
                    
                    parameters.Add(parameterNameFrom, filter.GetFromDate());
                    parameters.Add(parameterNameTo, filter.GetToDate());
                    
                    parameterIndex++;
                }
            }
        }

        private static void GenerateStringFilter(IEnumerable<StringFilter>? filters, string field, ref StringBuilder sqlBuilder, ref DynamicParameters parameters)
        {
            if (filters?.Count() > 0)
            {
                int parameterIndex = 0;
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

            if (filter.IsCaseInsensitive)
            {
                filter.Value = filter.Value.ToLower();
            }

            filter.Value = filter.Value.Replace("'", "''");

            switch (filter.FilterOperation)
            {
                case StringFilterOperation.Contains:
                    sqlBuilder.Append($" like @{parameterName}");
                    parameters.Add(parameterName, $"%{filter.Value}%");
                    break;
                case StringFilterOperation.StartsWith:
                    sqlBuilder.Append($" like @{parameterName}");
                    parameters.Add(parameterName, $"{filter.Value}%");
                    break;
                case StringFilterOperation.EndsWith:
                    sqlBuilder.Append($" like @{parameterName}");
                    parameters.Add(parameterName, $"%{filter.Value}");
                    break;
                default:
                    sqlBuilder.Append($" = @{parameterName}");
                    parameters.Add(parameterName, filter.Value);
                    break;
            }
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

        #endregion
    }
}
