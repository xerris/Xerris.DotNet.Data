using System.ComponentModel.DataAnnotations;
using Xerris.DotNet.Data.Domain;

namespace Xerris.DotNet.Data.Tests;

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