namespace EmailSubscription.Api.Models;

public class SubscribeRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public IEnumerable<string> Groups { get; set; }
}