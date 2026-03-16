namespace Eppoi.API.Services;

public interface IIngestionService
{
    Task IngestMunicipalityAsync(string municipalityName);
}