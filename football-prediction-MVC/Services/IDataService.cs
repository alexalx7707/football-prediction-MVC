using System.Text.Json;
using football_prediction_MVC.Models.Api;

namespace football_prediction_MVC.Services;

public interface IDataService
{
    Task<ImportResponse> ImportDatasetAsync(CancellationToken cancellationToken = default);
    Task<DataStatsResponse> GetStatsAsync(CancellationToken cancellationToken = default);
    Task<List<Dictionary<string, JsonElement>>> GetMatchesAsync(MatchQueryParams query, CancellationToken cancellationToken = default);
    Task<List<Dictionary<string, JsonElement>>> GetEloRatingsAsync(EloQueryParams query, CancellationToken cancellationToken = default);
    Task<List<string>> GetTeamsAsync(CancellationToken cancellationToken = default);
    Task<List<string>> GetSeasonsAsync(CancellationToken cancellationToken = default);
    Task<JsonElement> GetTeamSummaryAsync(TeamSummaryQuery query, CancellationToken cancellationToken = default);
    Task<JsonElement> GetHeadToHeadAsync(HeadToHeadQuery query, CancellationToken cancellationToken = default);
    Task<JsonElement> GetStandingsAsync(StandingsQuery query, CancellationToken cancellationToken = default);
}
