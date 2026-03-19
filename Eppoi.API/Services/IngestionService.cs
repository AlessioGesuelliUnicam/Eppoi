using System.Text.Json;
using Eppoi.API.Data;
using Eppoi.API.DTOs.External;
using Eppoi.API.Models;

namespace Eppoi.API.Services;

public class IngestionService : IIngestionService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://apispm.eppoi.io";

    public IngestionService(AppDbContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    public async Task IngestMunicipalityAsync(string municipalityName)
    {
        // Extract short name for API calls (e.g. "Comune di Gradara" → "Gradara")
        var apiName = municipalityName.Replace("Comune di ", "").Trim();

        // Create or retrieve municipality
        var municipality = _context.Municipalities
            .FirstOrDefault(m => m.Name == municipalityName);

        if (municipality == null)
        {
            municipality = new Municipality { Name = municipalityName };
            _context.Municipalities.Add(municipality);
            await _context.SaveChangesAsync();
        }

        await IngestPoisAsync(municipality, apiName);
        await IngestEventsAsync(municipality, apiName);
        await IngestArticlesAsync(municipality, apiName);
        await IngestOrganizationsAsync(municipality, apiName);
    }

    private async Task IngestPoisAsync(Municipality municipality, string apiName)
    {
        // ArtCulture POIs
        var artCultureCards = await FetchAsync<List<ExternalPoiCardDto>>(
            $"{BaseUrl}/api/art-culture/card-list?municipality={Uri.EscapeDataString(apiName)}&language=it");

        if (artCultureCards != null)
        {
            foreach (var card in artCultureCards)
            {
                if (string.IsNullOrWhiteSpace(card.EntityId)) continue;
                if (_context.PointsOfInterest.Any(p => p.ExternalId == card.EntityId)) continue;

                var detail = await FetchAsync<ExternalPoiDetailDto>(
                    $"{BaseUrl}/api/art-culture/detail/{card.EntityId}?language=it");

                _context.PointsOfInterest.Add(new PointOfInterest
                {
                    ExternalId = card.EntityId,
                    Name = detail?.OfficialName ?? card.EntityName ?? "Unknown",
                    Description = detail?.Description,
                    Address = detail?.FullAddress ?? card.Address,
                    ImagePath = card.ImagePath,
                    Type = "ArtCulture",
                    Email = detail?.Email,
                    Telephone = detail?.Telephone,
                    Website = detail?.Website,
                    Latitude = detail?.Latitude ?? 0,
                    Longitude = detail?.Longitude ?? 0,
                    MunicipalityId = municipality.Id
                });
            }
            await _context.SaveChangesAsync();
        }

        // Nature POIs
        var natureCards = await FetchAsync<List<ExternalPoiCardDto>>(
            $"{BaseUrl}/api/nature/card-list?municipality={Uri.EscapeDataString(apiName)}&language=it");

        if (natureCards != null)
        {
            foreach (var card in natureCards)
            {
                if (string.IsNullOrWhiteSpace(card.EntityId)) continue;
                if (_context.PointsOfInterest.Any(p => p.ExternalId == card.EntityId)) continue;

                var detail = await FetchAsync<ExternalPoiDetailDto>(
                    $"{BaseUrl}/api/nature/detail/{card.EntityId}?language=it");

                _context.PointsOfInterest.Add(new PointOfInterest
                {
                    ExternalId = card.EntityId,
                    Name = detail?.OfficialName ?? card.EntityName ?? "Unknown",
                    Description = detail?.Description,
                    Address = detail?.FullAddress ?? card.Address,
                    ImagePath = card.ImagePath,
                    Type = "Nature",
                    Email = detail?.Email,
                    Telephone = detail?.Telephone,
                    Website = detail?.Website,
                    Latitude = detail?.Latitude ?? 0,
                    Longitude = detail?.Longitude ?? 0,
                    MunicipalityId = municipality.Id
                });
            }
            await _context.SaveChangesAsync();
        }
    }

    private async Task IngestEventsAsync(Municipality municipality, string apiName)
    {
        var cards = await FetchAsync<List<ExternalEventCardDto>>(
            $"{BaseUrl}/api/events/card-list?municipality={Uri.EscapeDataString(apiName)}&language=it");

        if (cards == null) return;

        foreach (var card in cards)
        {
            if (string.IsNullOrWhiteSpace(card.EntityId)) continue;
            if (_context.Events.Any(e => e.ExternalId == card.EntityId)) continue;

            var detail = await FetchAsync<ExternalEventDetailDto>(
                $"{BaseUrl}/api/events/detail/{card.EntityId}?language=it");

            _context.Events.Add(new Event
            {
                ExternalId = card.EntityId,
                Title = detail?.Title ?? card.EntityName ?? "Unknown",
                Description = detail?.Description,
                Address = detail?.Address ?? card.Address,
                ImagePath = card.ImagePath,                Typology = detail?.Typology,
                Email = detail?.Email,
                Telephone = detail?.Telephone,
                Website = detail?.Website,
                Latitude = detail?.Latitude ?? 0,
                Longitude = detail?.Longitude ?? 0,
                StartDate = ParseDate(detail?.StartDate),
                EndDate = ParseDate(detail?.EndDate),
                MunicipalityId = municipality.Id
            });
        }
        await _context.SaveChangesAsync();
    }

    private async Task IngestArticlesAsync(Municipality municipality, string apiName)
    {
        var cards = await FetchAsync<List<ExternalArticleCardDto>>(
            $"{BaseUrl}/api/articles/card-list?municipality={Uri.EscapeDataString(apiName)}&language=it");

        if (cards == null) return;

        foreach (var card in cards)
        {
            if (string.IsNullOrWhiteSpace(card.EntityId)) continue;
            if (_context.Articles.Any(a => a.ExternalId == card.EntityId)) continue;

            var detail = await FetchAsync<ExternalArticleDetailDto>(
                $"{BaseUrl}/api/articles/detail/{card.EntityId}?language=it");

            _context.Articles.Add(new Article
            {
                ExternalId = card.EntityId,
                Title = detail?.Title ?? card.EntityName ?? "Unknown",
                Subtitle = detail?.Subtitle,
                Script = detail?.Script,
                ImagePath = card.ImagePath,
                UpdatedAt = ParseDate(detail?.UpdatedAt) ?? DateTime.UtcNow,
                MunicipalityId = municipality.Id
            });
        }
        await _context.SaveChangesAsync();
    }

    private async Task IngestOrganizationsAsync(Municipality municipality, string apiName)
    {
        var cards = await FetchAsync<List<ExternalOrganizationCardDto>>(
            $"{BaseUrl}/api/organizations/card-list?municipality={Uri.EscapeDataString(apiName)}&language=it");

        if (cards == null) return;

        foreach (var card in cards)
        {
            if (string.IsNullOrWhiteSpace(card.EntityId)) continue;
            if (_context.Organizations.Any(o => o.ExternalId == card.EntityId)) continue;

            var detail = await FetchAsync<ExternalOrganizationDetailDto>(
                $"{BaseUrl}/api/organizations/detail/{card.EntityId}?language=it");

            _context.Organizations.Add(new Organization
            {
                ExternalId = card.EntityId,
                Name = detail?.LegalName ?? card.EntityName ?? "Unknown",
                Description = detail?.Description,
                Address = detail?.Address ?? card.Address,
                ImagePath = card.ImagePath,
                Type = detail?.Type,
                Email = detail?.Email,
                Telephone = detail?.Telephone,
                Website = detail?.Website,
                Latitude = detail?.Latitude ?? 0,
                Longitude = detail?.Longitude ?? 0,
                MunicipalityId = municipality.Id
            });
        }
        await _context.SaveChangesAsync();
    }

    private async Task<T?> FetchAsync<T>(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return default;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            return default;
        }
    }

    private static DateTime? ParseDate(string? dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString)) return null;
        if (DateTime.TryParse(dateString, out var date))
            return DateTime.SpecifyKind(date, DateTimeKind.Utc);
        return null;
    }
}