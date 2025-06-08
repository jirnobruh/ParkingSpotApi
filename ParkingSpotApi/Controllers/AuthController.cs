using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ParkingSpotApi.Interfaces;
using ParkingSpotApi.Models;

namespace ParkingSpotApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IRepository<User> _userRepo;
    private readonly ITokenService _tokenService;

    public AuthController(IRepository<User> userRepo, ITokenService tokenService)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
    }
    
    // Регистрация нового пользователя (POST /api/v1/auth/register)
    [HttpPost("register")]
    public IActionResult Register([FromBody] User user)
    {
        // Проверяем, существует ли уже пользователь с указанным именем.
        var existingUser = _userRepo.GetAll()
            .FirstOrDefault(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase));
        if (existingUser != null)
            return BadRequest("User already exists.");

        _userRepo.Add(user);

        return CreatedAtAction(nameof(Register), new { username = user.Username }, user);
    }
    
    // Аунтентификация пользователя и выдача JWT (POST /api/v1/auth/login)
    [HttpPost("login")]
    public IActionResult Login([FromBody] User login)
    {
        // Поиск пользователя по имени
        var user = _userRepo.GetAll()
            .FirstOrDefault(u => u.Username.Equals(login.Username, StringComparison.OrdinalIgnoreCase));

        // В реальном мире необходимо использовать надежный метод проверки хеша пароля.
        if (user == null || user.Password != login.Password)
            return Unauthorized("Invalid credentials.");

        // Генерация JWT токена через наш сервис
        var token = _tokenService.GenerateToken(user);

        return Ok(new { token });
    }
    
    // Выход (logout). Серверная логика не требуется
    public IActionResult Logout()
    {
        // При использовании JWT сервер не хранит состояние сессии, поэтому "выход" сводится к 
        // тому, что клиент удаляет токен.
        return Ok("Logout successful.");
    }
}