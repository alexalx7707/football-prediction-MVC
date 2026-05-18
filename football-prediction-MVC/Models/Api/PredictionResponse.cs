using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models.Api;

public class PredictionResponse
{
    [JsonPropertyName("match")]
    public string Match { get; set; } = string.Empty;

    [JsonPropertyName("prediction")]
    public string Prediction { get; set; } = string.Empty;

    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }

    [JsonPropertyName("home_team")]
    public string HomeTeam { get; set; } = string.Empty;

    [JsonPropertyName("away_team")]
    public string AwayTeam { get; set; } = string.Empty;

    [JsonPropertyName("home_elo")]
    public double HomeElo { get; set; }

    [JsonPropertyName("away_elo")]
    public double AwayElo { get; set; }

    [JsonPropertyName("elo_difference")]
    public double EloDifference { get; set; }

    [JsonPropertyName("probabilities")]
    public Dictionary<string, double> Probabilities { get; set; } = new();

    [JsonPropertyName("home_win_prob")]
    public double HomeWinProb { get; set; }

    [JsonPropertyName("draw_prob")]
    public double DrawProb { get; set; }

    [JsonPropertyName("away_win_prob")]
    public double AwayWinProb { get; set; }
}
