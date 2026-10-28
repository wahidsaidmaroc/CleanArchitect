namespace CleanArchitecture.Application.DTOs;

public sealed record CustomerRequest(string FirstName, string LastName, string Email, AddressDto Address);

public sealed record CustomerResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    AddressDto Address);
