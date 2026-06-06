using Microsoft.AspNetCore.Mvc;
using AtlasChat.Models;
using AtlasChat.Services;

namespace AtlasChat.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IAIService aiService, ILogger<ChatController> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> Post([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { error = "Mesaj boş olamaz." });
        }

        _logger.LogInformation("Gelen mesaj: {Message}", request.Message);

        var aiResponse = await _aiService.GetResponseAsync(request.Message);

        return Ok(new ChatResponse { Response = aiResponse });
    }
}