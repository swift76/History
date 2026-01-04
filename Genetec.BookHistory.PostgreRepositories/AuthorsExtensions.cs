using Genetec.BookHistory.Entities.Base;

namespace Genetec.BookHistory.PostgreRepositories
{
    public static class AuthorsExtensions
    {
        public static IEnumerable<Author> ToAuthorsEnumerable(this IEnumerable<string> value)
        {
            return value.Select(item => new Author()
            {
                Name = item
            });
        }

        public static IEnumerable<Author>? ToAuthorsNullableEnumerable(this IEnumerable<string>? value)
        {
            if (value == null)
            {
                return null;
            }

            return ToAuthorsEnumerable(value);
        }

        public static IEnumerable<Author>? ToAuthorsNullableString(this string? value, string separator = ", ")
        {
            if (value == null)
            {
                return null;
            }

            return ToAuthorsEnumerable(value.Split(separator));
        }
    }
}
