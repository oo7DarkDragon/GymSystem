using GymSystem.DAL.Context;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repositories.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : BaseEntity, new()
    {
        private readonly GymDbContext dbContext;

        public GenericRepository(GymDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void add(TEntity entity)
        {
            dbContext.Set<TEntity>().Add(entity);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return await dbContext.Set<TEntity>().AnyAsync(predicate,ct);
        }

        public async Task<int> CompleteAsync()
        {
            return await dbContext.SaveChangesAsync();
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
            => predicate is null ? dbContext.Set<TEntity>().AsNoTracking().CountAsync(ct) : dbContext.Set<TEntity>().AsNoTracking().CountAsync(predicate, ct);

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool isTracked = false, CancellationToken ct = default)
        {
            var entity = isTracked ? dbContext.Set<TEntity>() : dbContext.Set<TEntity>().AsNoTracking();
            return await entity.FirstOrDefaultAsync(predicate, ct);
        }

        public async Task<IEnumerable<TEntity>> GetAll(bool isTracked, CancellationToken ct = default)
        {

            var entity = isTracked ? dbContext.Set<TEntity>() : dbContext.Set<TEntity>().AsNoTracking();
            return await entity.ToListAsync();
        }

        public async Task<TEntity?> GetById(int id, CancellationToken ct = default)
        {

            var entity = dbContext.Set<TEntity>().FirstOrDefaultAsync(p => p.Id == id);
            return await entity;
        }

        public void remove(int id)
        {
            var entity = dbContext.Set<TEntity>().FirstOrDefault(p => p.Id == id);
            if (entity != null)
                dbContext.Set<TEntity>().Remove(entity);
        }

        public void update(TEntity entity)
        {
            dbContext.Set<TEntity>().Update(entity);
        }


    }
}
