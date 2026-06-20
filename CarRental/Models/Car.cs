namespace CarRental.Models.Domain;

public class Car
{
    public int Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public decimal DailyPrice { get; set; }
    public string FuelType { get; set; } = string.Empty;
    public int Seats { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;

    // One-to-Many: Car pripada jednom Brand-u
    public int BrandId { get; set; }
    public Brand Brand { get; set; } = null!;

    // One-to-Many: Car se nalazi na jednoj Location-i
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    // Navigation
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}