using System.ComponentModel.DataAnnotations;

namespace CarRental.Models.Domain;

public class Brand
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Naziv brenda je obavezan.")]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Zemlja porekla")]
    public string Country { get; set; } = string.Empty;

    [Display(Name = "URL logoa")]
    public string LogoUrl { get; set; } = string.Empty;

    public ICollection<Car> Cars { get; set; } = new List<Car>();
}