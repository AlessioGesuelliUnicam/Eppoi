using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Eppoi.API.DTOs;
using Eppoi.API.Services;

namespace Eppoi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new { message = "Message is required." });

        try
        {
            var response = await _chatService.SendMessageAsync(request);
            return Ok(response);
        }
        catch (HttpRequestException)
        {
            return StatusCode(503, new { message = "Chatbot service unavailable. Make sure Ollama is running." });
        }
    }
}