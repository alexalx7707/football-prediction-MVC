using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models.Api;

public class DataStatsResponse
{
    [JsonPropertyName("collections")]
    public List<CollectionStats> Collections { get; set; } = new();
}
