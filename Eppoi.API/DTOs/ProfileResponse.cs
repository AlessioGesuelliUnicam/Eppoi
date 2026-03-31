namespace Eppoi.API.DTOs;

public class ProfileResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Interests { get; set; } = new();
    public string? TravelStyle { get; set; }
    public string? TimeAvailability { get; set; }
    public Dictionary<string, double> ProfileVector { get; set; } = new();
    public bool HasCompletedQuestionnaire { get; set; }
}