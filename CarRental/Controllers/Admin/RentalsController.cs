using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Data;
using CarRental.Models.ViewModels;

namespace CarRental.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/Rentals")]
public class RentalsController : Controller
{
    private readonly AppDbContext _context;

    public RentalsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? status, string? city, string? sortBy, int page = 1)
    {
        int pageSize = 8;

        var query = _context.Rentals
            .Include(r => r.Car).ThenInclude(c => c.Brand)
            .Include(r => r.Car).ThenInclude(c => c.Location)
            .Include(r => r.Customer)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.Status == status);

        if (!string.IsNullOrEmpty(city))
            query = query.Where(r => r.Car.Location.City == city);

        query = sortBy switch
        {
            "price_asc" => query.OrderBy(r => r.TotalPrice),
            "price_desc" => query.OrderByDescending(r => r.TotalPrice),
            "date_asc" => query.OrderBy(r => r.StartDate),
            _ => query.OrderByDescending(r => r.StartDate)
        };

        var totalCount = await query.CountAsync();

        var rentals = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var viewModel = new RentalAdminViewModel
        {
            Rentals = rentals,
            StatusFilter = status,
            CityFilter = city,
            SortBy = sortBy,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            PageSize = pageSize,
            AvailableStatuses = new List<string> { "Pending", "Active", "Completed", "Cancelled" },
            AvailableCities = await _context.Locations.Select(l => l.City).Distinct().ToListAsync()
        };

        return View("~/Views/Admin/Rentals/Index.cshtml", viewModel);
    }

    [HttpPost("UpdateStatus/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string newStatus, string? status, string? city, string? sortBy, int page = 1)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental == null) return NotFound();

        rental.Status = newStatus;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { status, city, sortBy, page });
    }

    [HttpGet("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var rental = await _context.Rentals
            .Include(r => r.Car).ThenInclude(c => c.Brand)
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rental == null) return NotFound();
        return View("~/Views/Admin/Rentals/Delete.cshtml", rental);
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental == null) return NotFound();

        _context.Rentals.Remove(rental);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
