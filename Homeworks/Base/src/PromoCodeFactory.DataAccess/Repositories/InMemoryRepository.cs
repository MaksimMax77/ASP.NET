using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected IList<T> Data { get; set; }

        public InMemoryRepository(IList<T> data)
        {
            Data = data;
        }

        public Task<IList<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }
        
        public async Task CreateAsync(T entity)
        {
            await Task.Run(()=>
            {
                Data.Add(entity);
            });
        }
        
        public Task<T> DeleteById(Guid id)
        {
            var searchResult = Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
            Data.Remove(searchResult.Result);
            return searchResult;
        }
    }
}