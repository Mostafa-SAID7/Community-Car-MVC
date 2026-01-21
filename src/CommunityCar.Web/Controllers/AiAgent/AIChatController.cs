using CommunityCar.Application.Common.Interfaces.Services.AI;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CommunityCar.Web.Controllers.AiAgent;

[ApiController]
[Route("api/ai-chat")]
public class AIChatController : ControllerBase
{
    private readonly IAIChatService _aiChatService;

    public AIChatController(IAIChatService aiChatService)
    {
        _aiChatService = aiChatService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Message cannot be empty.");
        }

        try
        {
            var response = await _aiChatService.GenerateResponseAsync(request.Message);
            return Ok(new { response });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { error = "AI service error", details = ex.Message });
        }
    }
}

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
}
