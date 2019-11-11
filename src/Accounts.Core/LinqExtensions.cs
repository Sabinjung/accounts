using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Accounts.Extensions
{
    public static class LinqExtensions
    {

        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> queryable, bool condition, Expression<Func<TEntity, bool>> predicate)
        {
            return condition ? queryable.Where(predicate) : queryable;
        }
    }
}
