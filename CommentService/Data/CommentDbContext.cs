using CommentService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CommentService.Data
{
    public class CommentDbContext : DbContext
    {
        public CommentDbContext(DbContextOptions<CommentDbContext> options)
            : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; }
    }
}
