using AutoMapper;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class PlanServices : IPlanServices
    {
        private readonly IUnitOfWork unitOfWork;

        public PlanServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            Mapper = mapper;
        }

        public IMapper Mapper { get; }

        public async Task<IEnumerable<Plan>> GetAllPlansAsync(CancellationToken ct)
        {
            var planRepo = unitOfWork.GetRepository<Plan>();
            return await planRepo.GetAll(false, ct);
        }

        public async Task<Plan> GetPlanByIdAsync(int id, CancellationToken ct)
        {
            var planRepo = unitOfWork.GetRepository<Plan>();
            return await planRepo.GetById(id,ct);
            
        }
    }
}
