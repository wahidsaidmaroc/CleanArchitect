using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Exceptions;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Services;

public sealed class CustomerService(
    IRepository<Customer> customers,
    IUnitOfWork unitOfWork) : ICustomerService
{
    public async Task<IReadOnlyList<CustomerResponse>> ListAsync(CancellationToken cancellationToken) =>
        (await customers.ListAsync(cancellationToken)).Select(Mapping.ToDto).ToArray();

    public async Task<CustomerResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await customers.GetByIdAsync(id, cancellationToken);
        return customer is null ? null : Mapping.ToDto(customer);
    }

    public async Task<CustomerResponse> CreateAsync(CustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = new Customer(
            request.FirstName, request.LastName, request.Email, Mapping.ToAddress(request.Address));
        customers.Add(customer);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Mapping.ToDto(customer);
    }

    public async Task<CustomerResponse?> UpdateAsync(
        Guid id,
        CustomerRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await customers.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            return null;
        }

        customer.Update(request.FirstName, request.LastName, request.Email, Mapping.ToAddress(request.Address));
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Mapping.ToDto(customer);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await customers.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            return false;
        }

        customers.Remove(customer);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
