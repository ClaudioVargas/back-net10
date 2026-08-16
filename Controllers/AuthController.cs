using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using back.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace back.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Email y password son obligatorios.");
        }

        const string validEmail = "admin@demo.com";
        const string validPassword = "P@ssw0rd";

        if (!string.Equals(request.Email, validEmail, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(request.Password, validPassword, StringComparison.Ordinal))
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
            new Claim(JwtRegisteredClaimNames.Sub, request.Email),
            new Claim(JwtRegisteredClaimNames.Email, request.Email),
            new Claim(ClaimTypes.Name, request.Email),
            new Claim(ClaimTypes.Role, "Admin")
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
            user = request.Email
        });
    }
}
