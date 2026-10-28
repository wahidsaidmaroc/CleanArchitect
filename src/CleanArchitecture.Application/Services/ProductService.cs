using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Application.Services;

public sealed class ProductService(
    IRepository<Product> products,
    IRepository<Category> categories,
    IUnitOfWork unitOfWork) : IProductService
{
    public async Task<IReadOnlyList<ProductResponse>> ListAsync(CancellationToken cancellationToken) =>
        (await products.ListAsync(cancellationToken)).Select(Mapping.ToDto).ToArray();

    public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(id, cancellationToken);
        return product is null ? null : Mapping.ToDto(product);
    }

    public async Task<ProductResponse> CreateAsync(ProductRequest request, CancellationToken cancellationToken)
    {
        await EnsureCategoryExists(request.CategoryId, cancellationToken);
        var product = CreateProduct(request);
        products.Add(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Mapping.ToDto(product);
    }

    public async Task<ProductResponse?> UpdateAsync(
        Guid id,
        ProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return null;
        }

        await EnsureCategoryExists(request.CategoryId, cancellationToken);
        product.Update(
            request.Name,
            request.Description,
            new Money(request.Price, request.Currency),
            request.StockQuantity,
            request.CategoryId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Mapping.ToDto(product);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return false;
        }

        products.Remove(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task EnsureCategoryExists(Guid categoryId, CancellationToken cancellationToken)
    {
        if (await categories.GetByIdAsync(categoryId, cancellationToken) is null)
        {
            throw new Application.Exceptions.NotFoundException($"Category '{categoryId}' was not found.");
        }
    }

    private static Product CreateProduct(ProductRequest request) =>
        new(request.Name, request.Description, new Money(request.Price, request.Currency),
            request.StockQuantity, request.CategoryId);
}
