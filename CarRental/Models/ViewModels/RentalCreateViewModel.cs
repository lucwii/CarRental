using System.ComponentModel.DataAnnotations;

namespace CarRental.Models.ViewModels;

public class RentalCreateViewModel
{
    [Required]
    public int CarId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Datum preuzimanja")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Datum vraćanja")]
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(1);

    public List<CarRental.Models.Domain.Car>? AvailableCars { get; set; }
}