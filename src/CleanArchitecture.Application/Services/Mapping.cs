using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Application.Services;

internal static class Mapping
{
    internal static Address ToAddress(AddressDto address)
    {
        if (address is null)
        {
            throw new DomainException("Address is required.");
        }

        return new Address(address.Street, address.City, address.PostalCode, address.Country);
    }

    internal static AddressDto ToDto(Address address) =>
        new(address.Street, address.City, address.PostalCode, address.Country);

    internal static CustomerResponse ToDto(Customer customer) =>
        new(customer.Id, customer.FirstName, customer.LastName, customer.Email, ToDto(customer.Address));

    internal static CategoryResponse ToDto(Category category) =>
        new(category.Id, category.Name);

    internal static ProductResponse ToDto(Product product) =>
        new(product.Id, product.Name, product.Description, product.Price.Amount,
            product.Price.Currency, product.StockQuantity, product.CategoryId);

    internal static OrderResponse ToDto(Order order)
    {
        var total = order.GetTotal();
        return new OrderResponse(
            order.Id,
            order.CustomerId,
            ToDto(order.ShippingAddress),
            order.Status.ToString(),
            order.Items.Select(item => new OrderItemResponse(
                item.ProductId,
                item.Quantity,
                item.UnitPrice.Amount,
                item.UnitPrice.Currency)).ToArray(),
            total.Amount,
            total.Currency);
    }
}
