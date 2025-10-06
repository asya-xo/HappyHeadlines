using Microsoft.AspNetCore.Mvc;
using CommentService.Data;

namespace CommentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheStatsController : ControllerBase
    {
        private readonly CommentCache _cache;

        public CacheStatsController(CommentCache cache)
        {
            _cache = cache;
        }

        [HttpGet]
        public IActionResult GetCacheStats()
        {
            var stats = _cache.GetStats();
            return Ok(new
            {
                cacheName = "CommentCache",
                hits = stats.hits,
                misses = stats.misses,
                hitRatio = stats.hitRatio
            });
        }
    }
}
