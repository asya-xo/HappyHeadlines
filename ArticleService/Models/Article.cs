namespace ArticleService.Models;

public class Article
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    public string Region { get; set; } = "global";
}
