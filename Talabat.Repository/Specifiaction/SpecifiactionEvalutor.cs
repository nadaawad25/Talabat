using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository.Specifiaction
{
    public class SpecifiactionEvalutor<T> where T : BaseEntity<int>
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;

            // Apply criteria if it exists
            if (specification.Criteria is not null)
                query = query.Where(specification.Criteria);

            
            if(specification.OrderBy is not null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            if(specification.OrderByDescending is not null)
            {
                query.OrderByDescending(specification.OrderByDescending);
            }

            if (specification.IsPaginationEnable)
            {
                query = query.Skip(specification.Skip).Take(specification.Take);
            }
            // Apply includes
            query = specification.Includes.Aggregate(query,
                (currentQuery, includeExpression) => currentQuery.Include(includeExpression));

            return query;
        }
    }
}
