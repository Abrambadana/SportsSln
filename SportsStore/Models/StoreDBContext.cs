using Microsoft.EntityFrameworkCore;
using SportsStore.Models;

using Microsoft.EntityFrameworkCore;
using SportsStore.Models;

public class StoreDBContext : DbContext
{
    public StoreDBContext(DbContextOptions<StoreDBContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<CartLine> CartLines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the Order-CartLine relationship
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Lines)
            .WithOne(l => l.Order)
            .HasForeignKey(l => l.OrderID)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the CartLine-Product relationship
        modelBuilder.Entity<CartLine>()
            .HasOne(l => l.Product)
            .WithMany()
            .HasForeignKey(l => l.ProductId)
            .OnDelete(DeleteBehavior.Restrict); // Use Restrict instead of Cascade

        // Ensure Product.ProductId is properly configured
        modelBuilder.Entity<Product>()
            .Property(p => p.ProductId)
            .ValueGeneratedOnAdd();

        base.OnModelCreating(modelBuilder);
    }
}