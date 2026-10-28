using CleanArchitecture.Application.Interfaces;

namespace CleanArchitecture.Infrastructure.Persistence;

public sealed class EfUnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
        context.SaveChangesAsync(cancellationToken);
}
