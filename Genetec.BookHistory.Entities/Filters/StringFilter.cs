using Genetec.BookHistory.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Genetec.BookHistory.Entities.Filters
{
    public class StringFilter : IFilter
    {
        public bool IsNegation { get; set; }

        public required StringFilterOperation FilterOperation { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The filter value is not provided")]
        public required string Value { get; set; }

        public bool IsCaseInsensitive { get; set; }
    }
}
