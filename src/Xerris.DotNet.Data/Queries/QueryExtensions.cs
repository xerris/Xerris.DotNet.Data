using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Xerris.DotNet.Data.Domain;
using Xerris.DotNet.Data.Exceptions;

namespace Xerris.DotNet.Data.Queries;

public static class QueryExtensions
{
    public static async Task<T> FindById<T>(this DbSet<T> dbSet, Guid id, 
        params Expression<Func<T, object>>[] includes) where T : class, IAuditable
    {
        var query = includes.Aggregate<Expression<Func<T, object>>?, IQueryable<T>>(dbSet,
            (current, include) => current.Include(include!));

        var entity = await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        return entity ?? throw new NotFoundException<T>(id);
    }

}