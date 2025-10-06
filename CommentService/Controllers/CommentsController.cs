using Microsoft.AspNetCore.Mvc;
using CommentService.Data;
using CommentService.Models;
using Polly.CircuitBreaker;

namespace CommentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly CommentRepository _repo;
        private readonly CommentCache _cache;
        private readonly HttpClient _profanityClient;

        public CommentController(CommentRepository repo, CommentCache cache, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _cache = cache;
            _profanityClient = httpClientFactory.CreateClient("ProfanityService");
        }

      
        [HttpGet("{articleId}")]
        public IActionResult GetByArticle(int articleId)
        {
            var comments = _cache.GetCommentsForArticle(articleId);
            return Ok(comments);
        }

      
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Comment comment)
        {
            try
            {
                var requestBody = new { Text = comment.Text };
                var response = await _profanityClient.PostAsJsonAsync("check", requestBody);

                if (!response.IsSuccessStatusCode)
                    return StatusCode(503, "ProfanityService unavailable");

                var result = await response.Content.ReadFromJsonAsync<ProfanityCheckResponse>(
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (result?.ContainsProfanity == true)
                    return BadRequest("Your comment contains profanity!");

                _repo.Add(comment);

              
                _cache.GetCommentsForArticle(comment.ArticleId);

                return Ok(comment);
            }
            catch (BrokenCircuitException)
            {
                return StatusCode(503, new { error = "ProfanityService unavailable (circuit breaker open)" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }
    }

    public class ProfanityCheckResponse
    {
        public bool ContainsProfanity { get; set; }
    }
}
