using System.ComponentModel.DataAnnotations;

namespace Genetec.BookHistory.Entities.Base
{
    public abstract class Book
    {
        [MaxLength(200)]
        public virtual string? Title { get; set; }

        [MaxLength(2000)]
        public virtual string? ShortDescription { get; set; }
        
        public virtual DateOnly? PublishDate { get; set; }

        //Just plain list of authors' names (without IDs), as CRUD operations on authors are out of scope.
        public virtual IEnumerable<Author>? Authors { get; set; }
    }
}
