using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ArticleService.Data;

public class ArticleCacheLoader : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<ArticleCacheLoader> _logger;
    private readonly ArticleCache _cache;

    public ArticleCacheLoader(IServiceProvider provider, ArticleCache cache, ILogger<ArticleCacheLoader> logger)
    {
        _provider = provider;
        _cache = cache;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _provider.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<ArticleRepository>();
                var articles = await repo.GetAllAsync("global"); 
                var latest14Days = articles.Where(a => a.PublishedAt >= DateTime.UtcNow.AddDays(-14)).ToList();

                _cache.SetArticles(latest14Days);
                _logger.LogInformation("[ArticleCacheLoader] Refreshed cache with {count} articles", latest14Days.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ArticleCacheLoader] Failed to refresh cache");
            }

            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}
