using Genetec.BookHistory.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Genetec.BookHistory.Entities.Requests
{
    public class InsertBook : Book
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Title is not provided")]
        public override string? Title { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "ShortDescription is not provided")]
        public override string? ShortDescription { get; set; }

        [Required(ErrorMessage = "PublishDate is not provided")]
        public override DateOnly? PublishDate { get; set; }

        [Required(ErrorMessage = "Authors are not provided")]
        public override List<string>? Authors { get; set; }
    }
}
