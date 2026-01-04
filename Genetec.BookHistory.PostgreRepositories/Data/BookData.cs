namespace Genetec.BookHistory.PostgreRepositories.Data
{
    internal class BookData
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public required string ShortDescription { get; set; }

        public required DateOnly PublishDate { get; set; }

        public required IEnumerable<string> Authors { get; set; }

        public int RevisionNumber { get; set; }

        public bool IsDeleted { get; set; }
    }
}
