using Microsoft.EntityFrameworkCore;
using SubscriberService.Models;

namespace SubscriberService.Data;

public class SubscriberRepository
{
    private readonly SubscriberDbContext _ctx;
    public SubscriberRepository(SubscriberDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<Subscriber>> GetAllAsync()
    {
        return await _ctx.Subscribers.AsNoTracking().ToListAsync();
    }

    public async Task<Subscriber?> GetByEmailAsync(string email)
    {
        return await _ctx.Subscribers.FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<Subscriber> AddAsync(string email)
    {
        var s = new Subscriber { Email = email, IsActive = true };
        _ctx.Subscribers.Add(s);
        await _ctx.SaveChangesAsync();
        return s;
    }

    public async Task<bool> RemoveAsync(string email)
    {
        var s = await _ctx.Subscribers.FirstOrDefaultAsync(x => x.Email == email);
        if (s is null) return false;

        _ctx.Subscribers.Remove(s);
        await _ctx.SaveChangesAsync();
        return true;
    }
}
