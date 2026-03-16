namespace Eppoi.API.DTOs.External;

public class ExternalPoiDetailDto
{
    public string? Identifier { get; set; }
    public string? OfficialName { get; set; }
    public string? Description { get; set; }
    public string? FullAddress { get; set; }
    public string? PrimaryImagePath { get; set; }
    public string? Type { get; set; }
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public string? Website { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}