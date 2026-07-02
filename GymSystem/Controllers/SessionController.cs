using GymSystem.BLL.Common;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;

namespace GymSystem.Controllers
{
    [Authorize]
    public class SessionController : Controller
    {
        private readonly ISessionServices _sessionServices;

        public SessionController(ISessionServices sessionServices)
        {
            _sessionServices = sessionServices;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions = await _sessionServices.GetAllSessionsAsync(ct);
            return View(sessions);
        }

        public  async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
        {
            await PopulateDropDownAsync(ct);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSession(CreateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownAsync(ct);
                return View(model);
            }
            var result = await _sessionServices.CreateSessionAsync(model, ct);
            if (result.Success)
            {
             TempData["SuccessMessage"] = "Session created successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.Error;
            await PopulateDropDownAsync(ct);
            return View(model);
        }

        private async Task PopulateDropDownAsync(CancellationToken ct)
        {
            ViewBag.Trainers = new SelectList(await _sessionServices.GetTrainersForDropDownMenuAsync(ct), "Id", "Name");
            ViewBag.Categories = new SelectList(await _sessionServices.GetCategoriesForDropDownMenuAsync(ct), "Id", "CategoryName");
           
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var session = await _sessionServices.GetSessionByIdAsync(id, ct);
            if (session is null)
            {
                TempData["ErrorMessage"] = $"Session not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        [HttpGet]
        public async Task<IActionResult> Edit (int id, CancellationToken ct)
        {
            var session = await _sessionServices.GetSessionToUpdateByIdAsync(id, ct);
            if(session is null)
            {
                TempData["ErrorMessage"] = "Session can not be edited, it's not found";
                return RedirectToAction(nameof (Index));
            }
            await PopulateDropDownAsync (ct);
            return View(session);
             
        }

        [HttpPost]
        public async Task<IActionResult> Edit (int id,UpdateSessionViewModel model,  CancellationToken ct)
        {
            if(!ModelState.IsValid)
            {
                await PopulateDropDownAsync(ct);
                return View(model);
            }

            var result = await _sessionServices.UpdateSessionAsync(id, model, ct);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Session updated Correctly";
                return RedirectToAction (nameof(Index));
            }

            TempData["ErrorMessage"] = "Failed to update session";

            await PopulateDropDownAsync(ct);
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var session = await _sessionServices.GetSessionByIdAsync(id, ct);
            if(session is null)
            {
                TempData["ErrorMessage"] = "Session not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _sessionServices.RemoveSessionAsync(id, ct);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Session deleted successfully" : "Failed to delete Session";

            return RedirectToAction(nameof(Index));
        }


    }
}
