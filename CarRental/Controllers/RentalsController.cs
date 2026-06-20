using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Data;
using CarRental.Models.Domain;
using CarRental.Models.ViewModels;

namespace CarRental.Controllers;

[Authorize] // samo ulogovani korisnici
public class RentalsController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public RentalsController(AppDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /Rentals - moje rezervacije
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);

        if (customer == null) return NotFound();

        var rentals = await _context.Rentals
            .Include(r => r.Car).ThenInclude(c => c.Brand)
            .Where(r => r.CustomerId == customer.Id)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();

        return View(rentals);
    }

    // GET: /Rentals/Create
    public async Task<IActionResult> Create()
    {
        var viewModel = new RentalCreateViewModel
        {
            AvailableCars = await _context.Cars
                .Include(c => c.Brand)
                .Where(c => c.IsAvailable)
                .ToListAsync()
        };
        return View(viewModel);
    }

    // POST: /Rentals/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RentalCreateViewModel viewModel)
    {
        // Custom validacija - EndDate mora biti posle StartDate
        if (viewModel.EndDate <= viewModel.StartDate)
        {
            ModelState.AddModelError("EndDate", "Datum vraćanja mora biti posle datuma preuzimanja.");
        }

        if (!ModelState.IsValid)
        {
            viewModel.AvailableCars = await _context.Cars
                .Include(c => c.Brand)
                .Where(c => c.IsAvailable)
                .ToListAsync();
            return View(viewModel);
        }

        var car = await _context.Cars.FindAsync(viewModel.CarId);
        if (car == null) return NotFound();

        var userId = _userManager.GetUserId(User);
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
        if (customer == null) return NotFound();

        var days = (viewModel.EndDate - viewModel.StartDate).Days;

        var rental = new Rental
        {
            CarId = viewModel.CarId,
            CustomerId = customer.Id,
            StartDate = viewModel.StartDate,
            EndDate = viewModel.EndDate,
            TotalPrice = car.DailyPrice * days,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Rentals/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var rental = await _context.Rentals.Include(r => r.Car).FirstOrDefaultAsync(r => r.Id == id);
        if (rental == null) return NotFound();

        if (!await IsOwner(rental)) return Forbid();

        var viewModel = new RentalCreateViewModel
        {
            CarId = rental.CarId,
            StartDate = rental.StartDate,
            EndDate = rental.EndDate,
            AvailableCars = await _context.Cars.Include(c => c.Brand).ToListAsync()
        };

        ViewBag.RentalId = rental.Id;
        return View(viewModel);
    }

    // POST: /Rentals/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RentalCreateViewModel viewModel)
    {
        var rental = await _context.Rentals.Include(r => r.Car).FirstOrDefaultAsync(r => r.Id == id);
        if (rental == null) return NotFound();

        if (!await IsOwner(rental)) return Forbid();

        if (viewModel.EndDate <= viewModel.StartDate)
        {
            ModelState.AddModelError("EndDate", "Datum vraćanja mora biti posle datuma preuzimanja.");
        }

        if (!ModelState.IsValid)
        {
            viewModel.AvailableCars = await _context.Cars.Include(c => c.Brand).ToListAsync();
            ViewBag.RentalId = rental.Id;
            return View(viewModel);
        }

        var days = (viewModel.EndDate - viewModel.StartDate).Days;

        rental.StartDate = viewModel.StartDate;
        rental.EndDate = viewModel.EndDate;
        rental.TotalPrice = rental.Car.DailyPrice * days;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Rentals/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var rental = await _context.Rentals
            .Include(r => r.Car).ThenInclude(c => c.Brand)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rental == null) return NotFound();
        if (!await IsOwner(rental)) return Forbid();

        return View(rental);
    }

    // POST: /Rentals/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental == null) return NotFound();
        if (!await IsOwner(rental)) return Forbid();

        _context.Rentals.Remove(rental);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> IsOwner(Rental rental)
    {
        var userId = _userManager.GetUserId(User);
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
        return customer != null && rental.CustomerId == customer.Id;
    }
}