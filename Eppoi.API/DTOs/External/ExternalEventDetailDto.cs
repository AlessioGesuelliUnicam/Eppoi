namespace Eppoi.API.DTOs.External;

public class ExternalEventDetailDto
{
    public string? Identifier { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? PrimaryImage { get; set; }
    public string? Typology { get; set; }
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public string? Website { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
}