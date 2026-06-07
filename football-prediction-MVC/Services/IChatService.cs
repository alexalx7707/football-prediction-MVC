using football_prediction_MVC.Models.Chat;

namespace football_prediction_MVC.Services;

public interface IChatService
{
    // Takes the full client transcript, returns the assistant's reply text.
    Task<string> SendAsync(IReadOnlyList<ChatMessage> history, CancellationToken cancellationToken = default);
}
