using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Eppoi.API.DTOs;
using Eppoi.API.Models;
using Eppoi.API.Repositories;

namespace Eppoi.API.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(IUserRepository userRepository, IConfiguration configuration, IEmailService emailService)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing != null)
            throw new InvalidOperationException("Email already registered.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var verificationToken = Guid.NewGuid().ToString();

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordHash,
            IsEmailVerified = false,
            EmailVerificationToken = verificationToken
        };

        var created = await _userRepository.CreateAsync(user);

        var verificationLink = $"http://localhost:5052/api/Auth/verify-email?token={verificationToken}";
        await _emailService.SendEmailAsync(
            created.Email,
            "Verifica la tua email — Eppoi",
            EmailTemplates.EmailVerification(created.Name, verificationLink)
        );

        return new AuthResponse
        {
            Id = created.Id,
            Name = created.Name,
            Email = created.Email,
            Message = "Registration successful. Please check your email to verify your account.",
            Token = string.Empty
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            throw new InvalidOperationException("Invalid email or password.");

        var isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isValid)
            throw new InvalidOperationException("Invalid email or password.");

        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Message = "Login successful.",
            Token = token
        };
    }

    public async Task VerifyEmailAsync(string token)
    {
        var user = await _userRepository.GetByVerificationTokenAsync(token);
        if (user == null)
            throw new InvalidOperationException("Invalid verification token.");

        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        await _userRepository.UpdateAsync(user);
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            return; // Non rivelare se l'email esiste o meno

        var resetToken = Guid.NewGuid().ToString();
        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);
        await _userRepository.UpdateAsync(user);

        var resetLink = $"http://localhost:5173/reset-password?token={resetToken}";
        await _emailService.SendEmailAsync(
            user.Email,
            "Reset password — Eppoi",
            EmailTemplates.PasswordReset(user.Name, resetLink)
        );
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userRepository.GetByPasswordResetTokenAsync(request.Token);
        if (user == null)
            throw new InvalidOperationException("Invalid or expired reset token.");

        if (user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
            throw new InvalidOperationException("Invalid or expired reset token.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiresAt = null;
        await _userRepository.UpdateAsync(user);
    }
}