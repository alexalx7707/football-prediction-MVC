using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models.Api;

public class TrainingResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("accuracy_rf")]
    public double AccuracyRf { get; set; }

    [JsonPropertyName("accuracy_lr")]
    public double AccuracyLr { get; set; }

    [JsonPropertyName("accuracy_xgb")]
    public double AccuracyXgb { get; set; }

    [JsonPropertyName("test_matches")]
    public int TestMatches { get; set; }

    [JsonPropertyName("models_saved")]
    public bool ModelsSaved { get; set; }
}
