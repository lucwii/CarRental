namespace CarRental.Models.Domain;

public class Rental
{
    public int Id { get; set; }

    public int CarId { get; set; }
    public Car Car { get; set; } = null!;

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    // Dodatna polja
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Active, Completed, Cancelled
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}