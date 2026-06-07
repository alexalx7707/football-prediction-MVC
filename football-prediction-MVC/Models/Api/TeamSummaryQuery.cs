using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models.Api;

public class TeamSummaryQuery
{
    [JsonPropertyName("team")]
    public string? Team { get; set; }

    [JsonPropertyName("season")]
    public string? Season { get; set; }

    [JsonPropertyName("date_from")]
    public string? DateFrom { get; set; }

    [JsonPropertyName("date_to")]
    public string? DateTo { get; set; }
}
