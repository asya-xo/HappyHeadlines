using ArticleService.Data;
using ArticleService.DTOs;
using ArticleService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArticleService.Controllers;

[ApiController]
[Route("articles")]
public class ArticlesController : ControllerBase
{
    private readonly ArticleRepository _repo;
    private readonly ArticleCache _cache;

    public ArticlesController(ArticleRepository repo, ArticleCache cache)
    {
        _repo = repo;
        _cache = cache;
    }

    private static string R(string? region) => string.IsNullOrWhiteSpace(region) ? "global" : region;

    // GET /articles?region=eu
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Article>>> GetAll([FromQuery] string? region)
    {
        var reg = R(region);

        // Only use cache for the global region (offline-filled cache)
        if (reg == "global")
        {
            var cached = _cache.GetArticles();
            if (cached != null)
            {
                Console.WriteLine("[ArticleService] Served articles from cache");
                return Ok(cached);
            }
        }

        // Fallback: fetch from DB
        var result = await _repo.GetAllAsync(reg);

        // Store in cache if global
        if (reg == "global")
            _cache.SetArticles(result);

        return Ok(result);
    }

    // GET /articles/5?region=eu
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Article>> GetById(int id, [FromQuery] string? region)
    {
        var reg = R(region);
        var a = await _repo.GetByIdAsync(reg, id);
        return a is null ? NotFound() : Ok(a);
    }

    // POST /articles?region=eu
    [HttpPost]
    public async Task<ActionResult<Article>> Create([FromQuery] string? region, [FromBody] ArticleCreate dto)
    {
        var r = R(region);
        var created = await _repo.CreateAsync(r, dto);

        // Clear the cache after changes to ensure freshness
        if (r == "global") _cache.Clear();

        return CreatedAtAction(nameof(GetById), new { id = created.Id, region = r }, created);
    }

    // PUT /articles/5?region=eu
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromQuery] string? region, [FromBody] ArticleUpdate dto)
    {
        var r = R(region);
        var updated = await _repo.UpdateAsync(r, id, dto);

        // Invalidate cache if global
        if (r == "global") _cache.Clear();

        return updated ? NoContent() : NotFound();
    }

    // DELETE /articles/5?region=eu
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, [FromQuery] string? region)
    {
        var r = R(region);
        var deleted = await _repo.DeleteAsync(r, id);

        // Invalidate cache if global
        if (r == "global") _cache.Clear();

        return deleted ? NoContent() : NotFound();
    }
}
