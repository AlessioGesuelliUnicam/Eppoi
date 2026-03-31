using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Eppoi.API.Data;
using Eppoi.API.Models;

namespace Eppoi.API.Services;

public class PersonalizationService : IPersonalizationService
{
    private readonly AppDbContext _context;
    private const int MinResults = 10;

    public PersonalizationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<object> GetPersonalizedContentAsync(int userId, int? municipalityId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || !user.HasCompletedQuestionnaire)
            return await GetFallbackContentAsync(municipalityId, stage: 4);

        var vector = user.ProfileVector != null
            ? JsonSerializer.Deserialize<Dictionary<string, double>>(user.ProfileVector) ?? new()
            : new Dictionary<string, double>();

        // Stage 1: preferences + category + proximity
        var result = await GetScoredContentAsync(vector, municipalityId);
        if (result.Count >= MinResults) return result;

        // Stage 2: preferences + category (without proximity)
        result = await GetScoredContentAsync(vector, municipalityId: null);
        if (result.Count >= MinResults) return result;

        // Stage 3: category only
        result = await GetFallbackContentAsync(municipalityId, stage: 3);
        if (result.Count >= MinResults) return result;

        // Stage 4: All content available
        return await GetFallbackContentAsync(municipalityId: null, stage: 4);
    }

    private async Task<List<object>> GetScoredContentAsync(
        Dictionary<string, double> vector, int? municipalityId)
    {
        var results = new List<(object item, double score)>();

        // POI
        var poisQuery = _context.PointsOfInterest.AsQueryable();
        if (municipalityId.HasValue)
            poisQuery = poisQuery.Where(p => p.MunicipalityId == municipalityId);

        var pois = await poisQuery.ToListAsync();
        foreach (var poi in pois)
        {
            var score = ComputePoiScore(poi, vector);
            results.Add((new
            {
                id = poi.Id,
                contentType = "POI",
                name = poi.Name,
                description = poi.Description,
                address = poi.Address,
                imagePath = poi.ImagePath,
                type = poi.Type,
                latitude = poi.Latitude,
                longitude = poi.Longitude,
                municipalityId = poi.MunicipalityId,
                score
            }, score));
        }

        // Events
        var eventsQuery = _context.Events.AsQueryable();
        if (municipalityId.HasValue)
            eventsQuery = eventsQuery.Where(e => e.MunicipalityId == municipalityId);

        var events = await eventsQuery.ToListAsync();
        foreach (var evt in events)
        {
            var score = vector.GetValueOrDefault("Events", 0.0) * 0.6 + 0.3;
            results.Add((new
            {
                id = evt.Id,
                contentType = "Event",
                title = evt.Title,
                description = evt.Description,
                address = evt.Address,
                imagePath = evt.ImagePath,
                startDate = evt.StartDate,
                endDate = evt.EndDate,
                municipalityId = evt.MunicipalityId,
                score
            }, score));
        }

        return results
            .OrderByDescending(r => r.score)
            .Select(r => r.item)
            .ToList();
    }

    private async Task<List<object>> GetFallbackContentAsync(int? municipalityId, int stage)
    {
        var results = new List<object>();

        var poisQuery = _context.PointsOfInterest.AsQueryable();
        if (municipalityId.HasValue && stage < 4)
            poisQuery = poisQuery.Where(p => p.MunicipalityId == municipalityId);

        var pois = await poisQuery.ToListAsync();
        results.AddRange(pois.Select(p => (object)new
        {
            id = p.Id,
            contentType = "POI",
            name = p.Name,
            description = p.Description,
            address = p.Address,
            imagePath = p.ImagePath,
            type = p.Type,
            latitude = p.Latitude,
            longitude = p.Longitude,
            municipalityId = p.MunicipalityId,
            score = 0.0
        }));

        var eventsQuery = _context.Events.AsQueryable();
        if (municipalityId.HasValue && stage < 4)
            eventsQuery = eventsQuery.Where(e => e.MunicipalityId == municipalityId);

        var events = await eventsQuery.ToListAsync();
        results.AddRange(events.Select(e => (object)new
        {
            id = e.Id,
            contentType = "Event",
            title = e.Title,
            description = e.Description,
            address = e.Address,
            imagePath = e.ImagePath,
            startDate = e.StartDate,
            endDate = e.EndDate,
            municipalityId = e.MunicipalityId,
            score = 0.0
        }));

        return results;
    }

    private static double ComputePoiScore(PointOfInterest poi, Dictionary<string, double> vector)
    {
        var preferenceScore = poi.Type switch
        {
            "ArtCulture" => vector.GetValueOrDefault("ArtCulture", 0.0),
            "Nature" => vector.GetValueOrDefault("Nature", 0.0),
            _ => 0.0
        };

        // 60% user preferences + 30% category + 10% fixed
        return preferenceScore * 0.6 + 0.3 + 0.1;
    }
}