namespace SubscriberService.Models;

public class Toggle
{
    public int Id { get; set; }
    public string Key { get; set; } = "subscriber_release_enabled";
    public bool Enabled { get; set; } = false;
}
