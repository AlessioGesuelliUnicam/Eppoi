namespace Eppoi.API.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsEmailVerified { get; set; } = false;
    public string? EmailVerificationToken { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiresAt { get; set; }
    
    // Profile
    public string? Interests { get; set; }
    public string? TravelStyle { get; set; }
    public string? TimeAvailability { get; set; }
    public string? ProfileVector { get; set; }
    public bool HasCompletedQuestionnaire { get; set; } = false;
}