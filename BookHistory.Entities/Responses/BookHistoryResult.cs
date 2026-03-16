using BookHistory.Entities.Base;
using BookHistory.Entities.Enums;

namespace BookHistory.Entities.Responses
{
    public class BookHistoryResult : Book
    {
        public int? Id { get; set; }

        public int? BookId { get; set; }

        public DateTime? OperationDate { get; set; }

        public BookOperation? OperationId { get; set; }

        public int? Count { get; set; }
    }
}
