using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Eppoi.API.Data;

namespace Eppoi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContentController : ControllerBase
{
    private readonly AppDbContext _context;

    public ContentController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("poi")]
    public async Task<IActionResult> GetPoi([FromQuery] int? municipalityId)
    {
        var query = _context.PointsOfInterest.AsQueryable();

        if (municipalityId.HasValue)
            query = query.Where(p => p.MunicipalityId == municipalityId);

        var pois = await query.Select(p => new
        {
            p.Id,
            p.Name,
            p.Description,
            p.Address,
            p.ImagePath,
            p.Type,
            p.Latitude,
            p.Longitude,
            p.MunicipalityId
        }).ToListAsync();

        return Ok(pois);
    }

    [HttpGet("events")]
    public async Task<IActionResult> GetEvents([FromQuery] int? municipalityId)
    {
        var query = _context.Events.AsQueryable();

        if (municipalityId.HasValue)
            query = query.Where(e => e.MunicipalityId == municipalityId);

        var events = await query.Select(e => new
        {
            e.Id,
            e.Title,
            e.Description,
            e.Address,
            e.ImagePath,
            e.Typology,
            e.StartDate,
            e.EndDate,
            e.Latitude,
            e.Longitude,
            e.MunicipalityId
        }).ToListAsync();

        return Ok(events);
    }

    [HttpGet("articles")]
    public async Task<IActionResult> GetArticles([FromQuery] int? municipalityId)
    {
        var query = _context.Articles.AsQueryable();

        if (municipalityId.HasValue)
            query = query.Where(a => a.MunicipalityId == municipalityId);

        var articles = await query.Select(a => new
        {
            a.Id,
            a.Title,
            a.Subtitle,
            a.ImagePath,
            a.UpdatedAt,
            a.MunicipalityId
        }).ToListAsync();

        return Ok(articles);
    }

    [HttpGet("organizations")]
    public async Task<IActionResult> GetOrganizations([FromQuery] int? municipalityId)
    {
        var query = _context.Organizations.AsQueryable();

        if (municipalityId.HasValue)
            query = query.Where(o => o.MunicipalityId == municipalityId);

        var organizations = await query.Select(o => new
        {
            o.Id,
            o.Name,
            o.Description,
            o.Address,
            o.ImagePath,
            o.Type,
            o.Latitude,
            o.Longitude,
            o.MunicipalityId
        }).ToListAsync();

        return Ok(organizations);
    }

    [HttpGet("municipalities")]
    public async Task<IActionResult> GetMunicipalities()
    {
        var municipalities = await _context.Municipalities
            .Select(m => new { m.Id, m.Name, m.LogoPath })
            .ToListAsync();

        return Ok(municipalities);
    }
}