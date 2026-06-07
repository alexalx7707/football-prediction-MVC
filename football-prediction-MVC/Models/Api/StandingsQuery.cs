using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models.Api;

public class StandingsQuery
{
    [JsonPropertyName("season")]
    public string? Season { get; set; }

    [JsonPropertyName("league")]
    public string? League { get; set; }
}
