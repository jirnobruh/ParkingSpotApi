using ParkingSpotApi.Interfaces;

namespace ParkingSpotApi.Models;

public class AppSeeder
{
    public static void SeedUsers(IRepository<User> userRepo)
    {
        if (userRepo.GetAll().Any())
            return;
        userRepo.Add(new User { Username = "admin", Password = "admin", Role = "Admin" });
        userRepo.Add(new User { Username = "alex", Password = "password", Role = "User" });
        userRepo.Add(new User { Username = "olga", Password = "password", Role = "User" });
    }
    public static void SeedParkingSpots(IRepository<ParkingSpot> spotRepo)
    {
        if (spotRepo.GetAll().Any())
            return;
        spotRepo.Add(new ParkingSpot { Location = "A1", IsReserved = false });
        spotRepo.Add(new ParkingSpot { Location = "A2", IsReserved = false });
        spotRepo.Add(new ParkingSpot { Location = "B1", IsReserved = false });
        spotRepo.Add(new ParkingSpot { Location = "B2", IsReserved = false });
        spotRepo.Add(new ParkingSpot {
            Location = "VIP-1",
            IsReserved = true,
            ReservedBy = "alex",
            ReservedUntil = DateTime.UtcNow.AddHours(2)
        });
    }
    public static void SeedAll(IRepository<User> userRepo, IRepository<ParkingSpot> spotRepo)
    {
        SeedUsers(userRepo);
        SeedParkingSpots(spotRepo);
    }
}