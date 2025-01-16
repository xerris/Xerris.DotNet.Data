using Microsoft.EntityFrameworkCore;

namespace Xerris.DotNet.Data;

public interface IDbContextFactory<out  T> where T : DbContext
{
    T Create();
}

public abstract class DbContextFactory<T> : IDbContextFactory<T> where T : DbContext
{
    protected readonly IConnectionBuilder ConnectionBuilder;
    protected readonly IDbContextObserver observer;

    // ReSharper disable once ConvertToPrimaryConstructor
    public DbContextFactory(IConnectionBuilder connectionBuilder, IDbContextObserver observer)
    {
        ConnectionBuilder = connectionBuilder;
        this.observer = observer;
    } 
    
    public T Create() => Create(ApplyOptions(), observer);
    protected abstract T Create(DbContextOptions<DbContext> applyOptions, IDbContextObserver dbContextObserver);
    protected abstract DbContextOptions<DbContext> ApplyOptions(bool sensitiveDataLoggingEnabled = false);
}