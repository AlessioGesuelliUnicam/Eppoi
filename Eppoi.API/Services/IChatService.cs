using Eppoi.API.DTOs;

namespace Eppoi.API.Services;

public interface IChatService
{
    Task<ChatResponse> SendMessageAsync(ChatRequest request);
}