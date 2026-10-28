using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(256).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
            entity.OwnsOne(x => x.Address, address => ConfigureAddress(address));
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(2000);
            entity.HasOne<Category>()
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.OwnsOne(x => x.Price, price => ConfigureMoney(price, "Price"));
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(x => x.Status).HasConversion<string>();
            entity.HasOne<Customer>()
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.OwnsOne(x => x.ShippingAddress, address => ConfigureAddress(address));
            entity.OwnsMany(x => x.Items, items =>
            {
                items.WithOwner().HasForeignKey("OrderId");
                items.HasKey(x => x.Id);
                items.Property(x => x.Id).ValueGeneratedNever();
                items.HasOne<Product>()
                    .WithMany()
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
                items.OwnsOne(x => x.UnitPrice, price => ConfigureMoney(price, "UnitPrice"));
            });
        });
    }

    private static void ConfigureAddress<TOwner>(OwnedNavigationBuilder<TOwner, Domain.ValueObjects.Address> address)
        where TOwner : class
    {
        address.Property(x => x.Street).HasMaxLength(200).IsRequired();
        address.Property(x => x.City).HasMaxLength(100).IsRequired();
        address.Property(x => x.PostalCode).HasMaxLength(20).IsRequired();
        address.Property(x => x.Country).HasMaxLength(100).IsRequired();
    }

    private static void ConfigureMoney<TOwner>(OwnedNavigationBuilder<TOwner, Domain.ValueObjects.Money> money, string prefix)
        where TOwner : class
    {
        money.Property(x => x.Amount).HasColumnName($"{prefix}Amount").HasPrecision(18, 2).IsRequired();
        money.Property(x => x.Currency).HasColumnName($"{prefix}Currency").HasMaxLength(3).IsRequired();
    }
}
