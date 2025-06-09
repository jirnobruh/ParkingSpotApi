using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkingSpotApi.Interfaces;
using ParkingSpotApi.Models;

namespace ParkingSpotApi.Controllers;

[ApiController, Authorize, Route("api/v1/[controller]")]
public class ParkingSpotsController : ControllerBase
{
    public IRepository<ParkingSpot> Repository { get; }
    
    public ParkingSpotsController(IRepository<ParkingSpot> repository)
    {
        Repository = repository;    
    }
    
    // Получить список всех парковочных мест (GET)
    // Доступ: анонимно
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetAll()
    {
        var spots = Repository.GetAll();
        return Ok(spots);
    }
    
    // Забронировать парковочное место (PATCH)
    // Доступ: только аутентифицированные пользователи
    [HttpPatch("{id}/reservation")]
    public async Task<IActionResult> Reserve(int id)
    {
        var spot = Repository.Find(id);
        
        if (spot.IsReserved)
            return BadRequest();
        
        spot.IsReserved = true;
        spot.ReservedBy = User.Identity.Name;
        spot.ReservedUntil = DateTime.UtcNow.AddHours(2);
        
        Repository.Update(spot);
        
        // Формируем сообщение для уведомления.
        var notifyPayload = new
        {
            EventId = spot.Id,
            Title = $"Парковочное место {spot.Id} забронировано пользователем {User.Identity.Name}.",
            EventDate = spot.ReservedUntil
        };

        // Отправляем уведомление через микросервис Telegram.
        using (var httpClient = new HttpClient())
        {
            var json = JsonSerializer.Serialize(notifyPayload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = 
                await httpClient.PostAsync("http://localhost:5229/api/notifications", content);
        }

        
        return Ok(spot);
    }
    
    // Освободить парковочное место (DELETE)
    // Доступ: только тот, кто бронировал, либо админ
    [HttpDelete("{id}/reservation")]
    public IActionResult Release(int id)
    {
        var spot = Repository.Find(id);
        
        var username = User.Identity?.Name;
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && !string.Equals(spot.ReservedBy, username, StringComparison.OrdinalIgnoreCase))
            return Forbid();

        spot.IsReserved = false;
        spot.ReservedBy = null;
        spot.ReservedUntil = null;

        Repository.Update(spot);

        return Ok(spot);
    }
    // Добавить новое место (POST)
    // Доступ: только для админов
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public IActionResult Add([FromBody] ParkingSpot spot)
    {
        Repository.Add(spot);
        return CreatedAtAction(nameof(GetAll), new { id = spot.Id }, spot);
    }
    // Удалить место (DELETE)
    // Доступ: только для админов
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var spot = Repository.Find(id);
        Repository.Remove(spot);
        return NoContent();
    }
}

/*
[ApiController]
[Route("api/v1/[controller]")]
public partial class AuthController : ControllerBase
{
    public InMemoryRepository<ParkingSpot> Repository { get; }

    public AuthController(InMemoryRepository<ParkingSpot> repository)
    {
        Repository = repository;    
    }
}*/