using CleanArchitecture.Application.DTOs;

namespace CleanArchitecture.Application.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<OrderResponse>> ListAsync(CancellationToken cancellationToken);
    Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<OrderResponse> CreateAsync(OrderRequest request, CancellationToken cancellationToken);
}
