using System.Text.Json;
using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models.Chat;

// Wire DTOs for the Anthropic Messages API (POST /v1/messages).
// Request shapes are ours to control; response blocks are parsed as raw
// JsonElements so the assistant turn can be echoed back verbatim in the loop.

public class ClaudeRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; }

    [JsonPropertyName("system")]
    public List<SystemBlock>? System { get; set; }

    [JsonPropertyName("tools")]
    public List<ClaudeTool>? Tools { get; set; }

    [JsonPropertyName("messages")]
    public List<ClaudeMessage> Messages { get; set; } = new();
}

public class ClaudeMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = "user";

    // string (plain text), List<JsonElement> (echoed assistant blocks),
    // or List<object> (tool_result blocks). Serialized by runtime type.
    [JsonPropertyName("content")]
    public object Content { get; set; } = string.Empty;
}

public class SystemBlock
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "text";

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("cache_control")]
    public CacheControl? CacheControl { get; set; }
}

public class CacheControl
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "ephemeral";
}

public class ClaudeTool
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("input_schema")]
    public JsonElement InputSchema { get; set; }

    [JsonPropertyName("cache_control")]
    public CacheControl? CacheControl { get; set; }
}

public class ToolResultBlock
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "tool_result";

    [JsonPropertyName("tool_use_id")]
    public string ToolUseId { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("is_error")]
    public bool? IsError { get; set; }
}

public class ClaudeResponse
{
    [JsonPropertyName("content")]
    public List<JsonElement> Content { get; set; } = new();

    [JsonPropertyName("stop_reason")]
    public string? StopReason { get; set; }

    [JsonPropertyName("usage")]
    public ClaudeUsage? Usage { get; set; }
}

public class ClaudeUsage
{
    [JsonPropertyName("input_tokens")]
    public int InputTokens { get; set; }

    [JsonPropertyName("output_tokens")]
    public int OutputTokens { get; set; }

    [JsonPropertyName("cache_creation_input_tokens")]
    public int CacheCreationInputTokens { get; set; }

    [JsonPropertyName("cache_read_input_tokens")]
    public int CacheReadInputTokens { get; set; }
}
