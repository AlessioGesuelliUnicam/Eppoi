using Xunit;
using Eppoi.API.Services;
using Eppoi.API.Models;

namespace Eppoi.Tests;

public class PersonalizationServiceTests
{
    private readonly PersonalizationService _personalizationService;

    public PersonalizationServiceTests()
    {
        _personalizationService = new PersonalizationService(null!);
    }

    [Fact]
    public void ComputePoiScore_ArtCultureWithFullWeight_ReturnsMaxScore()
    {
        var poi = new PointOfInterest { Type = "ArtCulture" };
        var vector = new Dictionary<string, double> { { "ArtCulture", 1.0 }, { "Nature", 0.0 } };

        var score = InvokeComputePoiScore(poi, vector);

        Assert.Equal(1.0, score, precision: 1);
    }

    [Fact]
    public void ComputePoiScore_NatureWithFullWeight_ReturnsMaxScore()
    {
        var poi = new PointOfInterest { Type = "Nature" };
        var vector = new Dictionary<string, double> { { "ArtCulture", 0.0 }, { "Nature", 1.0 } };

        var score = InvokeComputePoiScore(poi, vector);

        Assert.Equal(1.0, score, precision: 1);
    }

    [Fact]
    public void ComputePoiScore_ArtCultureWithZeroWeight_ReturnsBaseScore()
    {
        var poi = new PointOfInterest { Type = "ArtCulture" };
        var vector = new Dictionary<string, double> { { "ArtCulture", 0.0 }, { "Nature", 0.0 } };

        var score = InvokeComputePoiScore(poi, vector);

        Assert.Equal(0.4, score);
    }

    [Fact]
    public void ComputePoiScore_UnknownType_ReturnsBaseScore()
    {
        var poi = new PointOfInterest { Type = "Unknown" };
        var vector = new Dictionary<string, double> { { "ArtCulture", 1.0 }, { "Nature", 1.0 } };

        var score = InvokeComputePoiScore(poi, vector);

        Assert.Equal(0.4, score);
    }

    [Fact]
    public void ComputePoiScore_ArtCultureWithHalfWeight_ReturnsMidScore()
    {
        var poi = new PointOfInterest { Type = "ArtCulture" };
        var vector = new Dictionary<string, double> { { "ArtCulture", 0.7 }, { "Nature", 0.0 } };

        var score = InvokeComputePoiScore(poi, vector);

        Assert.Equal(0.82, score, precision: 2);
    }

    // Helper per invocare il metodo privato ComputePoiScore tramite reflection
    private static double InvokeComputePoiScore(PointOfInterest poi, Dictionary<string, double> vector)
    {
        var method = typeof(PersonalizationService)
            .GetMethod("ComputePoiScore",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        return (double)method!.Invoke(null, new object[] { poi, vector })!;
    }
}