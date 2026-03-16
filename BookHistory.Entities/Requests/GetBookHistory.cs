using BookHistory.Entities.Enums;
using BookHistory.Entities.Filters;
using BookHistory.Entities.Orders;
using BookHistory.Entities.Paging;

namespace BookHistory.Entities.Requests
{
    public class GetBookHistory
    {
        public BookHistoryFilter? Filter { get; set; }

        public IEnumerable<BookHistoryOrder>? Orders { get; set; }

        public PagingParameters? PagingParameters { get; set; }

        public IEnumerable<BookHistoryField>? Groups { get; set; }
    }
}
