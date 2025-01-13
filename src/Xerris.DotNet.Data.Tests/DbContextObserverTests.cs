using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;

namespace Xerris.DotNet.Data.Tests;

public class DbContextObserverTests : IDisposable
{
    private readonly MockRepository mockRepository;
    private readonly Mock<IDbContextObserver> mockObserver;

    public DbContextObserverTests()
    {
        mockRepository = new MockRepository(MockBehavior.Strict);
        mockObserver = mockRepository.Create<IDbContextObserver>();
    }

    [Fact]
    public async Task Observer_Should_Be_Wired_Up_Correctly()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DbContextObserver>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var customer = new Customer { Name = "Test Customer" };

        mockObserver
            .Setup(o => o.OnEntityTracked(It.IsAny<object>(), It.IsAny<EntityTrackedEventArgs>()))
            .Callback<object, EntityTrackedEventArgs>((_, e) =>
            {
                var tracked = e.Entry.Entity as Customer;
                customer.Should().BeSameAs(tracked);
            });
        
        mockObserver
            .Setup(o => o.OnStateChanged(It.IsAny<object>(), It.IsAny<EntityStateChangedEventArgs>()));
        
        mockObserver.Setup(x => x.OnSaved());

        // Act
        await using var context = new DbContextObserver(options, mockObserver.Object);
        
        context.Customers.Add(customer);
        await context.SaveChangesAsync();
        
        mockObserver.Verify(o => o.OnEntityTracked(It.IsAny<object>(), It.IsAny<EntityTrackedEventArgs>()),
            Times.Once);  
        mockObserver.Verify(o => o.OnStateChanged(It.IsAny<object>(), It.IsAny<EntityStateChangedEventArgs>()),
            Times.Once);
    }

    [Fact]
    public async Task Observer_Should_Receive_Entities_In_Order_They_Were_Added()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DbContextObserver>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var trackedEntities = new List<object>();

        mockObserver
            .Setup(o => o.OnEntityTracked(It.IsAny<object>(), It.IsAny<EntityTrackedEventArgs>()))
            .Callback<object, EntityTrackedEventArgs>((_, e) => { trackedEntities.Add(e.Entry.Entity); });

        //OnStateChanged
        mockObserver
            .Setup(o => o.OnStateChanged(It.IsAny<object>(), It.IsAny<EntityStateChangedEventArgs>()));
        mockObserver
            .Setup(o => o.OnStateChanged(It.IsAny<object>(), It.IsAny<EntityStateChangedEventArgs>()));
        mockObserver
            .Setup(o => o.OnStateChanged(It.IsAny<object>(), It.IsAny<EntityStateChangedEventArgs>()));
        
        mockObserver.Setup(x => x.OnSaved());

        // Act
        await using (var context = new DbContextObserver(options, mockObserver.Object))
        {
            var customer = new Customer { Name = "Customer1" };
            var order = new Order { CustomerId = customer.Id, Description = "Order1" };
            var orderItem = new OrderItem { OrderId = order.Id, ProductName = "Product1" };

            context.Customers.Add(customer);
            context.Orders.Add(order);
            context.OrderItems.Add(orderItem);

            await context.SaveChangesAsync();
        }

        // Assert
        mockObserver.Verify(o => o.OnEntityTracked(It.IsAny<object>(), It.IsAny<EntityTrackedEventArgs>()),
            Times.Exactly(3));
        
        mockObserver.Verify(o => o.OnStateChanged(It.IsAny<object>(), It.IsAny<EntityStateChangedEventArgs>()),
            Times.Exactly(3));
        
        trackedEntities.Count.Should().Be(3);
        trackedEntities[0].Should().BeOfType<Customer>();
        trackedEntities[1].Should().BeOfType<Order>();
        trackedEntities[2].Should().BeOfType<OrderItem>();
    }

    [Fact]
    public async Task Observer_Should_Be_Notified_On_State_Changed()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DbContextObserver>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var customer = new Customer { Name = "Test Customer" };

        mockObserver
            .Setup(o => o.OnEntityTracked(It.IsAny<object>(), It.IsAny<EntityTrackedEventArgs>()));

        mockObserver
            .Setup(o => o.OnStateChanged(It.IsAny<object>(), It.IsAny<EntityStateChangedEventArgs>()))
            .Callback<object, EntityStateChangedEventArgs>((_, e) =>
            {
                e.Entry.Entity.Should().BeSameAs(customer);
                e.OldState.Should().Be(EntityState.Added); //this is the OldState since this if fired AFTER the Save occurs in case it fails.
            });

        mockObserver.Setup(x => x.OnSaved());

        // Act
        await using (var context = new DbContextObserver(options, mockObserver.Object))
        {
            context.Customers.Add(customer);
            await context.SaveChangesAsync();
        }

        // Assert
        
        mockObserver.Verify(o => o.OnEntityTracked(It.IsAny<object>(), It.IsAny<EntityTrackedEventArgs>()),
            Times.Once);
        mockObserver.Verify(o => o.OnStateChanged(It.IsAny<object>(), It.IsAny<EntityStateChangedEventArgs>()),
            Times.Once);
    }

    public void Dispose()
        => mockRepository.VerifyAll();
}