using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xerris.DotNet.Core.Extensions;
using Xerris.DotNet.Data.Audit;
using Xerris.DotNet.Data.Domain;
using Xerris.DotNet.Data.Extensions;

namespace Xerris.DotNet.Data;

public abstract class DbContext<T> : DbContext where T : DbContext
{
    private readonly AuditVisitor auditVisitor;
    private Guid? TokenUserId { get; set; }

    public DbContext(DbContextOptions<T> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        auditVisitor = new AuditVisitor();
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

public class DateTimeConverter() : ValueConverter<DateTime, DateTime>(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

public class EnumToDescriptionConverter<TEnum>() : 
    ValueConverter<TEnum, string>(v => v.GetDescription(), v => v.FromDescription<TEnum>()) where TEnum : Enum;