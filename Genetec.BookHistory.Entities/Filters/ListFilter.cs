namespace Genetec.BookHistory.Entities.Filters
{
    public class ListFilter<T> : IFilter
    {
        public bool IsNegation { get; set; }

        public required IEnumerable<T> Values { get; set; }
    }
}
