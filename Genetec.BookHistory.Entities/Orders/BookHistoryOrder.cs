using Genetec.BookHistory.Entities.Enums;

namespace Genetec.BookHistory.Entities.Orders
{
    public class BookHistoryOrder
    {
        public BookHistoryField Field { get; set; }

        public bool IsDescending { get; set; }
    }
}
