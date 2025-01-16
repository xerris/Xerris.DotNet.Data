using Microsoft.EntityFrameworkCore;

namespace Xerris.DotNet.Data.Tests.Context;

public class TestDbContextFactory : DbContextFactory<TestDbContext>
{
    public TestDbContextFactory(IConnectionBuilder connectionBuilder, IDbContextObserver observer)
        : base(connectionBuilder, observer)
    {
    }

    protected override TestDbContext Create(DbContextOptions<DbContext> applyOptions,
        IDbContextObserver dbContextObserver)
    {
        return new TestDbContext(applyOptions, dbContextObserver);
    }

    protected override DbContextOptions<DbContext> ApplyOptions(bool sensitiveDataLoggingEnabled = false)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DbContext>();
        if (sensitiveDataLoggingEnabled)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        optionsBuilder.UseInMemoryDatabase("TestDatabase");
        return optionsBuilder.Options;
    }
}