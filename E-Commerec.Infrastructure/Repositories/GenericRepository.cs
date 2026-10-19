using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerec.Infrastructure.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        private readonly StoreDbContext _dbContext;

        public GenericRepository(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(TEntity entity)
        => _dbContext.Set<TEntity>().Add(entity);
        public void Delete(TEntity entity)
        => _dbContext.Set<TEntity>().Remove(entity);
        public void Update(TEntity entity)
        => _dbContext.Set<TEntity>().Update(entity);
        public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default)
        => await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync(ct);

        public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default)
        => await _dbContext.Set<TEntity>().FindAsync(id, ct);

        public async Task<IReadOnlyList<TEntity>> GetAllWithSpecificationsAsync(ISpecifications<TEntity, TKey> specifications, CancellationToken ct = default)
        {
            var result = await SpecificationEvaluator.CreateQuery<TEntity, TKey>(_dbContext.Set<TEntity>().AsNoTracking(), specifications).ToListAsync(ct);
            return result;
        }

        public async Task<TEntity?> GetByIdWithSpecificationsAsync(ISpecifications<TEntity, TKey> specifications, CancellationToken ct = default)
        {
            var result = await SpecificationEvaluator.CreateQuery<TEntity, TKey>(_dbContext.Set<TEntity>(), specifications).FirstOrDefaultAsync(ct);
            return result;
        }

        public async Task<int> GetProductCountWithSpecificationsAsync(ISpecifications<TEntity, TKey> specifications, CancellationToken ct = default)
        {
            var result = await SpecificationEvaluator.CreateQuery<TEntity, TKey>(_dbContext.Set<TEntity>(), specifications).CountAsync(ct);
            return result;
        }
    }
}
