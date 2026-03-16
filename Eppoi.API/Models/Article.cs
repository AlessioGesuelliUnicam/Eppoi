namespace Eppoi.API.Models;

public class Article
{
    public int Id { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Script { get; set; }
    public string? ImagePath { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public int MunicipalityId { get; set; }
    public Municipality Municipality { get; set; } = null!;
}