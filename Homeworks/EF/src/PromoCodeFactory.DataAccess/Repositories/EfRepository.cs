using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private DataBaseContext _dataBaseContext;
        
        public EfRepository(DataBaseContext context)
        {
            _dataBaseContext = context;
        }
        
        public void Create(T entity)
        {
            var dbSet = _dataBaseContext.Set<T>();
            dbSet.Add(entity);
            _dataBaseContext.SaveChanges();
        }

        public Task<ICollection<T>> GetAllAsync()
        {
            _dataBaseContext.TryGetEntities<T>(out var entities);
            return Task.FromResult(entities);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            _dataBaseContext.TryGetEntities<T>(out var entities);
            return Task.FromResult(entities.FirstOrDefault(x => x.Id == id));
        }

        public Task<Task<int>> UpdateAsync(T entity)
        {
            return Task.FromResult(_dataBaseContext.SaveChangesAsync());
        }
        
        public void Delete(T entity)
        {
            var dbSet = _dataBaseContext.Set<T>();
            dbSet.Remove(entity);
            _dataBaseContext.SaveChanges();
        }
    }
}