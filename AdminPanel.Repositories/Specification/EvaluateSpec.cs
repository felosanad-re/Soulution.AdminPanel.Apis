using AdminPanel.Core.Entities;
using AdminPanel.Core.Specifications;
using AdminPanel.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Repositories.Specification
{
    public static class EvaluateSpec<T> where T : ModelBase
    {
        public static IQueryable<T> GetQuery(IQueryable<T> initialQuery, ISpecifications<T> spec)
        {
            var query = initialQuery;
            // Add Creteria 
            if (spec.Creteria != null) query = query.Where(spec.Creteria);
            // Set Order By
            if(spec.OrderBy != null) query = query.OrderBy(spec.OrderBy);
            // set order by desc
            if(spec.OrderByDesc != null) query = query.OrderByDescending(spec.OrderByDesc);
            // Is Tracking
            if(!spec.IsTracking) query = query.AsNoTracking();
            // Pagination
            if (spec.IsPagination) query = query.Skip(spec.Skip).Take(spec.Take);
            // Includes
            query = spec.Includes.Aggregate(query, (baseQuery, nextQuery) => baseQuery.Include(nextQuery));

            return query;
        }
    }
}
