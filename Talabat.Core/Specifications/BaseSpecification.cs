using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class BaseSpecification<T> : ISpecification<T> where T : BaseEntity<int>
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDescending { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool IsPaginationEnable { get; set; }

        //Get By ID
        public BaseSpecification(Expression<Func<T, bool>> criteriaExpression)
        {
            Criteria = criteriaExpression;
        }
        public BaseSpecification()
        {
            
        }
        public void AddOrderBy(Expression<Func<T , object>> expression)
        {
            OrderBy = expression;
        }

        // Method to set OrderByDescending
        public void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDescending = orderByDescExpression;
        }
        public void ApplyPagination (int skip , int take)
        {
            IsPaginationEnable = true;
            Skip = skip;
            Take = take;
        }
    }
}
