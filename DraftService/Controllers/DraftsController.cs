using DraftService.Data;
using DraftService.Models;
using Microsoft.AspNetCore.Mvc;

namespace DraftService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DraftsController : ControllerBase
    {
        private readonly DraftRepository _repo;

        public DraftsController(DraftRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Draft>>> GetAll()
        {
            var drafts = await _repo.GetAllAsync();
            return Ok(drafts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Draft>> GetById(int id)
        {
            var draft = await _repo.GetByIdAsync(id);
            if (draft == null) return NotFound();
            return Ok(draft);
        }

        [HttpPost]
        public async Task<ActionResult<Draft>> Create(Draft draft)
        {
            var created = await _repo.AddAsync(draft);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}
