using DraftService.Models;
using Microsoft.EntityFrameworkCore;

namespace DraftService.Data
{
    public class DraftRepository
    {
        private readonly DraftDbContext _context;

        public DraftRepository(DraftDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Draft>> GetAllAsync()
        {
            return await _context.Drafts.ToListAsync();
        }

        public async Task<Draft?> GetByIdAsync(int id)
        {
            return await _context.Drafts.FindAsync(id);
        }

        public async Task<Draft> AddAsync(Draft draft)
        {
            _context.Drafts.Add(draft);
            await _context.SaveChangesAsync();
            return draft;
        }

        public async Task DeleteAsync(int id)
        {
            var draft = await _context.Drafts.FindAsync(id);
            if (draft != null)
            {
                _context.Drafts.Remove(draft);
                await _context.SaveChangesAsync();
            }
        }
    }
}
