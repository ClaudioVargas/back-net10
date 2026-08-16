using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using back.Core.Interfaces;
using back.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace back.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthController(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    [AllowAnonymous]
    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Nombre, email y password son obligatorios.");
        }

        var email = request.Email.Trim();
        if (await _userRepository.ExistsByEmail(email))
        {
            return Conflict(new
            {
                success = false,
                message = "El email ya está registrado."
            });
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            Role = nameof(UserRole.User),
            PasswordHash = _passwordHasher.HashPassword(new User(), request.Password)
        };

        await _userRepository.Add(user);

        return StatusCode(StatusCodes.Status201Created, new
        {
            success = true,
            message = "Usuario registrado correctamente.",
            user = new
            {
                user.Id,
                user.Name,
                user.Email,
                user.CreatedAt
            }
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Email y password son obligatorios.");
        }

        var user = await _userRepository.GetByEmail(request.Email.Trim());
        if (user == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "Credenciales inválidas."
            });
        }

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new
            {
                success = false,
                message = "Credenciales inválidas."
            });
        }

        var jwtSettings = _configuration.GetSection("JwtSettings");
        var issuer = jwtSettings["Issuer"] ?? "back-api";
        var audience = jwtSettings["Audience"] ?? "back-clients";
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("No se configuró la clave JWT.");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            success = true,
            token = tokenString,
            expiresAt = token.ValidTo,
            user = user.Email,
            role = user.Role
        });
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpGet("admin-check")]
    public IActionResult AdminCheck()
    {
        return Ok(new
        {
            success = true,
            message = "Acceso autorizado como administrador.",
            user = User.Identity?.Name,
            role = User.FindFirstValue(ClaimTypes.Role)
        });
    }
}
