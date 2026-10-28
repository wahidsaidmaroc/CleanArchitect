using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Domain.Entities;

public class Product : BaseEntity
{
    private Product()
    {
    }

    public Product(string name, string description, Money price, int stockQuantity, Guid categoryId)
    {
        Update(name, description, price, stockQuantity, categoryId);
    }

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Money Price { get; private set; } = null!;
    public int StockQuantity { get; private set; }
    public Guid CategoryId { get; private set; }

    public void Update(string name, string description, Money price, int stockQuantity, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Product name is required.");
        }

        if (stockQuantity < 0)
        {
            throw new DomainException("Stock quantity cannot be negative.");
        }

        if (categoryId == Guid.Empty)
        {
            throw new DomainException("A category is required.");
        }

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Price = price ?? throw new DomainException("Product price is required.");
        StockQuantity = stockQuantity;
        CategoryId = categoryId;
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0 || quantity > StockQuantity)
        {
            throw new DomainException("Requested quantity is not available in stock.");
        }

        StockQuantity -= quantity;
    }
}
