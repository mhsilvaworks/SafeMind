using Microsoft.EntityFrameworkCore;
using SafeMind.Infrastructure.Data;
using SafeMind.Application.Interfaces;
using SafeMind.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddControllers();

        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddOpenApi();
        
        // O "!" avisa o compilador que garantimos que a chave existe no appsettings
        var jwtKey = builder.Configuration["Jwt:Key"]!;
        var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
            };
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        // ===================================================================
        // PARTE 3: LIGAR A SEGURANÇA (A ordem é vital para a arquitetura!)
        // ===================================================================
        app.UseAuthentication(); // 1º: Descobre QUEM é o utilizador (Verifica o JWT)
        app.UseAuthorization();  // 2º: Verifica se ele PODE aceder à rota
        // ===================================================================

        app.MapControllers();

        app.Run();
    }
}