namespace Eppoi.API.DTOs;

public class ChatResponse
{
    public string Message { get; set; } = string.Empty;
    public bool IsOutOfDomain { get; set; } = false;
}