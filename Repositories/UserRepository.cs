using back.Core.Interfaces;
using back.Core.Models;
using back.Data;
using Microsoft.EntityFrameworkCore;

namespace back.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == email.Trim());
    }

    public async Task<bool> ExistsByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return await _context.Users.AnyAsync(user => user.Email == email.Trim());
    }

    public async Task<User> Add(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
