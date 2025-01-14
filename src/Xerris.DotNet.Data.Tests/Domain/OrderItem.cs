using System.ComponentModel.DataAnnotations;
using Xerris.DotNet.Data.Domain;

namespace Xerris.DotNet.Data.Tests.Domain;

public class OrderItem : AuditImmutableBase
{
    public Guid OrderId { get; set; }
    
    [MaxLength(100)]
    public string ProductName { get; set; } = string.Empty;
}