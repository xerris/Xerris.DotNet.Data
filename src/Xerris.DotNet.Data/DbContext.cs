using Microsoft.EntityFrameworkCore;
using Xerris.DotNet.Core.Extensions;
using Xerris.DotNet.Data.Audit;
using Xerris.DotNet.Data.Domain;

namespace Xerris.DotNet.Data;

public abstract class DbContext<T> : DbContext where T : DbContext
{
    private readonly AuditVisitor auditVisitor;
    private readonly IDbContextObserver? observer;
    private Guid? TokenUserId { get; set; }

    protected DbContext(DbContextOptions<T> options, IDbContextObserver observer)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        auditVisitor = new AuditVisitor();
        this.observer = observer;
        if (this.observer != null)
            base.ChangeTracker.Tracked += this.observer.OnEntityTracked!;
    }

    public DbContext WithUserId(Guid userId)
    {
        TokenUserId = userId;
        return this;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        RegisterModels(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    public override void Dispose()
    {
        if (observer != null)
            base.ChangeTracker.Tracked -= observer.OnEntityTracked!;
        base.Dispose();
    }

    protected abstract void RegisterModels(ModelBuilder modelBuilder);

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeConverter>();
        base.ConfigureConventions(configurationBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker
            .Entries()
            .Where(e => e is { Entity: IAuditable, State: EntityState.Added })
            .ForEach(x => auditVisitor.AcceptNew(x, TokenUserId));

        ChangeTracker
            .Entries()
            .Where(e => e is { Entity: IAuditable, State: EntityState.Modified })
            .ForEach(x => auditVisitor.AcceptModified(x, TokenUserId));

        //deleted entities
        ChangeTracker
            .Entries()
            .Where(e => e is { Entity: IDeleteable, State: EntityState.Deleted })
            .ForEach(x => auditVisitor.AcceptDeleted(x, TokenUserId));

        return await base.SaveChangesAsync(cancellationToken);
    }
}