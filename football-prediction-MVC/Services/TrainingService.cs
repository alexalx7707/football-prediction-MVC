using System.Net.Http.Json;
using System.Text.Json;
using football_prediction_MVC.Models.Api;

namespace football_prediction_MVC.Services;

public class TrainingService : ITrainingService
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public TrainingService(HttpClient client)
    {
        _client = client;
    }

    public async Task<TrainingResponse> TrainAsync(TrainingRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync("train", request, _jsonOptions, cancellationToken);
        return await ReadOrThrowAsync<TrainingResponse>(response, cancellationToken);
    }

    public async Task<TrainingStatusResponse> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetAsync("train/status", cancellationToken);
        return await ReadOrThrowAsync<TrainingStatusResponse>(response, cancellationToken);
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
