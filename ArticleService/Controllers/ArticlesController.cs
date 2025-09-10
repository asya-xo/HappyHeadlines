using ArticleService.Data;
using ArticleService.DTOs;
using ArticleService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArticleService.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly ArticleRepository _repo;
    public ArticlesController(ArticleRepository repo) => _repo = repo;

    private static string R(string? region) => string.IsNullOrWhiteSpace(region) ? "global" : region;

    // GET /articles?region=eu
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Article>>> GetAll([FromQuery] string? region)
        => Ok(await _repo.GetAllAsync(R(region)));

    // GET /articles/5?region=eu
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Article>> GetById(int id, [FromQuery] string? region)
    {
        var a = await _repo.GetByIdAsync(R(region), id);
        return a is null ? NotFound() : Ok(a);
    }

    // POST /articles?region=eu
    [HttpPost]
    public async Task<ActionResult<Article>> Create([FromQuery] string? region, [FromBody] ArticleCreate dto)
    {
        var r = R(region);
        var created = await _repo.CreateAsync(r, dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id, region = r }, created);
    }

    // PUT /articles/5?region=eu
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromQuery] string? region, [FromBody] ArticleUpdate dto)
        => await _repo.UpdateAsync(R(region), id, dto) ? NoContent() : NotFound();

    // DELETE /articles/5?region=eu
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, [FromQuery] string? region)
        => await _repo.DeleteAsync(R(region), id) ? NoContent() : NotFound();
}
