using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence;

public sealed class EfRepository<TEntity>(AppDbContext context) : IRepository<TEntity>
    where TEntity : BaseEntity
{
    public async Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken) =>
        await context.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);

    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.Set<TEntity>().SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    public void Add(TEntity entity) => context.Set<TEntity>().Add(entity);

    public void Remove(TEntity entity) => context.Set<TEntity>().Remove(entity);
}
