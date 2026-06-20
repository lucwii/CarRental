using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Data;
using CarRental.Models.Domain;

namespace CarRental.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/Locations")]
public class LocationsController : Controller
{
    private readonly AppDbContext _context;

    public LocationsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var locations = await _context.Locations.Include(l => l.Cars).ToListAsync();
        return View("~/Views/Admin/Locations/Index.cshtml", locations);
    }

    [HttpGet("Create")]
    public IActionResult Create() => View("~/Views/Admin/Locations/Create.cshtml", new Location());

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Location location)
    {
        ModelState.Remove("Cars");

        if (!ModelState.IsValid) return View("~/Views/Admin/Locations/Create.cshtml", location);

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null) return NotFound();
        return View("~/Views/Admin/Locations/Edit.cshtml", location);
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Location location)
    {
        if (id != location.Id) return NotFound();

        ModelState.Remove("Cars");

        if (!ModelState.IsValid) return View("~/Views/Admin/Locations/Edit.cshtml", location);

        _context.Update(location);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var location = await _context.Locations.Include(l => l.Cars).FirstOrDefaultAsync(l => l.Id == id);
        if (location == null) return NotFound();
        return View("~/Views/Admin/Locations/Delete.cshtml", location);
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null) return NotFound();

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}