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

    [JsonPropertyName("reasoning")]
    public PredictionReasoning? Reasoning { get; set; }
}

public class PredictionReasoning
{
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("factors")]
    public List<PredictionFactor> Factors { get; set; } = new();
}

public class PredictionFactor
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("home_value")]
    public double HomeValue { get; set; }

    [JsonPropertyName("away_value")]
    public double AwayValue { get; set; }

    [JsonPropertyName("favors")]
    public string Favors { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}
