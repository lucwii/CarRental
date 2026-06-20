using System.ComponentModel.DataAnnotations;

namespace CarRental.Models.Domain;

public class Location
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Grad je obavezan.")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Adresa je obavezna.")]
    public string Address { get; set; } = string.Empty;

    public ICollection<Car> Cars { get; set; } = new List<Car>();
}