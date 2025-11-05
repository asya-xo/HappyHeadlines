using DraftService.Data;
using DraftService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DraftService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DraftsController : ControllerBase
    {
        private readonly DraftRepository _repo;
        private readonly ILogger<DraftsController> _logger;
        private static readonly ActivitySource ActivitySource = new("DraftService.Controller");

        public DraftsController(DraftRepository repo, ILogger<DraftsController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Draft>>> GetAll()
        {
            using var activity = ActivitySource.StartActivity("HTTP GET /api/drafts");
            _logger.LogInformation("Received request: Get all drafts");
            var drafts = await _repo.GetAllAsync();
            return Ok(drafts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Draft>> GetById(int id)
        {
            using var activity = ActivitySource.StartActivity("HTTP GET /api/drafts/{id}");
            _logger.LogInformation("Received request: Get draft with ID {Id}", id);
            var draft = await _repo.GetByIdAsync(id);
            if (draft == null)
            {
                _logger.LogWarning("Draft with ID {Id} not found", id);
                return NotFound();
            }
            return Ok(draft);
        }

        [HttpPost]
        public async Task<ActionResult<Draft>> Create(Draft draft)
        {
            using var activity = ActivitySource.StartActivity("HTTP POST /api/drafts");
            _logger.LogInformation("Received request: Create draft {@Draft}", draft);
            var created = await _repo.AddAsync(draft);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var activity = ActivitySource.StartActivity("HTTP DELETE /api/drafts/{id}");
            _logger.LogInformation("Received request: Delete draft with ID {Id}", id);
            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}
