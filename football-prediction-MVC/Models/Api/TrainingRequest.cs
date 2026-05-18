using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models.Api;

public class TrainingRequest
{
    [JsonPropertyName("test_season")]
    public string TestSeason { get; set; } = "2024/25";

    [JsonPropertyName("n_estimators")]
    public int NEstimators { get; set; } = 300;
}
