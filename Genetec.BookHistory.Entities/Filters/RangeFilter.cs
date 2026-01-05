namespace Genetec.BookHistory.Entities.Filters
{
    public class RangeFilter<T> : IFilter
    {
        public bool IsNegation { get; set; }

        public required T From { get; set; }

        public required T To { get; set; }
    }
}
