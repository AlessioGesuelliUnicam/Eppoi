namespace Eppoi.API.DTOs;

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public int? MunicipalityId { get; set; }
}