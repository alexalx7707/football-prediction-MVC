using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models.Api;

public class TrainingStatusResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
