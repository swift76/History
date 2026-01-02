using Microsoft.EntityFrameworkCore;

namespace Genetec.BookHistory.PostgreRepositories.Base
{
    public abstract class EFBaseRepository(string connectionString)
    {
        private readonly string connectionString = connectionString;

        protected AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new AppDbContext(options);
        }
    }
}
