using ParkingSpotApi.Interfaces;

namespace ParkingSpotApi.Models;

public class ParkingSpot : IEntity
{
    public int Id { get; set; }
    public string Location { get; set; } = null!;
    public bool IsReserved { get; set; }
    public string? ReservedBy { get; set; }
    public DateTime? ReservedUntil { get; set; }
}