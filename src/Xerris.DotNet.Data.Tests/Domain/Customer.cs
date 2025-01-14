using System.ComponentModel.DataAnnotations;
using Xerris.DotNet.Data.Domain;

namespace Xerris.DotNet.Data.Tests.Domain;

public class Customer : AuditImmutableBase
{
    
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}