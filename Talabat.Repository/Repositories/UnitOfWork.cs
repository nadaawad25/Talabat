using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Repository.Data.Contexts;

namespace Talabat.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork

    {
        private Hashtable _Repositories;
        private readonly StoreDbContext _storeDbContext;
        public UnitOfWork(StoreDbContext storeDbContext)
        {
            _Repositories = new Hashtable();
            _storeDbContext = storeDbContext;
        }
        public async Task<int> CompleteAsync()

        {
           return await _storeDbContext.SaveChangesAsync();   
        }

        public ValueTask DisposeAsync()
            => _storeDbContext.DisposeAsync();
        
        //implimatation of unit of work (created once in project ) 
        

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity<int>
        {
           var  type = typeof(TEntity).Name;
            if(! _Repositories.ContainsKey(type) )
            {
                var Repository = new GenericRepository<TEntity>(_storeDbContext);
                _Repositories.Add(type,Repository);
               

            }
            return _Repositories[type] as IGenericRepository<TEntity>;
        }
    }
}
