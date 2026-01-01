using Genetec.BookHistory.Entities.Base;
using Genetec.BookHistory.Utilities.Extensions;

namespace Genetec.BookHistory.Entities.Requests
{
    public class UpdateBook : Book
    {
        public int Id { get; set; }
        public int LastRevisionNumber { get; set; }

        public string? GetUpdatedTitle(string currentTitle)
        {
            return GetUpdatedStringValue(Title, currentTitle);
        }

        public string? GetUpdatedShortDescription(string currentShortDescription)
        {
            return GetUpdatedStringValue(ShortDescription, currentShortDescription);
        }

        public DateOnly? GetUpdatedPublishDate(DateTime currentPublishDate)
        {
            if (!PublishDate.HasValue)
            {
                return null;
            }

            if (PublishDate.Value.ConvertToDateTime() == currentPublishDate)
            {
                return null;
            }

            return PublishDate;
        }

        public IEnumerable<string>? GetUpdatedAuthors(IEnumerable<Author>? currentAuthors)
        {
            if (Authors == null || !Authors.Any())
            {
                return null;
            }

            var authors = GetNormalizedAuthors();
            if (authors.SequenceEqual(currentAuthors.Select(item => item.Name)))
                return null;

            return authors;
        }

        private static string? GetUpdatedStringValue(string? value, string currentValue)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            value = value.Trim();
            if (value == currentValue)
            {
                return null;
            }

            return value;
        }
    }
}
