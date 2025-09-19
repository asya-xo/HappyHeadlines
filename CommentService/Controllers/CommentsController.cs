using Microsoft.AspNetCore.Mvc;
using CommentService.Data;
using CommentService.Models;
using System.Net.Http;
using Polly.CircuitBreaker; // 👈 add this

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
            try
            {
                var requestBody = new { Text = comment.Text };

                var response = await _profanityClient.PostAsJsonAsync("check", requestBody);

                Console.WriteLine($"[CommentService] ProfanityService responded: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(503, "ProfanityService unavailable");
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[CommentService] Raw response from ProfanityService: {json}");

                var result = await response.Content.ReadFromJsonAsync<ProfanityCheckResponse>(
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (result?.ContainsProfanity == true)
                {
                    return BadRequest("Your comment contains profanity!!");
                }

                _repo.Add(comment);
                return Ok(comment);
            }
            catch (BrokenCircuitException)
            {
                Console.WriteLine("[CommentService] Circuit breaker is OPEN – ProfanityService unavailable.");
                return StatusCode(503, new { error = "ProfanityService unavailable (circuit breaker open)" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CommentService] ERROR in Add(): {ex}");
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }
    }

    public class ProfanityCheckResponse
    {
        public bool ContainsProfanity { get; set; }
    }
}
