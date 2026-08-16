using back.Core.Interfaces;
using back.Filters;
using back.Middleware;
using back.Repositories;
using back.Security;
using back.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<RequestLoggingActionFilter>();
    options.Filters.Add(new AuthorizeFilter("ApiKeyPolicy"));
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "ApiKey";
    options.DefaultChallengeScheme = "ApiKey";
})
.AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", _ => { });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiKeyPolicy", policy =>
    {
        policy.AddAuthenticationSchemes("ApiKey");
        policy.RequireAuthenticatedUser();
    });
});

builder.Services.AddOpenApi();
builder.Services.AddSingleton<ContactoRepository>();

builder.Services.AddScoped<IContactoService, ContactoService>();
builder.Services.AddScoped<IContactoRepository, ContactoRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Mi API Moderna en .NET 10")
            .WithTheme(ScalarTheme.DeepSpace);
    });
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
