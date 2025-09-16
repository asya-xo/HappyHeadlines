using Microsoft.EntityFrameworkCore;
using ProfanityService.Models;
using System.Collections.Generic;

namespace ProfanityService.Data
{
    public class ProfanityDbContext : DbContext
    {
        public ProfanityDbContext(DbContextOptions<ProfanityDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProfanityWord> ProfanityWords { get; set; }
    }
}
