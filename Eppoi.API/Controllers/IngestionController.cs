using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Eppoi.API.Services;

namespace Eppoi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngestionController : ControllerBase
{
    private readonly IIngestionService _ingestionService;

    public IngestionController(IIngestionService ingestionService)
    {
        _ingestionService = ingestionService;
    }

    [Authorize]
    [HttpPost("ingest")]
    public async Task<IActionResult> Ingest([FromQuery] string municipality)
    {
        if (string.IsNullOrWhiteSpace(municipality))
            return BadRequest(new { message = "Municipality name is required." });

        try
        {
            await _ingestionService.IngestMunicipalityAsync(municipality);
            return Ok(new { message = $"Ingestion completed for {municipality}." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Ingestion failed: {ex.Message}" });
        }
    }
}