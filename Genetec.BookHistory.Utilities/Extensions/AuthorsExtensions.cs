using Genetec.BookHistory.Entities.Base;

namespace Genetec.BookHistory.Utilities.Extensions
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

        public static IEnumerable<string>? GetNormalizedAuthors(this IEnumerable<Author>? value)
        {
            return value?.Select(item => item.Name.Trim());
        }

        public static IEnumerable<string>? GetUpdatedValue(this IEnumerable<Author>? value, IEnumerable<Author>? previousValue)
        {
            if (value == null || !value.Any())
            {
                return null;
            }

            var authorsSerialized = value.GetNormalizedAuthors();
            if (authorsSerialized.SequenceEqual(previousValue.Select(item => item.Name)))
                return null;

            return authorsSerialized;
        }
    }
}
