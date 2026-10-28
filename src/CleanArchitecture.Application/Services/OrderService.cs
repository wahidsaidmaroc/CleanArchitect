using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Exceptions;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Services;

public sealed class OrderService(
    IRepository<Order> orders,
    IRepository<Customer> customers,
    IRepository<Product> products,
    IUnitOfWork unitOfWork) : IOrderService
{
    public async Task<IReadOnlyList<OrderResponse>> ListAsync(CancellationToken cancellationToken) =>
        (await orders.ListAsync(cancellationToken)).Select(Mapping.ToDto).ToArray();

    public async Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await orders.GetByIdAsync(id, cancellationToken);
        return order is null ? null : Mapping.ToDto(order);
    }

    public async Task<OrderResponse> CreateAsync(OrderRequest request, CancellationToken cancellationToken)
    {
        if (await customers.GetByIdAsync(request.CustomerId, cancellationToken) is null)
        {
            throw new NotFoundException($"Customer '{request.CustomerId}' was not found.");
        }

        if (request.Items is null || request.Items.Count == 0)
        {
            throw new Domain.Exceptions.DomainException("An order must contain at least one item.");
        }

        var order = new Order(request.CustomerId, Mapping.ToAddress(request.ShippingAddress));
        foreach (var item in request.Items)
        {
            var product = await products.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is null)
            {
                throw new NotFoundException($"Product '{item.ProductId}' was not found.");
            }

            order.AddItem(product, item.Quantity);
        }

        orders.Add(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Mapping.ToDto(order);
    }
}
