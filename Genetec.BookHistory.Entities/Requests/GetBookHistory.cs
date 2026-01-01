using Genetec.BookHistory.Entities.Enums;
using Genetec.BookHistory.Entities.Filters;
using Genetec.BookHistory.Entities.Orders;
using Genetec.BookHistory.Entities.Paging;

namespace Genetec.BookHistory.Entities.Requests
{
    public class GetBookHistory
    {
        public BookHistoryFilter? Filter { get; set; }

        public IEnumerable<BookHistoryOrder>? Orders { get; set; }

        public PagingParameters? PagingParameters { get; set; }

        public IEnumerable<BookHistoryField>? Groups { get; set; }
    }
}
