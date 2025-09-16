using Microsoft.AspNetCore.Mvc;
using ProfanityService.Data;

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
        public IActionResult Add([FromBody] string word)
        {
            _repo.AddWord(word);
            return Ok();
        }

        [HttpPost("check")]
        public IActionResult Check([FromBody] string text)
        {
            bool hasProfanity = _repo.ContainsProfanity(text);
            return Ok(new { containsProfanity = hasProfanity });
        }
    }
}
