using System.ComponentModel.DataAnnotations;

namespace Genetec.BookHistory.Entities.Base
{
    public class Author
    {
        //As there are authors with just a single name (like Petrarch or Stendhal), not split to first/last/middle names :).
        [Required(AllowEmptyStrings = false, ErrorMessage = "Author name is not provided")]
        [MaxLength(200)]
        public required string Name { get; set; }
    }
}
