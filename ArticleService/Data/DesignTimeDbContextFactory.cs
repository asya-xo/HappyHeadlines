using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ArticleService.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ArticleDbContext>
    {
        public ArticleDbContext CreateDbContext(string[] args)
        {
            // Temporary fallback for migrations only (local use)
            var optionsBuilder = new DbContextOptionsBuilder<ArticleDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Username=hh;Password=hh;Database=articles");

            return new ArticleDbContext(optionsBuilder.Options);
        }
    }
}
