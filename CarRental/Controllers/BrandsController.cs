using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Data;

namespace CarRental.Controllers;

public class BrandsController : Controller
{
    private readonly AppDbContext _context;

    public BrandsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var brands = await _context.Brands
            .Include(b => b.Cars)
            .ToListAsync();
        return View(brands);
    }

    public async Task<IActionResult> Details(int id)
    {
        var brand = await _context.Brands
            .Include(b => b.Cars)
            .ThenInclude(c => c.Location)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (brand == null) return NotFound();
        return View(brand);
    }
}