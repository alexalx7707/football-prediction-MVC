using football_prediction_MVC.Models.Chat;
using football_prediction_MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace football_prediction_MVC.Controllers;

[Authorize(Policy = "RequireUserRole")]
public class ChatController : Controller
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IChatService chatService, ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        if (request?.Messages is null || request.Messages.Count == 0)
        {
            return BadRequest(new { error = "Send at least one message." });
        }

        try
        {
            var reply = await _chatService.SendAsync(request.Messages, cancellationToken);
            return Json(new { reply });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Chat request failed");
            return StatusCode(500, new { error = "The assistant is unavailable right now. Try again in a moment." });
        }
    }
}
