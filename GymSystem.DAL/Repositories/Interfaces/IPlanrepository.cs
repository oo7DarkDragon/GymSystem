using GymSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repositories.Interfaces
{
    public interface IPlanrepository
    {
        Task<IEnumerable<Plan>> GetAll(bool isTracked,CancellationToken ct=default);
        Task<Plan?> GetById(int id, CancellationToken ct = default);

        void add (Plan plan);

        void update (Plan plan);    

        void remove (int id);

        Task<int> CompleteAsync();
     
    }
}
