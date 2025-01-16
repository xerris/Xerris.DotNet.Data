using Microsoft.EntityFrameworkCore;

namespace Xerris.DotNet.Data.Tests.Context;

public class TestDbContextFactory : DbContextFactory<TestDbContext>
{
    public TestDbContextFactory(IConnectionBuilder connectionBuilder, IDbContextObserver observer)
        : base(connectionBuilder, observer)
    {
    }

    protected override TestDbContext Create(DbContextOptions<DbContextBase> applyOptions,
        IDbContextObserver dbContextObserver)
    {
        return new TestDbContext(applyOptions, dbContextObserver);
    }

    protected override DbContextOptions<DbContextBase> ApplyOptions(bool sensitiveDataLoggingEnabled = false)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DbContextBase>();
        if (sensitiveDataLoggingEnabled)
            optionsBuilder.EnableSensitiveDataLogging();

        optionsBuilder.UseInMemoryDatabase("TestDatabase");
        return optionsBuilder.Options;
    }
}