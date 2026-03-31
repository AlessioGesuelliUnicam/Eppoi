using System.Text.Json;
using Eppoi.API.Data;
using Eppoi.API.DTOs;

namespace Eppoi.API.Services;

public class ProfileService : IProfileService
{
    private readonly AppDbContext _context;

    public ProfileService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProfileResponse> SaveQuestionnaireAsync(int userId, QuestionnaireRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        var vector = BuildProfileVector(request.Interests);

        user.Interests = JsonSerializer.Serialize(request.Interests);
        user.TravelStyle = request.TravelStyle;
        user.TimeAvailability = request.TimeAvailability;
        user.ProfileVector = JsonSerializer.Serialize(vector);
        user.HasCompletedQuestionnaire = true;

        await _context.SaveChangesAsync();

        return BuildResponse(user, vector);
    }

    public async Task<ProfileResponse?> GetProfileAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        var vector = user.ProfileVector != null
            ? JsonSerializer.Deserialize<Dictionary<string, double>>(user.ProfileVector) ?? new()
            : new Dictionary<string, double>();

        return BuildResponse(user, vector);
    }

    public Dictionary<string, double> BuildProfileVector(List<string> interests)
    {
        var vector = new Dictionary<string, double>
        {
            { "ArtCulture", 0.0 },
            { "Nature", 0.0 },
            { "EatAndDrink", 0.0 },
            { "Shopping", 0.0 },
            { "Events", 0.0 },
            { "Organizations", 0.0 }
        };

        foreach (var interest in interests)
        {
            switch (interest)
            {
                case "History & Culture":
                case "Art & Museums":
                    vector["ArtCulture"] = Math.Min(1.0, vector["ArtCulture"] + 0.7);
                    break;
                case "Nature & Adventure":
                case "Wellness & Relaxation":
                case "Beach & Sea":
                    vector["Nature"] = Math.Min(1.0, vector["Nature"] + 0.7);
                    break;
                case "Food & Wine":
                    vector["EatAndDrink"] = 1.0;
                    break;
                case "Shopping":
                    vector["Shopping"] = 1.0;
                    break;
                case "Nightlife":
                    vector["Events"] = Math.Min(1.0, vector["Events"] + 0.7);
                    break;
            }
        }

        return vector;
    }

    private static ProfileResponse BuildResponse(Models.User user, Dictionary<string, double> vector)
    {
        var interests = user.Interests != null
            ? JsonSerializer.Deserialize<List<string>>(user.Interests) ?? new()
            : new List<string>();

        return new ProfileResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Interests = interests,
            TravelStyle = user.TravelStyle,
            TimeAvailability = user.TimeAvailability,
            ProfileVector = vector,
            HasCompletedQuestionnaire = user.HasCompletedQuestionnaire
        };
    }
}