using ParkingSpotApi.Models;

namespace ParkingSpotApi.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}