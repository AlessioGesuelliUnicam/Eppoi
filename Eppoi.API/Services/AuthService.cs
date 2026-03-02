using Eppoi.API.DTOs;
using Eppoi.API.Models;
using Eppoi.API.Repositories;

namespace Eppoi.API.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if email already exists
        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing != null)
            throw new InvalidOperationException("Email already registered.");

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create user
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordHash
        };

        var created = await _userRepository.CreateAsync(user);

        return new AuthResponse
        {
            Id = created.Id,
            Name = created.Name,
            Email = created.Email,
            Message = "Registration successful."
        };
    }
    
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Find user by email
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            throw new InvalidOperationException("Invalid email or password.");

        // Verify password
        var isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isValid)
            throw new InvalidOperationException("Invalid email or password.");

        return new AuthResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Message = "Login successful."
        };
    }
}