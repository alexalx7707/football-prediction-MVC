using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models.Api;

public class TeamsResponse
{
    [JsonPropertyName("teams")]
    public List<string> Teams { get; set; } = new();
}
