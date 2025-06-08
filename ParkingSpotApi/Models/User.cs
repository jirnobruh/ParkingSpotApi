using ParkingSpotApi.Interfaces;

namespace ParkingSpotApi.Models;

public class User : IEntity
{
    public int Id { get; set; }
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string Role { get; init; } = "User";
}