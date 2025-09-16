using Microsoft.AspNetCore.Mvc;
using CommentService.Data;
using CommentService.Models;
using System.Net.Http;
using System.Text.Json;

namespace CommentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly CommentRepository _repo;
        private readonly HttpClient _profanityClient;

        public CommentController(CommentRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _profanityClient = httpClientFactory.CreateClient("ProfanityService");
        }

        // Get all comments
        [HttpGet]
        public IActionResult GetAll()
        {
            var comments = _repo.GetAll();
            return Ok(comments);
        }

        // Add new comment (with profanity check)
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Comment comment)
        {
            // Ask ProfanityService if this comment contains bad words
            var response = await _profanityClient.PostAsJsonAsync("check", comment.Text);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(503, "ProfanityService unavailable");
            }

            var result = await response.Content.ReadFromJsonAsync<ProfanityCheckResponse>();
            if (result?.ContainsProfanity == true)
            {
                return BadRequest("Your comment contains profanity 🚫");
            }

            _repo.Add(comment);
            return Ok(comment);
        }
    }

    // Match ProfanityService JSON { "containsProfanity": true/false }
    public class ProfanityCheckResponse
    {
        public bool ContainsProfanity { get; set; }
    }
}
