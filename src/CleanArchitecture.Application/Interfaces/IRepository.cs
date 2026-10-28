using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Application.Interfaces;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(TEntity entity);
    void Remove(TEntity entity);
}
