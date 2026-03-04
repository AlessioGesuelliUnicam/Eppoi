using Eppoi.API.DTOs;

namespace Eppoi.API.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task VerifyEmailAsync(string token);
}