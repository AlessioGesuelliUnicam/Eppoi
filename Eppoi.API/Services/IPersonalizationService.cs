namespace Eppoi.API.Services;

public interface IPersonalizationService
{
    Task<object> GetPersonalizedContentAsync(int userId, int? municipalityId);
}