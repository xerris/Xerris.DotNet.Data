using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Xerris.DotNet.Data.Domain;

namespace Xerris.DotNet.Data.Tests;


public class DbContextObserver : DbContext<DbContextObserver>
{
    public DbContextObserver(DbContextOptions<DbContextObserver> options, IDbContextObserver observer)
        : base(options, observer)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
    protected override void RegisterModels(ModelBuilder modelBuilder)
    {
        //do nothing
    }
}

public class Customer : AuditImmutableBase
{
    
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}

public class Order : AuditImmutableBase
{
    public Guid CustomerId { get; set; }
    
    [MaxLength(100)]
    public string Description { get; set; } = string.Empty;
}

public class OrderItem : AuditImmutableBase
{
    public Guid OrderId { get; set; }
    
    [MaxLength(100)]
    public string ProductName { get; set; } = string.Empty;
}