using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using football_prediction_MVC.Models.Chat;

namespace football_prediction_MVC.Services;

// Hand-rolled client for the Anthropic Messages API with a manual tool-use loop.
// Grounds answers in the app's own services via ChatToolExecutor.
public class ClaudeChatService : IChatService
{
    private const int MaxToolIterations = 6;

    private const string SystemPrompt =
        "You are the assistant for a football match-prediction web app. " +
        "Answer questions about match predictions, team records, head-to-head history, league standings, Elo ratings, " +
        "and the app's dataset. " +
        "You MUST use the provided tools for any prediction, rating, result, record, standing, or count — never invent or " +
        "recall Elo numbers, probabilities, match results, win/loss tallies, or table positions from memory. " +
        "Tool selection: for 'how many wins/goals/points' or a team's form, use get_team_summary; for two teams' history use " +
        "get_head_to_head; for league tables/positions use get_standings; use search_matches only to list specific fixtures or " +
        "find the latest/earliest match (it returns at most 20 rows, so never count or total its rows yourself). " +
        "Team names must match the dataset exactly and only casing is normalised — aliases are NOT resolved (e.g. 'Manchester " +
        "City' will not match the stored 'Man City'). When a user uses a nickname or full name, call list_teams and pick the " +
        "exact stored name before other tools. Seasons are formatted 'YYYY/YY' (e.g. '2023/24'); call list_seasons if unsure of " +
        "the valid range, and remember early seasons may be only partially covered while recent seasons are complete. " +
        "The dataset covers historical seasons up to the most recent completed one and is not updated live — when a prediction " +
        "or rating could be read as a real-time claim, briefly note it is based on the dataset's latest available data. " +
        "Stay on the topic of football and this app; politely decline unrelated requests. Keep answers concise.";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _client;
    private readonly ChatToolExecutor _executor;
    private readonly ILogger<ClaudeChatService> _logger;
    private readonly string _model;
    private readonly int _maxTokens;

    public ClaudeChatService(
        HttpClient client,
        ChatToolExecutor executor,
        IConfiguration configuration,
        ILogger<ClaudeChatService> logger)
    {
        _client = client;
        _executor = executor;
        _logger = logger;
        _model = configuration["Anthropic:Model"] ?? "claude-sonnet-4-6";
        _maxTokens = int.TryParse(configuration["Anthropic:MaxTokens"], out var max) ? max : 2048;
    }

    public async Task<string> SendAsync(IReadOnlyList<ChatMessage> history, CancellationToken cancellationToken = default)
    {
        var messages = history
            .Where(m => !string.IsNullOrWhiteSpace(m.Text))
            .Select(m => new ClaudeMessage
            {
                Role = m.Role == "assistant" ? "assistant" : "user",
                Content = m.Text
            })
            .ToList();

        if (messages.Count == 0)
        {
            return "Ask me something about predictions, Elo ratings, or the dataset.";
        }

        for (var iteration = 0; iteration < MaxToolIterations; iteration++)
        {
            var response = await CallApiAsync(messages, cancellationToken);

            if (response.StopReason == "tool_use")
            {
                // Echo the assistant's full content (text + tool_use) back verbatim,
                // then answer every tool_use block with a matching tool_result.
                messages.Add(new ClaudeMessage { Role = "assistant", Content = response.Content });

                var toolResults = new List<object>();
                foreach (var block in response.Content)
                {
                    if (!IsType(block, "tool_use"))
                    {
                        continue;
                    }

                    var id = block.GetProperty("id").GetString() ?? string.Empty;
                    var name = block.GetProperty("name").GetString() ?? string.Empty;
                    var input = block.TryGetProperty("input", out var inputEl) ? inputEl : default;

                    var result = await _executor.ExecuteAsync(name, input, cancellationToken);
                    toolResults.Add(new ToolResultBlock { ToolUseId = id, Content = result });
                }

                messages.Add(new ClaudeMessage { Role = "user", Content = toolResults });
                continue;
            }

            return ExtractText(response.Content);
        }

        return "I made too many tool calls without settling on an answer. Try asking something more specific.";
    }

    private async Task<ClaudeResponse> CallApiAsync(List<ClaudeMessage> messages, CancellationToken cancellationToken)
    {
        var request = new ClaudeRequest
        {
            Model = _model,
            MaxTokens = _maxTokens,
            // One cache breakpoint on the system block caches tools + system together (tools render first).
            System = new List<SystemBlock>
            {
                new() { Text = SystemPrompt, CacheControl = new CacheControl() }
            },
            Tools = _executor.Tools.ToList(),
            Messages = messages
        };

        using var httpResponse = await _client.PostAsJsonAsync("v1/messages", request, JsonOptions, cancellationToken);
        var body = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Anthropic API error {Status}: {Body}", (int)httpResponse.StatusCode, body);
            throw new InvalidOperationException($"Anthropic API returned {(int)httpResponse.StatusCode}.");
        }

        var response = JsonSerializer.Deserialize<ClaudeResponse>(body, JsonOptions)
            ?? throw new InvalidOperationException("Empty response from the Anthropic API.");

        if (response.Usage is { } usage)
        {
            _logger.LogDebug(
                "Claude usage: in={In} out={Out} cacheRead={CacheRead} cacheWrite={CacheWrite}",
                usage.InputTokens, usage.OutputTokens, usage.CacheReadInputTokens, usage.CacheCreationInputTokens);
        }

        return response;
    }

    private static bool IsType(JsonElement block, string type) =>
        block.ValueKind == JsonValueKind.Object
        && block.TryGetProperty("type", out var t)
        && t.GetString() == type;

    private static string ExtractText(IEnumerable<JsonElement> content)
    {
        var sb = new StringBuilder();
        foreach (var block in content)
        {
            if (IsType(block, "text") && block.TryGetProperty("text", out var text))
            {
                sb.Append(text.GetString());
            }
        }

        var result = sb.ToString().Trim();
        return string.IsNullOrEmpty(result)
            ? "I couldn't produce a response. Try rephrasing your question."
            : result;
    }
}
