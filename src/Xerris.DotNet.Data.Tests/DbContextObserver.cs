using Microsoft.EntityFrameworkCore;

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