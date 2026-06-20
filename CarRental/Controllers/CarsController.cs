using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Data;

namespace CarRental.Controllers;

public class CarsController : Controller
{
    private readonly AppDbContext _context;

    public CarsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Cars
    public async Task<IActionResult> Index()
    {
        var cars = await _context.Cars
            .Include(c => c.Brand)
            .Include(c => c.Location)
            .ToListAsync();
        return View(cars);
    }

    // GET: /Cars/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var car = await _context.Cars
            .Include(c => c.Brand)
            .Include(c => c.Location)
            .Include(c => c.Rentals)
            .ThenInclude(r => r.Customer)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (car == null) return NotFound();
        return View(car);
    }
}