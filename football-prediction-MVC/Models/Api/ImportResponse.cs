using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models.Api;

public class ImportResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("matches_imported")]
    public int MatchesImported { get; set; }

    [JsonPropertyName("elo_ratings_imported")]
    public int EloRatingsImported { get; set; }
}
