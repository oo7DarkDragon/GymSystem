using GymSystem.BLL.Services.Interfaces;
using GymSystem.DAL.Context;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Classes;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymSystem.Controllers
{
    [Authorize]
    public class PlanController : Controller
    {
        
        private readonly IPlanServices _planServices;

        public PlanController(IPlanServices _planServices)
        {
            this._planServices = _planServices;
        }
      
        public async Task<IActionResult> Index(CancellationToken token)
        {
           var plans = await _planServices.GetAllPlansAsync(token);
            return View(plans);
        }

     public async Task<IActionResult> Details(int id, CancellationToken token)
        {
            var plan = await _planServices.GetPlanByIdAsync(id,token);

            if (plan == null)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(plan);

        }

    }

}
