using Microsoft.EntityFrameworkCore;
using Eppoi.API.Models;

namespace Eppoi.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Municipality> Municipalities => Set<Municipality>();
    public DbSet<PointOfInterest> PointsOfInterest => Set<PointOfInterest>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Organization> Organizations => Set<Organization>();
}