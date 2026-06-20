using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.MembersViewModel;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class MemberServices : IMemberServices
    {
        private readonly IGenericRepository<Member> memberRepository;
        private readonly IGenericRepository<Membership> membershipRepository;
        private readonly IGenericRepository<Plan> planRepository;
        private readonly IGenericRepository<HealthRecord> healthRecordRepository;
        private readonly IGenericRepository<Booking> bookingRepository;

        public MemberServices(IGenericRepository<Member> memberRepository, IGenericRepository<Membership> membershipRepository,
            IGenericRepository<Plan> planRepository, IGenericRepository<HealthRecord> healthRecordRepository, IGenericRepository<Booking> bookingRepository)
        {
            this.memberRepository = memberRepository    ;
            this.membershipRepository = membershipRepository;
            this.planRepository = planRepository;
            this.healthRecordRepository = healthRecordRepository;
            this.bookingRepository = bookingRepository;
        }


        //GET
        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await memberRepository.GetAll(false, ct);

            if (members == null)
            {
                return [];
            }

            var MemberViewModels = members.Select(m => new MemberViewModel
            {
                Id = m.Id,
                Photo = m.Photo,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                Gender = m.Gender.ToString()
            });

            return MemberViewModels;
        }

        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberId, CancellationToken ct = default)
        {
            var member = await memberRepository.GetById(memberId, ct);

            if(member == null)
            {
                return null;
            }

            var MemberVM = new MemberViewModel()
            {
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Gender = member.Gender.ToString(),
                Address = $"{member.Address.BuildingNumber}- {member.Address.Street}- {member.Address.City}"
            };

            var ActiveMembership = await membershipRepository.FirstOrDefaultAsync(mb => mb.MemberId == memberId && mb.IsActive, false, ct);

            if(ActiveMembership is not null)
            {
                var plan = await planRepository.GetById(ActiveMembership.PlanId, ct);
                
                MemberVM.PlanName = plan?.Name;
                MemberVM.MembershipStartDate = ActiveMembership.CreatedAt.ToShortDateString();
                MemberVM.MembershipEndDate = ActiveMembership.EndDate.ToShortDateString();
            }

            return MemberVM;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var healthRecord = await healthRecordRepository.FirstOrDefaultAsync(hr => hr.MemberId == memberId, false, ct);
            if (healthRecord is null) 
            {
                return null;
            }

            return new HealthRecordViewModel()
            {
                Weight= healthRecord.Weight,
                Height = healthRecord.Height,
                BloodType = healthRecord.BloodType,
                Note = healthRecord.Note

            };
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member = await memberRepository.GetById(memberId, ct);

            if (member == null)
            {
                return null;
            }

            return new MemberToUpdateViewModel()
            {
                Name = member.Name,
                Photo = member.Photo,
                Email = member.Email,
                Phone = member.Phone,
                BuildingNumber = member.Address.BuildingNumber,
                Street = member.Address.Street,
                City = member.Address.City
            };
        }


        //POST
        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var emailExists = await memberRepository.AnyAsync(m => m.Email == model.Email, ct);
            var phoneExists = await memberRepository.AnyAsync(m => m.Phone == model.Phone, ct);

            if(emailExists || phoneExists)
            {
                return false;
            }

            var member = new Member()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    Street = model.Street,
                    City = model.City
                },
                HealthRecord = new HealthRecord()
                {
                    Weight = model.HealthRecordViewModel.Weight,
                    Height = model.HealthRecordViewModel.Height,
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Note = model.HealthRecordViewModel.Note
                }
            };

            memberRepository.add(member);
            var result = await memberRepository.CompleteAsync();

            return result > 0;

        }

        public async Task<bool> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await memberRepository.GetById(id, ct);

            if (member == null)
            {
                return false;
            }

            if(await memberRepository.AnyAsync(m => m.Email == model.Email && m.Id != id, ct) ||
                await memberRepository.AnyAsync(m => m.Phone == model.Phone && m.Id != id, ct))
            {
                return false;
            }

            member.Phone = model.Phone;
            member.Email = model.Email;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            member.UpdatedAt = DateTime.UtcNow;

            var result = await memberRepository.CompleteAsync();

            return result > 0;



        }

        public async Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct = default)
        {
            var member = await memberRepository.GetById(memberId, ct);

            if(member == null)
            {
                return false;
            }

            var hasActiveSessions = await bookingRepository.AnyAsync(b => b.MemberId == memberId && b.Session.EndDate > DateTime.Now, ct);

            if(hasActiveSessions)
            {
                return false;
            }
            
            memberRepository.remove(memberId);
            var result = await memberRepository.CompleteAsync();

            return result > 0;


        }

    }
}
