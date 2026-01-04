namespace Genetec.BookHistory.PostgreRepositories.Data
{
    internal class BookHistoryGroupKey
    {
        public int? Id { get; init; }
        
        public int? BookId { get; init; }
        
        public DateTime? OperationDate { get; init; }
        
        public byte? OperationId { get; init; }
        
        public string? Title { get; init; }
        
        public string? ShortDescription { get; init; }
        
        public DateOnly? PublishDate { get; init; }
        
        public string? AuthorsSerialized { get; init; }
    }
}
