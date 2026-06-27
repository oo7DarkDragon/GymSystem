using GymSystem.DAL.Context;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext dbContext;
        private readonly Dictionary<string, object> _Repos = [];

        public ISessionRepository SessionRepository { get; }

        public UnitOfWork(GymDbContext dbContext) 
        {
            this.dbContext = dbContext;
            this.SessionRepository = new SessionRepository(dbContext);
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var typeName = typeof(TEntity).Name;
            if (_Repos.TryGetValue(typeName, out object oldRepo))
            {
                return (IGenericRepository<TEntity>) oldRepo;
            }

            var newRepo = new GenericRepository<TEntity>(dbContext);
            _Repos[typeName] = newRepo;
            return newRepo;
        }

        public async Task<int> CompleteAsync()
        {
            return await dbContext.SaveChangesAsync();
        }

    }
}
