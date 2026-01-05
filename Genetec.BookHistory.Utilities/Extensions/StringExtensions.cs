namespace Genetec.BookHistory.Utilities.Extensions
{
    public static class StringExtensions
    {
        public static string? GetUpdatedValue(this string? value, string previousValue)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            value = value.Trim();
            if (value == previousValue)
            {
                return null;
            }

            return value;
        }
    }
}
