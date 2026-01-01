using Genetec.BookHistory.Entities.Enums;

namespace Genetec.BookHistory.Entities.Filters
{
    public class BookHistoryFilter
    {
        public ListFilter<int>? BookIdFilter { get; set; }

        public IEnumerable<RangeFilter<DateTime>>? OperationDateFilters { get; set; }
        
        public ListFilter<BookOperation>? OperationTypeFilter { get; set; }

        public IEnumerable<StringFilter>? TitleFilters { get; set; }

        public IEnumerable<StringFilter>? ShortDescriptionFilters { get; set; }

        public IEnumerable<RangeFilter<DateOnly>>? PublishDateFilters { get; set; }

        public IEnumerable<StringFilter>? AuthorsFilters { get; set; }
    }
}
