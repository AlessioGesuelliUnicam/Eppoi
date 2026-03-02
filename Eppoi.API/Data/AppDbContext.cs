using Microsoft.EntityFrameworkCore;
using Eppoi.API.Models;

namespace Eppoi.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
}