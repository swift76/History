namespace Genetec.BookHistory.PostgreRepositories.Data
{
    internal class BookHistoryData
    {
        public int? Id { get; set; }

        public int? BookId { get; set; }

        public DateTime? OperationDate { get; set; }

        public byte? OperationId { get; set; }

        public string? Title { get; set; }

        public string? ShortDescription { get; set; }

        public DateOnly? PublishDate { get; set; }

        public IEnumerable<string>? Authors { get; set; }
    }
}
