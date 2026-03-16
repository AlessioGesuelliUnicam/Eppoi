namespace Eppoi.API.Models;

public class Organization
{
    public int Id { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? ImagePath { get; set; }
    public string? Type { get; set; }
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public string? Website { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public int MunicipalityId { get; set; }
    public Municipality Municipality { get; set; } = null!;
}