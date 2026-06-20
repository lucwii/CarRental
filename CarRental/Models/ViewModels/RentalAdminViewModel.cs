using CarRental.Models.Domain;

namespace CarRental.Models.ViewModels;

public class RentalAdminViewModel
{
    public List<Rental> Rentals { get; set; } = new();

    public string? StatusFilter { get; set; }
    public string? CityFilter { get; set; }
    public string? SortBy { get; set; }

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 8;

    public List<string> AvailableStatuses { get; set; } = new();
    public List<string> AvailableCities { get; set; } = new();
}