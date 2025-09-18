namespace CommentService.Models
{
    public class CommentAddRequest
    {
        public int ArticleId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
