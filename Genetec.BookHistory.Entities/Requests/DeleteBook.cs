using System.ComponentModel.DataAnnotations;

namespace Genetec.BookHistory.Entities.Requests
{
    public class DeleteBook
    {
        [Required(ErrorMessage = "Id is not provided")]
        public int Id { get; set; }
    }
}
