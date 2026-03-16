namespace Eppoi.API.Models;

public class Municipality
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoPath { get; set; }
    
    public ICollection<PointOfInterest> PointsOfInterest { get; set; } = new List<PointOfInterest>();
    public ICollection<Event> Events { get; set; } = new List<Event>();
    public ICollection<Article> Articles { get; set; } = new List<Article>();
    public ICollection<Organization> Organizations { get; set; } = new List<Organization>();
}