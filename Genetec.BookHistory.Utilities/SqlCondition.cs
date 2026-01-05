using Genetec.BookHistory.Entities.Enums;
using Genetec.BookHistory.Entities.Filters;

namespace Genetec.BookHistory.Utilities
{
    public static class SqlCondition
    {
        public static string GetLikeValue(StringFilter stringFilter)
        {
            var value = stringFilter.Value;
            if (stringFilter.IsCaseInsensitive)
            {
                value = value.ToLower();
            }

            return stringFilter.FilterOperation switch
            {
                StringFilterOperation.Contains =>
                    $"%{value}%",
                StringFilterOperation.StartsWith =>
                    $"{value}%",
                StringFilterOperation.EndsWith =>
                    $"%{value}",
                _ =>
                    value
            };
        }
    }
}
