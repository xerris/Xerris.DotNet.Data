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
    public void Observer_Should_Be_Wired_Up_Correctly()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DbContextObserver>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        Customer? trackedCustomer = null!;
        EntityTrackedEventArgs trackedEventArgs = null!;

        mockObserver
            .Setup(o => o.OnEntityTracked(It.IsAny<object>(), It.IsAny<EntityTrackedEventArgs>()))
            .Callback<object, EntityTrackedEventArgs>((_, e) =>
            {
                trackedCustomer = e.Entry.Entity as Customer;
                trackedEventArgs = e;
                trackedCustomer.Should().NotBeNull();
            })
            .Verifiable();

        // Act
        using (var context = new DbContextObserver(options, mockObserver.Object))
        {
            var customer = new Customer { Name = "Test Customer" };
            context.Customers.Add(customer);
            context.SaveChanges();
        }

        // Assert
        mockObserver.Verify(o => o.OnEntityTracked(It.IsAny<object>(), It.IsAny<EntityTrackedEventArgs>()), Times.Once);
        trackedEventArgs.Should().NotBeNull();
        trackedEventArgs.Entry.Entity.Should().Be(trackedCustomer);
    }

    [Fact]
    public void Observer_Should_Receive_Entities_In_Order_They_Were_Added()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<DbContextObserver>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var trackedEntities = new List<object>();

        mockObserver
            .Setup(o => o.OnEntityTracked(It.IsAny<object>(), It.IsAny<EntityTrackedEventArgs>()))
            .Callback<object, EntityTrackedEventArgs>((_, e) =>
            {
                trackedEntities.Add(e.Entry.Entity);
            })
            .Verifiable();

        // Act
        using (var context = new DbContextObserver(options, mockObserver.Object))
        {
            var customer = new Customer { Name = "Customer1" };
            var order = new Order { CustomerId = customer.Id, Description = "Order1" };
            var orderItem = new OrderItem { OrderId = order.Id, ProductName = "Product1" };

            context.Customers.Add(customer);
            context.Orders.Add(order);
            context.OrderItems.Add(orderItem);
            
            context.SaveChanges();
        }

        // Assert
        mockObserver.Verify(o => o.OnEntityTracked(It.IsAny<object>(), It.IsAny<EntityTrackedEventArgs>()), Times.Exactly(3));
        trackedEntities.Count.Should().Be(3);
        trackedEntities[0].Should().BeOfType<Customer>();
        trackedEntities[1].Should().BeOfType<Order>();
        trackedEntities[2].Should().BeOfType<OrderItem>();
    }

    public void Dispose()
    {
        mockRepository.VerifyAll();
    }
}