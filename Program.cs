using back.Core.Interfaces;
using back.Middleware;
using back.Repositories;
using back.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<ContactoRepository>();

builder.Services.AddScoped<IContactoService, ContactoService>();
builder.Services.AddScoped<IContactoRepository, ContactoRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        // 3. Mapea la interfaz visual moderna de Scalar apuntando al JSON nativo
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("Mi API Moderna en .NET 10")
                .WithTheme(ScalarTheme.DeepSpace); // Elige tu tema preferido
        });
    });
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
