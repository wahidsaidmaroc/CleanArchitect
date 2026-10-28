namespace CleanArchitecture.Application.DTOs;

public sealed record CategoryRequest(string Name);

public sealed record CategoryResponse(Guid Id, string Name);
