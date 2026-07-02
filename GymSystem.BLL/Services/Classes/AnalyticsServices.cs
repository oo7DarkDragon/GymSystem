using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.AnalyticsViewModel;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class AnalyticsServices : IAnalyticsServices
    {
        private readonly IUnitOfWork unitOfWork;

        public AnalyticsServices(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<AnalyticsViewModel> GetAnalyticalDataAsync(CancellationToken ct = default)
        {
            var sessions= await unitOfWork.GetRepository<Session>().GetAll(false,ct);


            var totalTrainers = await unitOfWork.GetRepository<Trainer>().CountAsync();
            var totalMember = await unitOfWork.GetRepository<Member>().CountAsync();
            var activeMembers = await unitOfWork.GetRepository<Membership>().CountAsync(m=> m.EndDate> DateTime.Now);

            return new AnalyticsViewModel
            {
                TotalTrainers = totalTrainers,
                TotalMembers = totalMember,
                ActiveMembers = activeMembers,
                UpcomingSessions = sessions.Count(x=> x.EndDate > DateTime.Now),
                OngoingSessions = sessions.Count(x=> x.StartDate<= DateTime.Now && x.EndDate >= DateTime.Now),
                CompletedSessions = sessions.Count(x=>x.EndDate < DateTime.Now)
            };
        }
    }
}
