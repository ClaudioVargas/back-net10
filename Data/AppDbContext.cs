using back.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace back.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.PasswordHash).IsRequired();
            entity.Property(x => x.Role).IsRequired().HasDefaultValue(nameof(UserRole.User));
            entity.Property(x => x.CreatedAt).IsRequired();
        });

        var passwordHasher = new PasswordHasher<User>();
        var adminUser = new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "System Admin",
            Email = "admin@demo.com",
            PasswordHash = passwordHasher.HashPassword(new User(), "P@ssw0rd"),
            Role = nameof(UserRole.Admin),
            CreatedAt = DateTime.UtcNow
        };

        modelBuilder.Entity<User>().HasData(adminUser);
    }
}
