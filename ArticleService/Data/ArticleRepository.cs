using ArticleService.DTOs;
using ArticleService.Models;

namespace ArticleService.Data;

public class ArticleRepository
{
    private readonly RegionConnectionResolver _resolver;
    public ArticleRepository(RegionConnectionResolver resolver) => _resolver = resolver;

    private ArticleDbContext Ctx(string region) =>
        ArticleDbContext.Create(_resolver.GetConnectionString(region));

    public Task<List<Article>> GetAllAsync(string region)
    {
        using var db = Ctx(region);
        var list = db.Articles.OrderByDescending(a => a.PublishedAt).ToList();
        return Task.FromResult(list);
    }

    public Task<Article?> GetByIdAsync(string region, int id)
    {
        using var db = Ctx(region);
        return Task.FromResult(db.Articles.Find(id));
    }

    public Task<Article> CreateAsync(string region, ArticleCreate dto)
    {
        using var db = Ctx(region);
        var a = new Article { Title = dto.Title, Content = dto.Content, Region = region };
        db.Add(a);
        db.SaveChanges();
        return Task.FromResult(a);
    }

    public Task<bool> UpdateAsync(string region, int id, ArticleUpdate dto)
    {
        using var db = Ctx(region);
        var a = db.Articles.Find(id);
        if (a is null) return Task.FromResult(false);
        a.Title = dto.Title; a.Content = dto.Content;
        db.SaveChanges();
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(string region, int id)
    {
        using var db = Ctx(region);
        var a = db.Articles.Find(id);
        if (a is null) return Task.FromResult(false);
        db.Remove(a); db.SaveChanges();
        return Task.FromResult(true);
    }
}
