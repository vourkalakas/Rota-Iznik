using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Data
{
    public static class SorgulamaUzantıları
    {
        public static IQueryable<T> IncludeProperties<T>(this IQueryable<T> queryable,
            params Expression<Func<T, object>>[] includeProperties)
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            foreach (var includeProperty in includeProperties)
                queryable = queryable.Include(includeProperty);

            return queryable;
        }
    }
}
