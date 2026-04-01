using Xunit;
using Eppoi.API.Services;

namespace Eppoi.Tests;

public class ProfileServiceTests
{
    private readonly ProfileService _profileService;

    public ProfileServiceTests()
    {
        _profileService = new ProfileService(null!);
    }

    [Fact]
    public void BuildProfileVector_WithHistoryAndCulture_SetsArtCultureWeight()
    {
        var interests = new List<string> { "History & Culture" };
        var vector = _profileService.BuildProfileVector(interests);
        Assert.Equal(0.7, vector["ArtCulture"]);
    }

    [Fact]
    public void BuildProfileVector_WithArtMuseums_SetsArtCultureWeight()
    {
        var interests = new List<string> { "Art & Museums" };
        var vector = _profileService.BuildProfileVector(interests);
        Assert.Equal(0.7, vector["ArtCulture"]);
    }

    [Fact]
    public void BuildProfileVector_WithHistoryAndArtMuseums_CapsArtCultureAtOne()
    {
        var interests = new List<string> { "History & Culture", "Art & Museums" };
        var vector = _profileService.BuildProfileVector(interests);
        Assert.Equal(1.0, vector["ArtCulture"]);
    }

    [Fact]
    public void BuildProfileVector_WithNatureAdventure_SetsNatureWeight()
    {
        var interests = new List<string> { "Nature & Adventure" };
        var vector = _profileService.BuildProfileVector(interests);
        Assert.Equal(0.7, vector["Nature"]);
    }

    [Fact]
    public void BuildProfileVector_WithFoodAndWine_SetsEatAndDrinkWeight()
    {
        var interests = new List<string> { "Food & Wine" };
        var vector = _profileService.BuildProfileVector(interests);
        Assert.Equal(1.0, vector["EatAndDrink"]);
    }

    [Fact]
    public void BuildProfileVector_WithShopping_SetsShoppingWeight()
    {
        var interests = new List<string> { "Shopping" };
        var vector = _profileService.BuildProfileVector(interests);
        Assert.Equal(1.0, vector["Shopping"]);
    }

    [Fact]
    public void BuildProfileVector_WithNightlife_SetsEventsWeight()
    {
        var interests = new List<string> { "Nightlife" };
        var vector = _profileService.BuildProfileVector(interests);
        Assert.Equal(0.7, vector["Events"]);
    }

    [Fact]
    public void BuildProfileVector_WithEmptyInterests_AllWeightsAreZero()
    {
        var interests = new List<string>();
        var vector = _profileService.BuildProfileVector(interests);
        Assert.All(vector.Values, v => Assert.Equal(0.0, v));
    }

    [Fact]
    public void BuildProfileVector_WithAllInterests_ContainsAllKeys()
    {
        var interests = new List<string> { "History & Culture", "Nature & Adventure", "Food & Wine", "Shopping", "Nightlife" };
        var vector = _profileService.BuildProfileVector(interests);
        Assert.True(vector.ContainsKey("ArtCulture"));
        Assert.True(vector.ContainsKey("Nature"));
        Assert.True(vector.ContainsKey("EatAndDrink"));
        Assert.True(vector.ContainsKey("Shopping"));
        Assert.True(vector.ContainsKey("Events"));
    }
}