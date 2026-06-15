using GymSystem.DAL.Context;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repositories.Classes
{
    public class PlanRepository : IPlanrepository
    {
        private readonly GymDbContext dbContext;

        public PlanRepository(GymDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public void add(Plan plan)
        {
            dbContext.Plans.Add(plan);
        }


        public async Task<IEnumerable<Plan>> GetAll(bool isTracked, CancellationToken ct= default)
        {
            var plans = isTracked ? dbContext.Plans : dbContext.Plans.AsNoTracking();
            return await plans.ToListAsync();
        }

       

        public async Task<Plan?> GetById(int id, CancellationToken ct=default)
        {
            var plan = dbContext.Plans.FirstOrDefaultAsync(p => p.Id == id);
            return await plan;
        }

        public void remove(int id)
        {
            var product = dbContext.Plans.FirstOrDefault(p=>p.Id == id);
            if (product != null)
                dbContext.Plans.Remove(product);
        }

        public void update(Plan plan)
        {
            dbContext.Plans.Update(plan);
        }
        public async Task<int> CompleteAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
    }
}
