using System.Net.Http.Json;
using System.Text.Json;
using football_prediction_MVC.Models.Api;

namespace football_prediction_MVC.Services;

public class DataService : IDataService
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public DataService(HttpClient client)
    {
        _client = client;
    }

    public async Task<ImportResponse> ImportDatasetAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsync("data/import", null, cancellationToken);
        return await ReadOrThrowAsync<ImportResponse>(response, cancellationToken);
    }

    public async Task<DataStatsResponse> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetAsync("data/stats", cancellationToken);
        return await ReadOrThrowAsync<DataStatsResponse>(response, cancellationToken);
    }

    public async Task<List<Dictionary<string, JsonElement>>> GetMatchesAsync(MatchQueryParams query, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync("data/matches", query, _jsonOptions, cancellationToken);
        return await ReadOrThrowAsync<List<Dictionary<string, JsonElement>>>(response, cancellationToken);
    }

    public async Task<List<Dictionary<string, JsonElement>>> GetEloRatingsAsync(EloQueryParams query, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync("data/elo", query, _jsonOptions, cancellationToken);
        return await ReadOrThrowAsync<List<Dictionary<string, JsonElement>>>(response, cancellationToken);
    }

    public async Task<List<string>> GetTeamsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetAsync("data/teams", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return new List<string>();
        }

        var payload = await ReadOrThrowAsync<TeamsResponse>(response, cancellationToken);
        return payload.Teams
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(t => t, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<List<string>> GetSeasonsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetAsync("data/seasons", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return new List<string>();
        }

        var payload = await ReadOrThrowAsync<SeasonsResponse>(response, cancellationToken);
        return payload.Seasons ?? new List<string>();
    }

    public Task<JsonElement> GetTeamSummaryAsync(TeamSummaryQuery query, CancellationToken cancellationToken = default) =>
        PostForJsonAsync("data/team-summary", query, cancellationToken);

    public Task<JsonElement> GetHeadToHeadAsync(HeadToHeadQuery query, CancellationToken cancellationToken = default) =>
        PostForJsonAsync("data/head-to-head", query, cancellationToken);

    public Task<JsonElement> GetStandingsAsync(StandingsQuery query, CancellationToken cancellationToken = default) =>
        PostForJsonAsync("data/standings", query, cancellationToken);

    // Analytics endpoints return a computed JSON object on 200 and a structured
    // { "error": ... } body on 400 (e.g. standings listing available_leagues). Relay
    // both verbatim so the chatbot can read the error and retry; only hard-fail on 5xx.
    private async Task<JsonElement> PostForJsonAsync<TBody>(string url, TBody body, CancellationToken cancellationToken)
    {
        var response = await _client.PostAsJsonAsync(url, body, _jsonOptions, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if ((int)response.StatusCode >= 500)
        {
            throw new InvalidOperationException($"API request failed ({(int)response.StatusCode}): {content}");
        }

        using var document = JsonDocument.Parse(content);
        return document.RootElement.Clone();
    }

    private async Task<T> ReadOrThrowAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"API request failed ({(int)response.StatusCode}): {content}");
        }

        var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);
        return result ?? throw new InvalidOperationException("API returned an empty response.");
    }
}
