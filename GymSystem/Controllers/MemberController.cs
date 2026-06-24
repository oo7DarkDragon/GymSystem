using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.MembersViewModel;
using Microsoft.AspNetCore.Mvc;

namespace GymSystem.Controllers
{
    public class MemberController : Controller
    {
        private readonly IMemberServices memberServices;

        public MemberController(IMemberServices memberServices)
        {
            this.memberServices = memberServices;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {

            var members = await memberServices.GetAllMembersAsync(ct);
            return View(members);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) 
            {
                return View(nameof(Create),model);
            }

            await memberServices.CreateMemberAsync(model, ct);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var member = await memberServices.GetMemberDetailsAsync(id, ct);
            if (member is null)
            {
              TempData["ErrorMessage"] = $"Member with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [HttpGet]
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var healthRecord = await memberServices.GetMemberHealthRecordAsync(id, ct);
            if (healthRecord is null)
            {
                TempData["ErrorMessage"] = $"Health record for member with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(healthRecord);
        }

    }
}
