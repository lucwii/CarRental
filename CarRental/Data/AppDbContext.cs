using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarRental.Models.Domain;

namespace CarRental.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Brand> Brands { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Rental> Rentals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Car>()
            .HasOne(c => c.Brand)
            .WithMany(b => b.Cars)
            .HasForeignKey(c => c.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Car>()
            .HasOne(c => c.Location)
            .WithMany(l => l.Cars)
            .HasForeignKey(c => c.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Car)
            .WithMany(c => c.Rentals)
            .HasForeignKey(r => r.CarId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Customer)
            .WithMany(c => c.Rentals)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Decimal precision za MySQL
        modelBuilder.Entity<Car>()
            .Property(c => c.DailyPrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Rental>()
            .Property(r => r.TotalPrice)
            .HasPrecision(10, 2);
    }
}