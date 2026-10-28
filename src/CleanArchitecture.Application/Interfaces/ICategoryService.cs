using CleanArchitecture.Application.DTOs;

namespace CleanArchitecture.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponse>> ListAsync(CancellationToken cancellationToken);
    Task<CategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<CategoryResponse> CreateAsync(CategoryRequest request, CancellationToken cancellationToken);
    Task<CategoryResponse?> UpdateAsync(Guid id, CategoryRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
