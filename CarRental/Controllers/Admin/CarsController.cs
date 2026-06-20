using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Data;
using CarRental.Models.Domain;

namespace CarRental.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/Cars")]
public class CarsController : Controller
{
    private readonly AppDbContext _context;

    public CarsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Admin/Cars
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var cars = await _context.Cars
            .Include(c => c.Brand)
            .Include(c => c.Location)
            .ToListAsync();
        return View("~/Views/Admin/Cars/Index.cshtml", cars);
    }

    // GET: /Admin/Cars/Create
    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View("~/Views/Admin/Cars/Create.cshtml", new Car());
    }

    // POST: /Admin/Cars/Create
    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Car car)
    {
        ModelState.Remove("Brand");
        ModelState.Remove("Location");
        ModelState.Remove("Rentals");
        ModelState.Remove("ImageUrl");

        if (!ModelState.IsValid)
        {
            await PopulateDropdowns();
            return View("~/Views/Admin/Cars/Create.cshtml", car);
        }

        _context.Cars.Add(car);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Admin/Cars/Edit/5
    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null) return NotFound();

        await PopulateDropdowns();
        return View("~/Views/Admin/Cars/Edit.cshtml", car);
    }

    // POST: /Admin/Cars/Edit/5
    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Car car)
    {
        if (id != car.Id) return NotFound();

        ModelState.Remove("Brand");
        ModelState.Remove("Location");
        ModelState.Remove("Rentals");
        ModelState.Remove("ImageUrl");

        if (!ModelState.IsValid)
        {
            await PopulateDropdowns();
            return View("~/Views/Admin/Cars/Edit.cshtml", car);
        }

        _context.Update(car);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Admin/Cars/Delete/5
    [HttpGet("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var car = await _context.Cars
            .Include(c => c.Brand)
            .Include(c => c.Location)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (car == null) return NotFound();
        return View("~/Views/Admin/Cars/Delete.cshtml", car);
    }

    // POST: /Admin/Cars/Delete/5
    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null) return NotFound();

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdowns()
    {
        ViewBag.Brands = await _context.Brands.ToListAsync();
        ViewBag.Locations = await _context.Locations.ToListAsync();
    }
}