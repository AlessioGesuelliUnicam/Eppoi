using Xunit;
using Eppoi.API.Services;
using Eppoi.API.DTOs.External;

namespace Eppoi.Tests;

public class IngestionServiceTests
{
    [Fact]
    public void ParseDate_WithValidDate_ReturnsUtcDateTime()
    {
        var method = typeof(IngestionService)
            .GetMethod("ParseDate",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var result = (DateTime?)method!.Invoke(null, new object[] { "2026-01-15T10:00:00" });

        Assert.NotNull(result);
        Assert.Equal(DateTimeKind.Utc, result!.Value.Kind);
        Assert.Equal(2026, result.Value.Year);
        Assert.Equal(1, result.Value.Month);
        Assert.Equal(15, result.Value.Day);
    }

    [Fact]
    public void ParseDate_WithNullString_ReturnsNull()
    {
        var method = typeof(IngestionService)
            .GetMethod("ParseDate",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var result = (DateTime?)method!.Invoke(null, new object?[] { null });

        Assert.Null(result);
    }

    [Fact]
    public void ParseDate_WithEmptyString_ReturnsNull()
    {
        var method = typeof(IngestionService)
            .GetMethod("ParseDate",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var result = (DateTime?)method!.Invoke(null, new object[] { "" });

        Assert.Null(result);
    }

    [Fact]
    public void ParseDate_WithInvalidString_ReturnsNull()
    {
        var method = typeof(IngestionService)
            .GetMethod("ParseDate",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var result = (DateTime?)method!.Invoke(null, new object[] { "not-a-date" });

        Assert.Null(result);
    }

    [Fact]
    public void ExternalPoiCardDto_WithValidData_HasCorrectProperties()
    {
        var dto = new ExternalPoiCardDto
        {
            EntityId = "test-guid",
            EntityName = "Test POI",
            ImagePath = "/Media/POI/test.webp",
            Address = "Via Test, 1"
        };

        Assert.Equal("test-guid", dto.EntityId);
        Assert.Equal("Test POI", dto.EntityName);
        Assert.Equal("/Media/POI/test.webp", dto.ImagePath);
        Assert.Equal("Via Test, 1", dto.Address);
    }

    [Fact]
    public void ExternalEventCardDto_WithValidData_HasCorrectProperties()
    {
        var dto = new ExternalEventCardDto
        {
            EntityId = "event-guid",
            EntityName = "Test Event",
            Date = "2026-06-15"
        };

        Assert.Equal("event-guid", dto.EntityId);
        Assert.Equal("Test Event", dto.EntityName);
        Assert.Equal("2026-06-15", dto.Date);
    }

    [Fact]
    public void ExternalOrganizationCardDto_WithNullEntityId_EntityIdIsNull()
    {
        var dto = new ExternalOrganizationCardDto
        {
            EntityId = null,
            EntityName = "Test Org"
        };

        Assert.Null(dto.EntityId);
        Assert.Equal("Test Org", dto.EntityName);
    }
}