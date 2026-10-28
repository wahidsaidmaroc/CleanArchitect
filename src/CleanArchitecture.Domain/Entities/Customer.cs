using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Domain.Entities;

public class Customer : BaseEntity
{
    private Customer()
    {
    }

    public Customer(string firstName, string lastName, string email, Address address)
    {
        Update(firstName, lastName, email, address);
    }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public Address Address { get; private set; } = null!;

    public void Update(string firstName, string lastName, string email, Address address)
    {
        if (string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("First name, last name and email are required.");
        }

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim();
        Address = address ?? throw new DomainException("Customer address is required.");
    }
}
