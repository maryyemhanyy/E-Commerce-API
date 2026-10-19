using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerec.Infrastructure.Repositories
{
    public class UnitOfWork(StoreDbContext dbContext) : IUnitOfWork
    {
        private readonly Dictionary<string, object> repositories = [];
        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await dbContext.SaveChangesAsync(ct);

        public IGenericRepository<TEntity, TKey> GetRepositoryAsync<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            var typeName = typeof(TEntity).Name;

            if(repositories.TryGetValue(typeName , out object oldRepo))
                return (IGenericRepository<TEntity, TKey>)oldRepo;

            var newRepo = new GenericRepository<TEntity, TKey>(dbContext);
            repositories[typeName] = newRepo;
            return newRepo;
        }
    }
}