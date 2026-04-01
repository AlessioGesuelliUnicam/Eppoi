using Xunit;
using Moq;
using Eppoi.API.Services;
using Eppoi.API.Repositories;
using Eppoi.API.DTOs;
using Eppoi.API.Models;
using Microsoft.Extensions.Configuration;

namespace Eppoi.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mockConfig = new Mock<IConfiguration>();

        _mockConfig.Setup(c => c["Jwt:Key"]).Returns("questa-e-una-chiave-segreta-molto-lunga-per-eppoi-app");
        _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("eppoi-api");
        _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("eppoi-frontend");

        _authService = new AuthService(_mockUserRepo.Object, _mockConfig.Object, _mockEmailService.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithNewEmail_ReturnsAuthResponse()
    {
        _mockUserRepo.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync((User?)null);
        _mockUserRepo.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);
        _mockEmailService.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        var request = new RegisterRequest { Name = "Test", Email = "test@test.com", Password = "123456" };
        var result = await _authService.RegisterAsync(request);

        Assert.Equal("Test", result.Name);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ThrowsException()
    {
        var existingUser = new User { Email = "test@test.com" };
        _mockUserRepo.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync(existingUser);

        var request = new RegisterRequest { Name = "Test", Email = "test@test.com", Password = "123456" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.RegisterAsync(request));
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsTokenInResponse()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        var user = new User { Id = 1, Name = "Test", Email = "test@test.com", PasswordHash = passwordHash };
        _mockUserRepo.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync(user);

        var request = new LoginRequest { Email = "test@test.com", Password = "123456" };
        var result = await _authService.LoginAsync(request);

        Assert.NotEmpty(result.Token);
        Assert.Equal("Login successful.", result.Message);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ThrowsException()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        var user = new User { Id = 1, Name = "Test", Email = "test@test.com", PasswordHash = passwordHash };
        _mockUserRepo.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync(user);

        var request = new LoginRequest { Email = "test@test.com", Password = "wrongpassword" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentEmail_ThrowsException()
    {
        _mockUserRepo.Setup(r => r.GetByEmailAsync("nobody@test.com")).ReturnsAsync((User?)null);

        var request = new LoginRequest { Email = "nobody@test.com", Password = "123456" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.LoginAsync(request));
    }
}