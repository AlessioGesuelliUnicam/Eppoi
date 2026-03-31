namespace Eppoi.API.DTOs;

public class QuestionnaireRequest
{
    public List<string> Interests { get; set; } = new();
    public string TravelStyle { get; set; } = string.Empty;
    public string TimeAvailability { get; set; } = string.Empty;
}