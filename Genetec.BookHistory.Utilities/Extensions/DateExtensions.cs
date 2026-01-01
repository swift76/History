namespace Genetec.BookHistory.Utilities.Extensions
{
    public static class DateExtensions
    {
        public static DateTime ConvertToDateTime(this DateOnly value)
        {
            return value.ToDateTime(TimeOnly.MinValue);
        }

        public static string ToSqlString(this DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToSqlString(this DateOnly value)
        {
            return value.ToString("yyyy-MM-dd");
        }

        public static DateOnly? ConvertToDateOnly(this DateTime? value)
        {
            if (value == null)
            {
                return null;
            }

            return DateOnly.FromDateTime(value.Value);
        }
    }
}
