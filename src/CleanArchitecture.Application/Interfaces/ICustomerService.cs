using CleanArchitecture.Application.DTOs;

namespace CleanArchitecture.Application.Interfaces;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerResponse>> ListAsync(CancellationToken cancellationToken);
    Task<CustomerResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<CustomerResponse> CreateAsync(CustomerRequest request, CancellationToken cancellationToken);
    Task<CustomerResponse?> UpdateAsync(Guid id, CustomerRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
