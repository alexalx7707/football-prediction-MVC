using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models.Api;

public class PredictionRequest
{
    [JsonPropertyName("home_team")]
    public string HomeTeam { get; set; } = string.Empty;

    [JsonPropertyName("away_team")]
    public string AwayTeam { get; set; } = string.Empty;
}
