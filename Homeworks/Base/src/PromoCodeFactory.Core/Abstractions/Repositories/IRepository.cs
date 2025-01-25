using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        public Task<IList<T>> GetAllAsync();
        public Task<T> GetByIdAsync(Guid id);
        public Task CreateAsync(T entity);
        public Task<T> DeleteById(Guid id);
    }
}