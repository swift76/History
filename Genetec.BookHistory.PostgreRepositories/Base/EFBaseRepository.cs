using Microsoft.EntityFrameworkCore;

namespace Genetec.BookHistory.PostgreRepositories.Base
{
    public abstract class EFBaseRepository(string connectionString)
    {
        private readonly string connectionString = connectionString;

        protected AppDbContext CreateContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            return new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options);
        }
    }
}
