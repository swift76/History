using System.ComponentModel.DataAnnotations;

namespace Genetec.BookHistory.Entities.Paging
{
    public class PagingParameters
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page number should be a positive number")]
        public int PageNumber { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Page size should be a positive number")]
        public int PageSize { get; set; }
    }
}
