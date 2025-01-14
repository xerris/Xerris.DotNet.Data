using System.ComponentModel.DataAnnotations;
using Xerris.DotNet.Data.Domain;

namespace Xerris.DotNet.Data.Tests.Domain;

public class Order : AuditImmutableBase
{
    public Guid CustomerId { get; set; }
    
    [MaxLength(100)]
    public string Description { get; set; } = string.Empty;
}