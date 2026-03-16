using BookHistory.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BookHistory.Entities.Requests
{
    public class UpdateBook : Book
    {
        [Required(ErrorMessage = "Id is not provided")]
        public int Id { get; set; }

        [Required(ErrorMessage = "LastRevisionNumber is not provided")]
        public int LastRevisionNumber { get; set; }
    }
}
