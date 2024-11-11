using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications;
using Talabat.Repository.Data.Contexts;
using Talabat.Repository.Specifiaction;

namespace Talabat.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity<int>
    {
        private StoreDbContext _dbContext;
        public GenericRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        #region WithOut Specification
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
          return await _dbContext.Set<T>().ToListAsync();
        }
        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        #endregion


        #region With Specification
        public IQueryable<T> ApplySpecifications (ISpecification<T> specification)
        {
            return  SpecifiactionEvalutor<T>.GetQuery(_dbContext.Set<T>(), specification);
        }
        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> specification)
        {
            return await ApplySpecifications(specification).ToListAsync();
        }
        public async Task<T> GetByEntityWithSpecAsync(ISpecification<T> specification)
        {
           return await ApplySpecifications(specification).FirstOrDefaultAsync();
        }
        public async Task<int> GetCountAsyncWithSpec(ISpecification<T> specification)
        {
            return await ApplySpecifications(specification).CountAsync();
        }

        public async Task AddAsync(T item)
        => await  _dbContext.Set<T>().AddAsync(item);
        

        public void Update(T item)
        => _dbContext.Set<T>().Update(item);

        public void Delete(T item)
        => _dbContext.Remove(item);




        #endregion


    }
}
