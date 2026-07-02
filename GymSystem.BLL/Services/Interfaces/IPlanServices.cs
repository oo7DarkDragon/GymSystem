using GymSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Interfaces
{
    public interface IPlanServices
    {
        Task<IEnumerable<Plan>> GetAllPlansAsync(CancellationToken ct);
        Task<Plan> GetPlanByIdAsync(int id, CancellationToken ct);
    }
}
