using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T>
        where T : BaseEntity
    {
        void Create(T entity); 
        public Task<ICollection<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);

        Task<Task<int>> UpdateAsync(T entity);
        
        public void Delete(T entity);
    }
}