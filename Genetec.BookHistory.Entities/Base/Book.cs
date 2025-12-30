namespace Genetec.BookHistory.Entities.Base
{
    public abstract class Book
    {
        public virtual string? Title { get; set; }
        
        public virtual string? ShortDescription { get; set; }
        
        public virtual DateOnly? PublishDate { get; set; }

        //Just plain list of authors' names (without IDs), as CRUD operations on authors are out of scope.
        //As there are authors with just a single name (like Petrarch or Stendhal), not split to first/last/middle names :).
        public virtual List<string>? Authors { get; set; }
    }
}
