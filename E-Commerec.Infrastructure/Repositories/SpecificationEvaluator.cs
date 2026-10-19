using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerec.Infrastructure.Repositories
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> CreateQuery<TEntity, TKey>(IQueryable<TEntity> InputQuery, ISpecifications<TEntity, TKey> specifications) where TEntity : BaseEntity<TKey>
        {
            var query = InputQuery;

            #region Include
            if (specifications.IncludeExpressions.Count > 0)
                query = specifications.IncludeExpressions.Aggregate(query, (current, expression) => current.Include(expression));
            #endregion

            #region Where
            if (specifications.Criteria != null)
                query = query.Where(specifications.Criteria);
            #endregion

            #region Sorting
            if (specifications.OrderBy is not null) query = query.OrderBy(specifications.OrderBy);
            if (specifications.OrderByDesc is not null) query = query.OrderByDescending(specifications.OrderByDesc);

            #endregion

            #region Pagination
            if (specifications.IsPaginated)
            {
                query = query.Skip(specifications.Skip).Take(specifications.Take);
            }
            #endregion
            return query;

        }
    }
}
