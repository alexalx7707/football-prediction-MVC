using System.Text.Json.Serialization;

namespace football_prediction_MVC.Models.Chat;

// Simple client <-> server contract. The browser keeps the transcript and
// resends it each turn; the server is stateless.

public class ChatMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = "user";

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

public class ChatRequest
{
    [JsonPropertyName("messages")]
    public List<ChatMessage> Messages { get; set; } = new();
}
