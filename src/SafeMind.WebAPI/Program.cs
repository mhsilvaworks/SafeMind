using Microsoft.EntityFrameworkCore;
using SafeMind.Infrastructure.Data;
using SafeMind.Application.Interfaces;
using SafeMind.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// 1. Banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

// 2. Injeção de dependência
builder.Services.AddScoped<IAuthService, AuthService>();

// 3. OpenAPI nativo do .NET 10 
builder.Services.AddOpenApi();

// 4. JWT
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

// 5. Swagger UI via Scalar (moderno para .NET 10)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();             
    app.MapScalarApiReference();   
}

app.UseMiddleware<SafeMind.WebAPI.Middlewares.GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();