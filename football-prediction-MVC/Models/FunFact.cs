using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models;

public class FunFact
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}
