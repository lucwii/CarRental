using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental.Data;
using CarRental.Models.Domain;

namespace CarRental.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/Brands")]
public class BrandsController : Controller
{
    private readonly AppDbContext _context;

    public BrandsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var brands = await _context.Brands.Include(b => b.Cars).ToListAsync();
        return View("~/Views/Admin/Brands/Index.cshtml", brands);
    }

    [HttpGet("Create")]
    public IActionResult Create() => View("~/Views/Admin/Brands/Create.cshtml", new Brand());

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Brand brand)
    {
        ModelState.Remove("Cars");
        ModelState.Remove("LogoUrl");

        if (!ModelState.IsValid) return View("~/Views/Admin/Brands/Create.cshtml", brand);

        brand.LogoUrl ??= string.Empty;

        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null) return NotFound();
        return View("~/Views/Admin/Brands/Edit.cshtml", brand);
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Brand brand)
    {
        if (id != brand.Id) return NotFound();

        ModelState.Remove("Cars");
        ModelState.Remove("LogoUrl");

        if (!ModelState.IsValid) return View("~/Views/Admin/Brands/Edit.cshtml", brand);

        brand.LogoUrl ??= string.Empty;

        _context.Update(brand);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var brand = await _context.Brands.Include(b => b.Cars).FirstOrDefaultAsync(b => b.Id == id);
        if (brand == null) return NotFound();
        return View("~/Views/Admin/Brands/Delete.cshtml", brand);
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null) return NotFound();

        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}