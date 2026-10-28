using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Domain.Entities;

public class OrderItem
{
    private OrderItem()
    {
    }

    internal OrderItem(Guid productId, int quantity, Money unitPrice)
    {
        if (productId == Guid.Empty)
        {
            throw new DomainException("A product is required for an order item.");
        }

        if (quantity <= 0)
        {
            throw new DomainException("Order item quantity must be greater than zero.");
        }

        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice ?? throw new DomainException("Order item price is required.");
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;

    public Money GetTotal() => UnitPrice.Multiply(Quantity);
}
