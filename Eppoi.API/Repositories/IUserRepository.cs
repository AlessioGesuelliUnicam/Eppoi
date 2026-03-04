using Eppoi.API.Models;

namespace Eppoi.API.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByVerificationTokenAsync(string token);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
}