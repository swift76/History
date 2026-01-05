using Genetec.BookHistory.Entities.Enums;
using Genetec.BookHistory.Entities.Orders;

namespace Genetec.BookHistory.Utilities.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool HasDuplicates(this IEnumerable<BookHistoryField>? value)
        {
            if (value == null)
            {
                return false;
            }

            return value.Count() > value.Distinct().Count();
        }

        public static bool HasDuplicates(this IEnumerable<BookHistoryOrder>? value)
        {
            if (value == null)
            {
                return false;
            }

            return HasDuplicates(value.Select(item => item.Field));
        }
    }
}
