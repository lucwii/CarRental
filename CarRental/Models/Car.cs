using System.ComponentModel.DataAnnotations;

namespace CarRental.Models.Domain;

public class Car
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Model je obavezan.")]
    [StringLength(50)]
    public string Model { get; set; } = string.Empty;

    [Required]
    [Range(1990, 2027, ErrorMessage = "Godina mora biti između 1990 i 2027.")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Registarska oznaka je obavezna.")]
    [StringLength(20)]
    [Display(Name = "Registarska oznaka")]
    public string LicensePlate { get; set; } = string.Empty;

    [Required]
    [Range(1, 10000, ErrorMessage = "Cena mora biti veća od 0.")]
    [Display(Name = "Cena po danu (€)")]
    public decimal DailyPrice { get; set; }

    [Required]
    [Display(Name = "Tip goriva")]
    public string FuelType { get; set; } = string.Empty;

    [Required]
    [Range(1, 9)]
    [Display(Name = "Broj sedišta")]
    public int Seats { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    [Display(Name = "Dostupan")]
    public bool IsAvailable { get; set; } = true;

    [Display(Name = "Brend")]
    public int BrandId { get; set; }
    public Brand Brand { get; set; } = null!;

    [Display(Name = "Lokacija")]
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}