using DraftService.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DraftService.Data
{
    public class DraftRepository
    {
        private readonly DraftDbContext _context;
        private readonly ILogger<DraftRepository> _logger;
        private static readonly ActivitySource ActivitySource = new("DraftService.Repository");

        public DraftRepository(DraftDbContext context, ILogger<DraftRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Draft>> GetAllAsync()
        {
            using var activity = ActivitySource.StartActivity("GetAllDrafts");
            _logger.LogInformation("Fetching all drafts from database...");
            var drafts = await _context.Drafts.ToListAsync();
            _logger.LogInformation("Fetched {Count} drafts", drafts.Count);
            return drafts;
        }

        public async Task<Draft?> GetByIdAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("GetDraftById");
            _logger.LogInformation("Fetching draft with ID {Id}", id);
            var draft = await _context.Drafts.FindAsync(id);
            _logger.LogInformation(draft == null ? "Draft not found" : "Draft found");
            return draft;
        }

        public async Task<Draft> AddAsync(Draft draft)
        {
            using var activity = ActivitySource.StartActivity("AddDraft");
            _logger.LogInformation("Adding new draft: {@Draft}", draft);
            _context.Drafts.Add(draft);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Draft with ID {Id} added successfully", draft.Id);
            return draft;
        }

        public async Task DeleteAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("DeleteDraft");
            _logger.LogInformation("Attempting to delete draft with ID {Id}", id);
            var draft = await _context.Drafts.FindAsync(id);
            if (draft != null)
            {
                _context.Drafts.Remove(draft);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Draft with ID {Id} deleted", id);
            }
            else
            {
                _logger.LogWarning("Draft with ID {Id} not found for deletion", id);
            }
        }
    }
}
