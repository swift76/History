namespace Genetec.BookHistory.Entities.Base
{
    public class BookHistoryDto
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public DateTime OperationDate { get; set; }

        public byte OperationId { get; set; }

        public string? Title { get; set; }

        public string? ShortDescription { get; set; }

        public DateTime? PublishDate { get; set; }

        public string? Authors { get; set; }
    }
}
