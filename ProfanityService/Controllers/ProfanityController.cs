using Microsoft.AspNetCore.Mvc;
using ProfanityService.Data;
using ProfanityService.Models;

namespace ProfanityService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfanityController : ControllerBase
    {
        private readonly ProfanityRepository _repo;

        public ProfanityController(ProfanityRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_repo.GetAll());

        [HttpPost]
        public IActionResult Add([FromBody] ProfanityAddRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Word))
                return BadRequest(new { error = "Word is required" });

            _repo.AddWord(request.Word);
            return Ok(new { added = request.Word });
        }

        [HttpPost("check")]
        public IActionResult Check([FromBody] ProfanityCheckRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest(new { error = "Text is required" });

            bool hasProfanity = _repo.ContainsProfanity(request.Text);
            return Ok(new { containsProfanity = hasProfanity });
        }
    }
}
