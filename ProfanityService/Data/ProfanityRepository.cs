using ProfanityService.Models;

namespace ProfanityService.Data
{
    public class ProfanityRepository
    {
        private readonly ProfanityDbContext _context;

        public ProfanityRepository(ProfanityDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ProfanityWord> GetAll() => _context.ProfanityWords.ToList();

        public void AddWord(string word)
        {
            _context.ProfanityWords.Add(new ProfanityWord { Word = word });
            _context.SaveChanges();
        }

        public bool ContainsProfanity(string text)
        {
            var words = _context.ProfanityWords.Select(w => w.Word).ToList();
            return words.Any(w => text.Contains(w, StringComparison.OrdinalIgnoreCase));
        }
    }
}
