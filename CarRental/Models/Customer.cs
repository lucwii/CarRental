namespace CarRental.Models.Domain;

public class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string DrivingLicenseNumber { get; set; } = string.Empty;

    // Povezan sa Identity nalogom
    public string UserId { get; set; } = string.Empty;

    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}