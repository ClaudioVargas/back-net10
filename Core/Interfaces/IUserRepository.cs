using back.Core.Models;

namespace back.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmail(string email);
    Task<bool> ExistsByEmail(string email);
    Task<User> Add(User user);
}
