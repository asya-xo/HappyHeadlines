using Microsoft.AspNetCore.Mvc;
using ArticleService.Data;

namespace ArticleService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheDashboardController : ControllerBase
    {
        private readonly ArticleCache _articleCache;

        public CacheDashboardController(ArticleCache articleCache)
        {
            _articleCache = articleCache;
        }

        [HttpGet]
        public IActionResult GetDashboard()
        {
            var stats = _articleCache.GetStats();

            return Ok(new
            {
                dashboard = "Article Cache Dashboard",
                hits = stats.hits,
                misses = stats.misses,
                hitRatio = stats.hitRatio
            });
        }
    }
}
