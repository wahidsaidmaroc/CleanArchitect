using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Domain.Entities;

public class Order : BaseEntity
{
    private Order()
    {
    }

    public Order(Guid customerId, Address shippingAddress)
    {
        if (customerId == Guid.Empty)
        {
            throw new DomainException("A customer is required for an order.");
        }

        CustomerId = customerId;
        ShippingAddress = shippingAddress ?? throw new DomainException("Shipping address is required.");
        Status = OrderStatus.Pending;
    }

    public Guid CustomerId { get; private set; }
    public Address ShippingAddress { get; private set; } = null!;
    public OrderStatus Status { get; private set; }
    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

    public void AddItem(Product product, int quantity)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (Items.Count > 0 && Items.First().UnitPrice.Currency != product.Price.Currency)
        {
            throw new DomainException("All products in an order must use the same currency.");
        }

        product.RemoveStock(quantity);
        Items.Add(new OrderItem(product.Id, quantity, product.Price));
    }

    public Money GetTotal()
    {
        if (Items.Count == 0)
        {
            throw new DomainException("An order must contain at least one item.");
        }

        return Items.Skip(1).Aggregate(Items.First().GetTotal(), (total, item) => total.Add(item.GetTotal()));
    }
}
