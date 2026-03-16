using BookHistory.Entities.Enums;

namespace BookHistory.Entities.Orders
{
    public class BookHistoryOrder
    {
        public BookHistoryField Field { get; set; }

        public bool IsDescending { get; set; }
    }
}
