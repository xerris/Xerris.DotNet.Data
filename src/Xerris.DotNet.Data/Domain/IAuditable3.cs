namespace Xerris.DotNet.Data.Domain;

public interface IAuditable
{
    public Guid Id { get; set; }
    Guid? CreatedBy { get; set; }
    DateTime CreatedOn { get; set; }
    Guid? ModifiedBy { get; set; }
    DateTime ModifiedOn { get; set; }
    DateTime SynchronizedOn { get; set; }
    public int Version { get; set; }
    public bool IsDeleted { get; set; }
}