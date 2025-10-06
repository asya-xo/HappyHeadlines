using ArticleService.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ArticleService.Data;

public class ArticleCache
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<ArticleCache> _logger;

    private const string CacheKey = "LatestArticles";
    private int _hits = 0;
    private int _misses = 0;

    public ArticleCache(IMemoryCache cache, ILogger<ArticleCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public void SetArticles(IEnumerable<Article> articles)
    {
        _cache.Set(CacheKey, articles, TimeSpan.FromHours(6));
        _logger.LogInformation("[ArticleCache] Cached {count} articles", articles.Count());
    }

    public IEnumerable<Article>? GetArticles()
    {
        if (_cache.TryGetValue(CacheKey, out IEnumerable<Article>? articles))
        {
            _hits++;
            _logger.LogInformation("[ArticleCache] Cache HIT ({hits} total hits)", _hits);
            return articles;
        }

        _misses++;
        _logger.LogInformation("[ArticleCache] Cache MISS ({misses} total misses)", _misses);
        return null;
    }

    public void Clear()
    {
        _cache.Remove(CacheKey);
        _logger.LogInformation("[ArticleCache] Cache cleared manually");
    }

    // dASHBOARDD 
    public (int hits, int misses, double hitRatio) GetStats()
    {
        int total = _hits + _misses;
        double ratio = total > 0 ? Math.Round((double)_hits / total, 2) : 0;
        return (_hits, _misses, ratio);
    }
}
