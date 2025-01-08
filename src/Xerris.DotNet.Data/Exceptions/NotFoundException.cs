using Xerris.DotNet.Data.Domain;

namespace Xerris.DotNet.Data.Exceptions;

public class NotFoundException<T>(Guid id) : Exception where T : class, IAuditable
{
    public override string Message =>  $"{typeof(T).Name} with id: {id} not found";
}