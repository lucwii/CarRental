using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CarRental.Models.Domain;

namespace CarRental.Data;

public class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await SeedRolesAsync(roleManager);
        await SeedAdminAsync(userManager);

        if (!await context.Brands.AnyAsync())
            await SeedBrandsAsync(context);

        if (!await context.Locations.AnyAsync())
            await SeedLocationsAsync(context);

        if (!await context.Cars.AnyAsync())
            await SeedCarsAsync(context);

        if (!await context.Customers.AnyAsync())
            await SeedCustomersAsync(context, userManager);

        if (!await context.Rentals.AnyAsync())
            await SeedRentalsAsync(context);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task SeedAdminAsync(UserManager<IdentityUser> userManager)
    {
        if (await userManager.FindByEmailAsync("admin@carrental.com") == null)
        {
            var admin = new IdentityUser
            {
                UserName = "admin@carrental.com",
                Email = "admin@carrental.com",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }

    private static async Task SeedBrandsAsync(AppDbContext context)
    {
        var brands = new List<Brand>
        {
            new() { Name = "BMW", Country = "Germany", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/4/44/BMW.svg" },
            new() { Name = "Audi", Country = "Germany", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/9/92/Audi-Logo_2016.svg" },
            new() { Name = "Toyota", Country = "Japan", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/9/9d/Toyota_carlogo.svg" },
            new() { Name = "Mercedes-Benz", Country = "Germany", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/9/90/Mercedes-Logo.svg" },
            new() { Name = "Volkswagen", Country = "Germany", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/6/6c/Volkswagen_logo_2019.svg" },
            new() { Name = "Tesla", Country = "USA", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/b/bb/Tesla_T_symbol.svg" },
        };

        await context.Brands.AddRangeAsync(brands);
        await context.SaveChangesAsync();
    }

    private static async Task SeedLocationsAsync(AppDbContext context)
    {
        var locations = new List<Location>
        {
            new() { City = "Beograd", Address = "Aerodrom Nikola Tesla" },
            new() { City = "Novi Sad", Address = "Bulevar Oslobođenja 12" },
            new() { City = "Niš", Address = "Trg Kralja Milana 5" },
            new() { City = "Beograd", Address = "Centar, Knez Mihailova" },
        };

        await context.Locations.AddRangeAsync(locations);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCarsAsync(AppDbContext context)
    {
        var brands = await context.Brands.ToListAsync();
        var locations = await context.Locations.ToListAsync();

        var bmw = brands.First(b => b.Name == "BMW");
        var audi = brands.First(b => b.Name == "Audi");
        var toyota = brands.First(b => b.Name == "Toyota");
        var mercedes = brands.First(b => b.Name == "Mercedes-Benz");
        var vw = brands.First(b => b.Name == "Volkswagen");
        var tesla = brands.First(b => b.Name == "Tesla");

        var cars = new List<Car>
        {
            new() { Model = "320i", Year = 2022, LicensePlate = "BG-001-AA", DailyPrice = 45, FuelType = "Benzin", Seats = 5, BrandId = bmw.Id, LocationId = locations[0].Id, IsAvailable = true },
            new() { Model = "X5", Year = 2023, LicensePlate = "BG-002-AA", DailyPrice = 80, FuelType = "Dizel", Seats = 5, BrandId = bmw.Id, LocationId = locations[0].Id, IsAvailable = true },
            new() { Model = "A4", Year = 2021, LicensePlate = "NS-001-AA", DailyPrice = 42, FuelType = "Dizel", Seats = 5, BrandId = audi.Id, LocationId = locations[1].Id, IsAvailable = true },
            new() { Model = "Q7", Year = 2023, LicensePlate = "NS-002-AA", DailyPrice = 90, FuelType = "Dizel", Seats = 7, BrandId = audi.Id, LocationId = locations[1].Id, IsAvailable = true },
            new() { Model = "Corolla", Year = 2022, LicensePlate = "NI-001-AA", DailyPrice = 30, FuelType = "Hibrid", Seats = 5, BrandId = toyota.Id, LocationId = locations[2].Id, IsAvailable = true },
            new() { Model = "RAV4", Year = 2023, LicensePlate = "NI-002-AA", DailyPrice = 55, FuelType = "Hibrid", Seats = 5, BrandId = toyota.Id, LocationId = locations[2].Id, IsAvailable = true },
            new() { Model = "C-Class", Year = 2022, LicensePlate = "BG-003-AA", DailyPrice = 60, FuelType = "Benzin", Seats = 5, BrandId = mercedes.Id, LocationId = locations[3].Id, IsAvailable = true },
            new() { Model = "E-Class", Year = 2023, LicensePlate = "BG-004-AA", DailyPrice = 75, FuelType = "Dizel", Seats = 5, BrandId = mercedes.Id, LocationId = locations[3].Id, IsAvailable = true },
            new() { Model = "Golf", Year = 2021, LicensePlate = "NS-003-AA", DailyPrice = 28, FuelType = "Benzin", Seats = 5, BrandId = vw.Id, LocationId = locations[1].Id, IsAvailable = true },
            new() { Model = "Tiguan", Year = 2022, LicensePlate = "NS-004-AA", DailyPrice = 50, FuelType = "Dizel", Seats = 5, BrandId = vw.Id, LocationId = locations[1].Id, IsAvailable = true },
            new() { Model = "Model 3", Year = 2023, LicensePlate = "BG-005-AA", DailyPrice = 70, FuelType = "Električni", Seats = 5, BrandId = tesla.Id, LocationId = locations[0].Id, IsAvailable = true },
            new() { Model = "Model Y", Year = 2023, LicensePlate = "BG-006-AA", DailyPrice = 85, FuelType = "Električni", Seats = 5, BrandId = tesla.Id, LocationId = locations[0].Id, IsAvailable = true },
            new() { Model = "118i", Year = 2020, LicensePlate = "NI-003-AA", DailyPrice = 35, FuelType = "Benzin", Seats = 5, BrandId = bmw.Id, LocationId = locations[2].Id, IsAvailable = true },
            new() { Model = "A3", Year = 2020, LicensePlate = "BG-007-AA", DailyPrice = 32, FuelType = "Benzin", Seats = 5, BrandId = audi.Id, LocationId = locations[3].Id, IsAvailable = true },
            new() { Model = "Yaris", Year = 2021, LicensePlate = "NS-005-AA", DailyPrice = 25, FuelType = "Benzin", Seats = 5, BrandId = toyota.Id, LocationId = locations[1].Id, IsAvailable = true },
        };

        await context.Cars.AddRangeAsync(cars);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCustomersAsync(AppDbContext context, UserManager<IdentityUser> userManager)
    {
        // Test korisnik (User rola)
        if (await userManager.FindByEmailAsync("marko@test.com") == null)
        {
            var user = new IdentityUser
            {
                UserName = "marko@test.com",
                Email = "marko@test.com",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user, "Test123!");
            await userManager.AddToRoleAsync(user, "User");

            context.Customers.Add(new Customer
            {
                FullName = "Marko Marković",
                PhoneNumber = "0641234567",
                DrivingLicenseNumber = "DL123456",
                UserId = user.Id
            });

            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedRentalsAsync(AppDbContext context)
    {
        var cars = await context.Cars.ToListAsync();
        var customers = await context.Customers.ToListAsync();

        if (customers.Count == 0 || cars.Count == 0) return;

        var customer = customers.First();
        var random = new Random(42);
        var statuses = new[] { "Pending", "Active", "Completed", "Cancelled" };

        var rentals = new List<Rental>();

        for (int i = 0; i < 12; i++)
        {
            var car = cars[i % cars.Count];
            var startDate = DateTime.UtcNow.AddDays(-30 + i * 3);
            var days = random.Next(2, 8);
            var endDate = startDate.AddDays(days);

            rentals.Add(new Rental
            {
                CarId = car.Id,
                CustomerId = customer.Id,
                StartDate = startDate,
                EndDate = endDate,
                TotalPrice = car.DailyPrice * days,
                Status = statuses[i % statuses.Length],
                CreatedAt = startDate.AddDays(-1)
            });
        }

        await context.Rentals.AddRangeAsync(rentals);
        await context.SaveChangesAsync();
    }
}