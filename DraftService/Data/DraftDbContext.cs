using DraftService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DraftService.Data
{
    public class DraftDbContext : DbContext
    {
        public DraftDbContext(DbContextOptions<DraftDbContext> options) : base(options) { }

        public DbSet<Draft> Drafts { get; set; }
    }
}
