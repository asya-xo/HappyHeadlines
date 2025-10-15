using CommentService.Models;
using System.Collections.Concurrent;

namespace CommentService.Data
{
    public class CommentCache
    {
        private readonly CommentRepository _repo;
        private readonly ConcurrentDictionary<int, List<Comment>> _cache = new();
        private readonly LinkedList<int> _lruList = new();
        private readonly object _lock = new();

        private const int MaxArticles = 30;

       
        private int _hits = 0;
        private int _misses = 0;

        public CommentCache(CommentRepository repo)
        {
            _repo = repo;
        }

        public List<Comment> GetCommentsForArticle(int articleId)
        {
            lock (_lock)
            {
                if (_cache.TryGetValue(articleId, out var comments))
                {
                    _hits++;
                    MoveToRecent(articleId);
                    Console.WriteLine($"[CommentCache] Cache HIT for Article {articleId}");
                    return comments;
                }

                _misses++;
                Console.WriteLine($"[CommentCache] Cache MISS for Article {articleId}");
                comments = _repo.GetAll().Where(c => c.ArticleId == articleId).ToList();
                AddToCache(articleId, comments);
                return comments;
            }
        }

        private void AddToCache(int articleId, List<Comment> comments)
        {
            if (_cache.Count >= MaxArticles)
            {
                var oldest = _lruList.Last;
                if (oldest != null)
                {
                    _cache.TryRemove(oldest.Value, out _);
                    _lruList.RemoveLast();
                    Console.WriteLine($"[CommentCache] Evicted LRU Article {oldest.Value}");
                }
            }

            _cache[articleId] = comments;
            _lruList.AddFirst(articleId);
        }

        private void MoveToRecent(int articleId)
        {
            _lruList.Remove(articleId);
            _lruList.AddFirst(articleId);
        }

        //For the dashboard
        public (int hits, int misses, double hitRatio) GetStats()
        {
            int total = _hits + _misses;
            double ratio = total > 0 ? Math.Round((double)_hits / total, 2) : 0;
            return (_hits, _misses, ratio);
        }
    }
}
