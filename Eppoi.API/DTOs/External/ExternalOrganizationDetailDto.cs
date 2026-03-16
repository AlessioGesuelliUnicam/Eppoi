namespace Eppoi.API.DTOs.External;

public class ExternalOrganizationDetailDto
{
    public string? TaxCode { get; set; }
    public string? LegalName { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? PrimaryImagePath { get; set; }
    public string? Type { get; set; }
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public string? Website { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}