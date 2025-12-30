using Genetec.BookHistory.Entities.Base;

namespace Genetec.BookHistory.Entities.Responses
{
    public class GetBookResult
    {
        public required string Title { get; set; }

        public required string ShortDescription { get; set; }

        public required DateTime PublishDate { get; set; }

        public required IEnumerable<Author> Authors { get; set; }

        public int RevisionNumber { get; set; }

        public bool IsDeleted { get; set; }
    }
}
