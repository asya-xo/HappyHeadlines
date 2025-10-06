using ArticleService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ArticleService.Data;

public class ArticleDbContext : DbContext
{
    public ArticleDbContext(DbContextOptions<ArticleDbContext> options) : base(options) { }
    public DbSet<Article> Articles => Set<Article>();

    // Factory used since each region has its own connection string
    public static ArticleDbContext Create(string connString)
    {
        var opts = new DbContextOptionsBuilder<ArticleDbContext>()
            .UseNpgsql(connString)
            .Options;

        var ctx = new ArticleDbContext(opts);
        ctx.Database.EnsureCreated(); 
        return ctx;
    }
}
