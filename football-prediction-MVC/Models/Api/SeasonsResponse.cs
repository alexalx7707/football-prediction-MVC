using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models.Api;

public class SeasonsResponse
{
    [JsonPropertyName("seasons")]
    public List<string> Seasons { get; set; } = new();
}
