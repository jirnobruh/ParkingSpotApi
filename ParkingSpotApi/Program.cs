using System.Text;
using Microsoft.IdentityModel.Tokens;
using ParkingSpotApi.Interfaces;
using ParkingSpotApi.Models;

namespace ParkingSpotApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        
        var jwtKey = "mysupersecretkeymysupersecretkey";
        var jwtIssuer = "parkingapi";
        
        // Регистрируем репозитории для пользователей и парковочных мест.
        builder.Services.AddSingleton<IRepository<User>, InMemoryRepository<User>>();
        builder.Services.AddSingleton<IRepository<ParkingSpot>, InMemoryRepository<ParkingSpot>>();
        builder.Services.AddSingleton<ITokenService>(new TokenService(jwtKey, jwtIssuer));
        
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });
        
        // Настраиваем политики авторизации.
        // Это позволяет в дальнейшем защищать отдельные эндпоинты
        // атрибутом `[Authorize(Policy = "AdminOnly")]`.
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        });

        var app = builder.Build();
        
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        
        var userRepo = app.Services.GetRequiredService<IRepository<User>>();
        var spotRepo = app.Services.GetRequiredService<IRepository<ParkingSpot>>();
        AppSeeder.SeedAll(userRepo, spotRepo);
        
        app.Run();
    }
}