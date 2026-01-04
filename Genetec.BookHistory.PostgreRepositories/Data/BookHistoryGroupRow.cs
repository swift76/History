namespace Genetec.BookHistory.PostgreRepositories.Data
{
    internal class BookHistoryGroupRow
    {
        public BookHistoryGroupKey Key { get; init; } = default!;
       
        public int Count { get; init; }
    }
}
