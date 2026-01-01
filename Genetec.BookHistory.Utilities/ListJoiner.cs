namespace Genetec.BookHistory.Utilities
{
    public static class ListJoiner
    {
        public static string Get<T>(IEnumerable<T> list, string separator = ", ")
        {
            return string.Join(separator, list);
        }
    }
}
