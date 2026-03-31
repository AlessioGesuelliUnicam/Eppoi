using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Eppoi.API.DTOs;
using Eppoi.API.Services;

namespace Eppoi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpPost("questionnaire")]
    public async Task<IActionResult> SaveQuestionnaire([FromBody] QuestionnaireRequest request)
    {
        if (request.Interests == null || request.Interests.Count == 0)
            return BadRequest(new { message = "At least one interest is required." });

        if (string.IsNullOrWhiteSpace(request.TravelStyle))
            return BadRequest(new { message = "Travel style is required." });

        if (string.IsNullOrWhiteSpace(request.TimeAvailability))
            return BadRequest(new { message = "Time availability is required." });

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        try
        {
            var result = await _profileService.SaveQuestionnaireAsync(userId, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var profile = await _profileService.GetProfileAsync(userId);

        if (profile == null)
            return NotFound(new { message = "Profile not found." });

        return Ok(profile);
    }
}