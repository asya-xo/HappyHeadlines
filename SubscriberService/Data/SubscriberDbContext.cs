using Microsoft.EntityFrameworkCore;
using SubscriberService.Models;
using System.Collections.Generic;

namespace SubscriberService.Data;

public class SubscriberDbContext : DbContext
{
    public SubscriberDbContext(DbContextOptions<SubscriberDbContext> options) : base(options) { }

    public DbSet<Subscriber> Subscribers => Set<Subscriber>();
    public DbSet<Toggle> Toggles => Set<Toggle>();
}
