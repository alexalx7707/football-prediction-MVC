using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models.Api;

public class CollectionStats
{
    [JsonPropertyName("collection")]
    public string Collection { get; set; } = string.Empty;

    [JsonPropertyName("document_count")]
    public int DocumentCount { get; set; }
}
