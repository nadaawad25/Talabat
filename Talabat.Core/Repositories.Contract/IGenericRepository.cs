using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Repositories.Contract
{
    public interface IGenericRepository <T> where T : BaseEntity <int>
    {
        #region  withoutSpesifications 
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);

        #endregion
        #region With Specification
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> specification);

        Task<T> GetByEntityWithSpecAsync (ISpecification<T> specification);

       

        Task<int> GetCountAsyncWithSpec (ISpecification<T> specification);

        Task AddAsync(T item);
       
        void Update(T item);
        void Delete(T item);

        #endregion

    }
}
