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

    [Fact]
    public async Task VerifyEmailAsync_WithValidToken_SetsEmailVerified()
    {
        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            IsEmailVerified = false,
            EmailVerificationToken = "valid-token"
        };
        _mockUserRepo.Setup(r => r.GetByVerificationTokenAsync("valid-token")).ReturnsAsync(user);
        _mockUserRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        await _authService.VerifyEmailAsync("valid-token");

        Assert.True(user.IsEmailVerified);
        Assert.Null(user.EmailVerificationToken);
    }

    [Fact]
    public async Task VerifyEmailAsync_WithInvalidToken_ThrowsException()
    {
        _mockUserRepo.Setup(r => r.GetByVerificationTokenAsync("invalid-token")).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _authService.VerifyEmailAsync("invalid-token"));
    }

    [Fact]
    public async Task ForgotPasswordAsync_WithExistingEmail_SendsEmail()
    {
        var user = new User { Id = 1, Name = "Test", Email = "test@test.com" };
        _mockUserRepo.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync(user);
        _mockUserRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);
        _mockEmailService.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        await _authService.ForgotPasswordAsync(new ForgotPasswordRequest { Email = "test@test.com" });

        _mockEmailService.Verify(e => e.SendEmailAsync(
            "test@test.com",
            It.IsAny<string>(),
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ForgotPasswordAsync_WithNonExistentEmail_DoesNotSendEmail()
    {
        _mockUserRepo.Setup(r => r.GetByEmailAsync("nobody@test.com")).ReturnsAsync((User?)null);

        await _authService.ForgotPasswordAsync(new ForgotPasswordRequest { Email = "nobody@test.com" });

        _mockEmailService.Verify(e => e.SendEmailAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ResetPasswordAsync_WithValidToken_UpdatesPassword()
    {
        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            PasswordResetToken = "reset-token",
            PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(1)
        };
        _mockUserRepo.Setup(r => r.GetByPasswordResetTokenAsync("reset-token")).ReturnsAsync(user);
        _mockUserRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        await _authService.ResetPasswordAsync(new ResetPasswordRequest
        {
            Token = "reset-token",
            NewPassword = "newpassword123"
        });

        Assert.Null(user.PasswordResetToken);
        Assert.Null(user.PasswordResetTokenExpiresAt);
        Assert.True(BCrypt.Net.BCrypt.Verify("newpassword123", user.PasswordHash));
    }

    [Fact]
    public async Task ResetPasswordAsync_WithExpiredToken_ThrowsException()
    {
        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            PasswordResetToken = "expired-token",
            PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(-1)
        };
        _mockUserRepo.Setup(r => r.GetByPasswordResetTokenAsync("expired-token")).ReturnsAsync(user);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _authService.ResetPasswordAsync(new ResetPasswordRequest
            {
                Token = "expired-token",
                NewPassword = "newpassword123"
            }));
    }
}