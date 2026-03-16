using System.ComponentModel.DataAnnotations;

namespace BookHistory.Entities.Requests
{
    public class DeleteBook
    {
        [Required(ErrorMessage = "Id is not provided")]
        public int Id { get; set; }
    }
}
