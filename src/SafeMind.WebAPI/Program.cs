using Microsoft.EntityFrameworkCore;
using SafeMind.Infrastructure.Data;
// 1. ADICIONADOS: Os namespaces da nossa Aplicação
using SafeMind.Application.Interfaces;
using SafeMind.Application.Services;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Registrando o AppDbContext com PostgreSQL
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        // 2. ADICIONADO: Ensinar o .NET a ler os nossos Controllers (como o AuthController)
        builder.Services.AddControllers();

        // 3. ADICIONADO: Injeção de Dependência do nosso serviço SOLID
        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        // 4. ADICIONADO: Ativar as rotas dos Controllers na internet
        app.MapControllers();

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        // Rota de exemplo que já veio com o .NET (pode apagar isso no futuro quando não precisar mais)
        app.MapGet("/weatherforecast", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast");

        app.Run();
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}