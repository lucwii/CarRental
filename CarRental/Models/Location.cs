namespace CarRental.Models.Domain;

public class Location
{
    public int Id { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public ICollection<Car> Cars { get; set; } = new List<Car>();
}