using Genetec.BookHistory.Entities.Base;

namespace Genetec.BookHistory.Entities.Requests
{
    public class UpdateBook : Book
    {
        public int Id { get; set; }
        public int LastRevisionNumber { get; set; }
    }
}
