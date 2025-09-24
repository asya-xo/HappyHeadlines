using ArticleService.DTOs;
using ArticleService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Data;

public class ArticleRepository
{
    private readonly RegionConnectionResolver _resolver;

    public ArticleRepository(RegionConnectionResolver resolver)
    {
        _resolver = resolver;
    }

    private ArticleDbContext CreateContext(string region)
    {
        var conn = _resolver.GetConnectionString(region);
        return ArticleDbContext.Create(conn);
    }

    public async Task<IEnumerable<Article>> GetAllAsync(string region)
    {
        using var ctx = CreateContext(region);
        return await ctx.Articles.AsNoTracking().ToListAsync();
    }

    public async Task<Article?> GetByIdAsync(string region, int id)
    {
        using var ctx = CreateContext(region);
        return await ctx.Articles.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Article> CreateAsync(string region, ArticleCreate dto)
    {
        using var ctx = CreateContext(region);
        var article = new Article { Title = dto.Title, Content = dto.Content };
        ctx.Articles.Add(article);
        await ctx.SaveChangesAsync();
        return article;
    }

    public async Task<bool> UpdateAsync(string region, int id, ArticleUpdate dto)
    {
        using var ctx = CreateContext(region);
        var article = await ctx.Articles.FirstOrDefaultAsync(a => a.Id == id);
        if (article == null) return false;

        article.Title = dto.Title;
        article.Content = dto.Content;
        await ctx.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string region, int id)
    {
        using var ctx = CreateContext(region);
        var article = await ctx.Articles.FirstOrDefaultAsync(a => a.Id == id);
        if (article == null) return false;

        ctx.Articles.Remove(article);
        await ctx.SaveChangesAsync();
        return true;
    }
}
