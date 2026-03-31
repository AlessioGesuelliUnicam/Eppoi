using Eppoi.API.DTOs;

namespace Eppoi.API.Services;

public interface IProfileService
{
    Task<ProfileResponse> SaveQuestionnaireAsync(int userId, QuestionnaireRequest request);
    Task<ProfileResponse?> GetProfileAsync(int userId);
    Dictionary<string, double> BuildProfileVector(List<string> interests);
}