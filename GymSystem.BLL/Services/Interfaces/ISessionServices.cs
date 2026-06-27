using GymSystem.BLL.Common;
using GymSystem.BLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Interfaces
{
    public interface ISessionServices
    {
        Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct); 

        Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct);

        Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownMenuAsync(CancellationToken ct = default);
        Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownMenuAsync(CancellationToken ct = default);

        Task<SessionViewModel> GetSessionByIdAsync(int sessionId, CancellationToken ct);

        Task<UpdateSessionViewModel> GetSessionToUpdateByIdAsync(int sessionId, CancellationToken ct);

        Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default);

        Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct);
    }
}
