using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Eppoi.API.Data;
using Eppoi.API.DTOs;


namespace Eppoi.API.Services;

public class ChatService : IChatService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;
    private const string OllamaUrl = "http://localhost:11434/api/chat";
    private const string Model = "llama3.2:3b";

    public ChatService(AppDbContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    public async Task<ChatResponse> SendMessageAsync(ChatRequest request)
    {
        // Build context from DB
        var context = await BuildContextAsync(request.MunicipalityId);
        var municipalityName = await GetMunicipalityNameAsync(request.MunicipalityId);

        // Build system prompt
        var systemPrompt = BuildSystemPrompt(municipalityName, context);

        // Call Ollama
        var response = await CallOllamaAsync(systemPrompt, request.Message);

        // Check if out of domain
        var isOutOfDomain = response.Contains("posso rispondere solo a domande", StringComparison.OrdinalIgnoreCase);

        return new ChatResponse
        {
            Message = response,
            IsOutOfDomain = isOutOfDomain
        };
    }

    private async Task<string> BuildContextAsync(int? municipalityId)
    {
        var sb = new StringBuilder();

        // POIs
        var poisQuery = _context.PointsOfInterest.AsQueryable();
        if (municipalityId.HasValue)
            poisQuery = poisQuery.Where(p => p.MunicipalityId == municipalityId);

        var pois = await poisQuery.Take(20).ToListAsync();
        if (pois.Any())
        {
            sb.AppendLine("LUOGHI DI INTERESSE:");
            foreach (var poi in pois)
                sb.AppendLine($"- {poi.Name} ({poi.Type}): {poi.Address}. {poi.Description?.Substring(0, Math.Min(poi.Description.Length, 200))}...");
        }

        // Events
        var eventsQuery = _context.Events.AsQueryable();
        if (municipalityId.HasValue)
            eventsQuery = eventsQuery.Where(e => e.MunicipalityId == municipalityId);

        var events = await eventsQuery.Take(10).ToListAsync();
        if (events.Any())
        {
            sb.AppendLine("\nEVENTI:");
            foreach (var evt in events)
                sb.AppendLine($"- {evt.Title}: {evt.Address}. {evt.Description?.Substring(0, Math.Min(evt.Description?.Length ?? 0, 150))}...");
        }

        // Organizations
        var orgsQuery = _context.Organizations.AsQueryable();
        if (municipalityId.HasValue)
            orgsQuery = orgsQuery.Where(o => o.MunicipalityId == municipalityId);

        var orgs = await orgsQuery.Take(10).ToListAsync();
        if (orgs.Any())
        {
            sb.AppendLine("\nORGANIZZAZIONI:");
            foreach (var org in orgs)
                sb.AppendLine($"- {org.Name} ({org.Type}): {org.Address}.");
        }

        return sb.ToString();
    }

    private async Task<string> GetMunicipalityNameAsync(int? municipalityId)
    {
        if (!municipalityId.HasValue) return "il comune";
        var municipality = await _context.Municipalities.FindAsync(municipalityId);
        return municipality?.Name ?? "il comune";
    }

    private static string BuildSystemPrompt(string municipalityName, string context)
    {
        return $"""
            Sei un assistente turistico per {municipalityName}.
            Rispondi SOLO a domande riguardanti il turismo locale: luoghi di interesse, eventi, organizzazioni e attrazioni del comune.
            
            Per rispondere, usa SOLO le seguenti informazioni disponibili:
            
            {context}
            
            Se la domanda non riguarda il turismo di {municipalityName}, rispondi esattamente con:
            'Mi dispiace, posso rispondere solo a domande sul turismo di {municipalityName}.'
            
            Rispondi sempre in italiano, con un tono amichevole e informativo.
            Non inventare informazioni non presenti nel contesto fornito.
            """;
    }

    private async Task<string> CallOllamaAsync(string systemPrompt, string userMessage)
    {
        var payload = new
        {
            model = Model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userMessage }
            },
            stream = false
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(OllamaUrl, content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseJson);

        return result
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "Errore nella generazione della risposta.";
    }
}