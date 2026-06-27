using AutoMapper;
using GymSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymSystem.BLL.Common;
using Azure.Core.Pipeline;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GymSystem.BLL.Services.Classes
{
    public class SessionServices : ISessionServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public SessionServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct)
        {
            if ((model.EndDate <= model.StartDate) || (model.EndDate <= DateTime.Now)) return Result.ValidationFailed("End date must be after start date and in the future.");

            var TrainerRepo = unitOfWork.GetRepository<Trainer>();
            var Trainer = await TrainerRepo.GetById(model.TrainerId, ct);

            if (Trainer is null) return Result.NotFound("Trainer not found.");

            var categoryRepo = unitOfWork.GetRepository<Category>();
            var category = await categoryRepo.GetById(model.CategoryId, ct);
            if (category is null) return Result.NotFound("Category not found.");

            var session = mapper.Map<CreateSessionViewModel, Session>(model);

            var sessionRepo = unitOfWork.GetRepository<Session>();

            sessionRepo.add(session);

            var rowsAffected = await unitOfWork.CompleteAsync();
            return rowsAffected > 0 ? Result.Ok() : Result.Failed("Failed to create session.");

        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct)
        {
           var sessions = await unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategoryAsync(ct);
           if (!sessions.Any()) return null;

           sessions = sessions.OrderByDescending(session => session.StartDate);

            var MappedSessions = mapper.Map<IEnumerable<Session>, IEnumerable<SessionViewModel>>(sessions);

            foreach (var session in MappedSessions)
            {
                session.AvailableSlots = session.Capacity - await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(session.Id, ct);
            }

            return MappedSessions;
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownMenuAsync(CancellationToken ct = default)
        {
            var categories = await unitOfWork.GetRepository<Category>().GetAll(false, ct);
            return mapper.Map<IEnumerable<Category>, IEnumerable<CategorySelectViewModel>>(categories);
        }

        public async Task<SessionViewModel> GetSessionByIdAsync(int sessionId, CancellationToken ct)
        {
            var session = await unitOfWork.SessionRepository.GetSessionByIDWithTrainerAndCategoryAsync(sessionId, ct);

            if (session is null) return null;

            var mappedSession = mapper.Map<Session, SessionViewModel>(session);

            mappedSession.AvailableSlots = mappedSession.Capacity - await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(sessionId, ct);

            return mappedSession;


        }

        public async Task<UpdateSessionViewModel> GetSessionToUpdateByIdAsync(int sessionId, CancellationToken ct)
        {
            var session = await unitOfWork.GetRepository<Session>().GetById(sessionId, ct);

            if (session is null) return null;

            if (!await IsSessionValidForUpdateAsync(session, ct)) return null;

            return mapper.Map<Session, UpdateSessionViewModel>(session);



        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownMenuAsync(CancellationToken ct = default)
        {
            var Trainer = await unitOfWork.GetRepository<Trainer>().GetAll(false, ct);
            return mapper.Map<IEnumerable<Trainer>, IEnumerable<TrainerSelectViewModel>>(Trainer);
        }


        public async Task<bool> IsSessionValidForUpdateAsync(Session session, CancellationToken ct)
        {
            if(session.StartDate<= DateTime.Now) return false;
            var booked = await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(session.Id, ct);

            return !(booked > 0);
        }

        public async Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct)
        {
            var repo = unitOfWork.GetRepository<Session>();
            var session = await repo.GetById(sessionId, ct);
            if (session is null) return Result.NotFound("Session not found.");

            if (session.EndDate >= DateTime.Now) return Result.Failed("Can not delete a asession that has not yet finished.");

            var bookedCount = await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(sessionId, ct);
            if (bookedCount > 0)
                return Result.Failed("Can not delete a session that has been booked.");

            repo.remove(sessionId);

            var affectedRows = await unitOfWork.CompleteAsync();
            return affectedRows > 0 ? Result.Ok() : Result.Failed("Failed to delete session");
        }

        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var sessionRepo = unitOfWork.GetRepository<Session>();

            var session = await sessionRepo.GetById(id, ct);

            if (session is null) return Result.NotFound("Session not Found.");

            if (session.StartDate <= DateTime.Now) return Result.Failed("Can't edit a session that already started.");

            var bookedCount = await unitOfWork.SessionRepository.GetCountOfBookedSlotAsync(id, ct);

            if (bookedCount > 0) return Result.Failed("Can't edit a session that has already been booked.");

            if (model.StartDate >= model.EndDate) return Result.ValidationFailed("End date must be after start date.");
            if (model.StartDate <= DateTime.Now) return Result.ValidationFailed("start date Have to be in the future.");

            var TrainerRepo = unitOfWork.GetRepository<Trainer>();
            var Trainer = await TrainerRepo.GetById(model.TrainerId, ct);

            if (Trainer is null) return Result.NotFound("Trainer not found.");

            session.UpdatedAt = DateTime.Now;

            mapper.Map(model, session);
            sessionRepo.update(session);

            var affectedRows = await unitOfWork.CompleteAsync();

            return affectedRows > 0 ? Result.Ok() : Result.Failed("Failed to create session.");

        }

       
    }
}
