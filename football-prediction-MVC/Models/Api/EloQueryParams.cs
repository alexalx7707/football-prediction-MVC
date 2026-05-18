using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models.Api;

public class EloQueryParams
{
    [JsonPropertyName("club")]
    public string? Club { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}
