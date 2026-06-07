using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models.Api;

public class HeadToHeadQuery
{
    [JsonPropertyName("team_a")]
    public string? TeamA { get; set; }

    [JsonPropertyName("team_b")]
    public string? TeamB { get; set; }

    [JsonPropertyName("season")]
    public string? Season { get; set; }

    [JsonPropertyName("date_from")]
    public string? DateFrom { get; set; }

    [JsonPropertyName("date_to")]
    public string? DateTo { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}
