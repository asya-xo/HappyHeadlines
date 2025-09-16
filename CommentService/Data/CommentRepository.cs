using CommentService.Models;

namespace CommentService.Data
{
    public class CommentRepository
    {
        private readonly CommentDbContext _context;

        public CommentRepository(CommentDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Comment> GetAll() => _context.Comments.ToList();

        public void Add(Comment comment)
        {
            _context.Comments.Add(comment);
            _context.SaveChanges();
        }
    }
}
