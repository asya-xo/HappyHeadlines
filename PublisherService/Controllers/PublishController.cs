using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

[ApiController]
[Route("api/[controller]")]
public class PublishController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;
    private static readonly ActivitySource ActivitySource = new("PublisherService");

    public PublishController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpPost]
    public async Task<IActionResult> Publish([FromBody] object article)
    {
        using var activity = ActivitySource.StartActivity("PublishArticle");
        var client = _clientFactory.CreateClient("ArticleService");

        var response = await client.PostAsJsonAsync("/api/articles", article);

        return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
    }
}
