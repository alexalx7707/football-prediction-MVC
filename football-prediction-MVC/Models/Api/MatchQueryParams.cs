using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models.Api;

public class MatchQueryParams
{
    [JsonPropertyName("home_team")]
    public string? HomeTeam { get; set; }

    [JsonPropertyName("away_team")]
    public string? AwayTeam { get; set; }

    [JsonPropertyName("date_from")]
    public string? DateFrom { get; set; }

    [JsonPropertyName("date_to")]
    public string? DateTo { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}
