using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Services;

public sealed class CategoryService(
    IRepository<Category> categories,
    IUnitOfWork unitOfWork) : ICategoryService
{
    public async Task<IReadOnlyList<CategoryResponse>> 
        ListAsync(CancellationToken cancellationToken) =>
        (await categories.ListAsync(cancellationToken)).Select(Mapping.ToDto).ToArray();

    public async Task<CategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await categories.GetByIdAsync(id, cancellationToken);
        return category is null ? null : Mapping.ToDto(category);
    }

    public async Task<CategoryResponse> CreateAsync(CategoryRequest request, CancellationToken cancellationToken)
    {
        var category = new Category(request.Name);
        categories.Add(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Mapping.ToDto(category);
    }

    public async Task<CategoryResponse?> UpdateAsync(
        Guid id,
        CategoryRequest request,
        CancellationToken cancellationToken)
    {
        var category = await categories.GetByIdAsync(id, cancellationToken);
        if (category is null)
        {
            return null;
        }

        category.Rename(request.Name);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Mapping.ToDto(category);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await categories.GetByIdAsync(id, cancellationToken);
        if (category is null)
        {
            return false;
        }

        categories.Remove(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
