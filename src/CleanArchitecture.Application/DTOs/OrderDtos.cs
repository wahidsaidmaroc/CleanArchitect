namespace CleanArchitecture.Application.DTOs;

public sealed record OrderItemRequest(Guid ProductId, int Quantity);

public sealed record OrderRequest(Guid CustomerId, AddressDto ShippingAddress, IReadOnlyCollection<OrderItemRequest> Items);

public sealed record OrderItemResponse(Guid ProductId, int Quantity, decimal UnitPrice, string Currency);

public sealed record OrderResponse(
    Guid Id,
    Guid CustomerId,
    AddressDto ShippingAddress,
    string Status,
    IReadOnlyCollection<OrderItemResponse> Items,
    decimal Total,
    string Currency);
