using Genetec.BookHistory.PostgreRepositories.Data;
using Microsoft.EntityFrameworkCore;

namespace Genetec.BookHistory.PostgreRepositories.Base
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<BookData> Books => Set<BookData>();
        public DbSet<BookHistoryData> BookHistories => Set<BookHistoryData>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookData>(builder =>
            {
                builder.ToTable("book");
                builder.HasKey(item => item.Id);
                builder.Property(item => item.Id).HasColumnName("id");
                builder.Property(item => item.Title).HasColumnName("title");
                builder.Property(item => item.ShortDescription).HasColumnName("short_description");
                builder.Property(item => item.PublishDate).HasColumnName("publish_date");
                builder.Property(item => item.Authors)
                       .HasColumnName("authors")
                       .HasColumnType("text[]");
                builder.Property(item => item.IsDeleted)
                       .HasColumnName("is_deleted")
                       .HasDefaultValue(false);
                builder.Property(item => item.RevisionNumber)
                       .HasColumnName("revision_number")
                       .IsConcurrencyToken();
                builder.HasQueryFilter(item => !item.IsDeleted);
            });

            modelBuilder.Entity<BookHistoryData>(builder =>
            {
                builder.ToTable("book_history");
                builder.HasKey(item => item.Id);
                builder.Property(item => item.Id).HasColumnName("id");
                builder.Property(item => item.BookId).HasColumnName("book_id");
                builder.Property(item => item.OperationId).HasColumnName("operation_id");
                builder.Property(item => item.OperationDate).HasColumnName("operation_date");
                builder.Property(item => item.Title).HasColumnName("title");
                builder.Property(item => item.ShortDescription).HasColumnName("short_description");
                builder.Property(item => item.PublishDate).HasColumnName("publish_date");
                builder.Property(item => item.Authors)
                       .HasColumnName("authors")
                       .HasColumnType("text[]");
            });
        }
    }
}
