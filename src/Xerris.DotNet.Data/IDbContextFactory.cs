using Microsoft.EntityFrameworkCore;

namespace Xerris.DotNet.Data;

public interface IDbContextFactory<out  T> where T : DbContext
{
    T Create();
}

public abstract class DbContextFactory<T> : IDbContextFactory<T> where T : DbContext
{
    protected readonly IConnectionBuilder ConnectionBuilder;
    protected readonly IDbContextObserver Observer;

    // ReSharper disable once ConvertToPrimaryConstructor
    public DbContextFactory(IConnectionBuilder connectionBuilder, IDbContextObserver observer)
    {
        ConnectionBuilder = connectionBuilder;
        this.Observer = observer;
    } 
    
    public T Create() => Create(ApplyOptions(), Observer);
    protected abstract T Create(DbContextOptions<DbContextBase> applyOptions, IDbContextObserver dbContextObserver);
    protected abstract DbContextOptions<DbContextBase> ApplyOptions(bool sensitiveDataLoggingEnabled = false);
}