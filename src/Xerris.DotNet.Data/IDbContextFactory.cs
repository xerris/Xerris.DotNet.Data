using Microsoft.EntityFrameworkCore;

namespace Xerris.DotNet.Data;

public interface IDbContextFactory<out  T> where T : Microsoft.EntityFrameworkCore.DbContext
{
    T Create();
}

public abstract class DbContextFactory<T> : IDbContextFactory<T> where T : Microsoft.EntityFrameworkCore.DbContext
{
    protected readonly IConnectionBuilder ConnectionBuilder;

    // ReSharper disable once ConvertToPrimaryConstructor
    public DbContextFactory(IConnectionBuilder connectionBuilder)
        => ConnectionBuilder = connectionBuilder;

    public T Create() => Create(ApplyOptions());
    protected abstract T Create(DbContextOptions<T> applyOptions);

    protected abstract DbContextOptions<T> ApplyOptions(bool sensitiveDataLoggingEnabled = false);
}