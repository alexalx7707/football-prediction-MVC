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
