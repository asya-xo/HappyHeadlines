namespace ProfanityService.Models
{
    public class ProfanityCheckRequest
    {
        public string Text { get; set; } = string.Empty;
    }

    public class ProfanityAddRequest
    {
        public string Word { get; set; } = string.Empty;
    }
}
