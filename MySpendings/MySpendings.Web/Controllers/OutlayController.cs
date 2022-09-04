using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySpendings.DataAccess.Repository.IRepository;
using MySpendings.Models;
using MySpendings.Models.ViewModels;

namespace MySpendings.Web.Controllers
{
    public class OutlayController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OutlayController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Upsert(int? id)
        {
            var currentUser = await _unitOfWork.User
                .GetFirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            if (currentUser == null)
                return RedirectToAction("Login", controllerName: "Account");

            var outlayViewModel = new OutlayViewModel()
            {
                Outlay = new Outlay() { CreatedDate = DateTimeOffset.Now }
            };

            UpdateMinMaxDate(outlayViewModel);
            await UpdateOutlayCategories(outlayViewModel, currentUser.Id);

            if (id == null || id == 0)
                return View(outlayViewModel);
            else
            {
                outlayViewModel.Outlay = await _unitOfWork.Outlay
                    .GetFirstOrDefaultAsync(c => c.Id == id);
                return View(outlayViewModel);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(OutlayViewModel outlayViewModel)
        {
            var currentUser = await _unitOfWork.User
                .GetFirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            if (currentUser == null)
                return RedirectToAction("Login", controllerName: "Account");

            if (ModelState.IsValid)
            {
                TempData["Success"] = outlayViewModel.Outlay.Id == 0 ? "Outlay created seccessfully" : "Outlay edited seccessfully";

                if (outlayViewModel.Outlay.Id == 0)
                {
                    await _unitOfWork.Outlay.AddAsync(outlayViewModel.Outlay);
                    await _unitOfWork.SaveAsync();
                    await _unitOfWork.UserOutlay
                        .AddAsync(new UserOutlay() { UserId = currentUser.Id, OutlayId = outlayViewModel.Outlay.Id });
                }
                else
                    _unitOfWork.Outlay.Update(outlayViewModel.Outlay);

                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }

            UpdateMinMaxDate(outlayViewModel);
            await UpdateOutlayCategories(outlayViewModel, currentUser.Id);

            return View(outlayViewModel);
        }

        #region Helpers Methods
        private void UpdateMinMaxDate(OutlayViewModel outlayViewModel)
        {
            var currentDate = DateTime.Now;
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            outlayViewModel.MinDate = $"{firstDayOfMonth.Year}-{firstDayOfMonth.Month.ToString("D2")}-{firstDayOfMonth.Day.ToString("D2")}";
            outlayViewModel.MaxDate = $"{lastDayOfMonth.Year}-{lastDayOfMonth.Month.ToString("D2")}-{lastDayOfMonth.Day.ToString("D2")}";
        }

        private async Task UpdateOutlayCategories(OutlayViewModel outlayViewModel, int userId)
        {
            var userCategories = await _unitOfWork.UserCategory
                .GetAllByAsync(x => x.UserId == userId, includeProperties: "Category");

            var categories = userCategories.Select(x => x.Category);

            outlayViewModel.Categories = categories
                .Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
        }
        #endregion

        #region API CALLS
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            var outlay = await _unitOfWork.Outlay.GetFirstOrDefaultAsync(o => o.Id == id);
            if (outlay == null)
                return Json(new { success = false, message = "Error while deleting" });

            _unitOfWork.Outlay.Remove(outlay);
            await _unitOfWork.SaveAsync();
            return Json(new { success = true, message = "Delete Successful" });
        }

        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var currentUser = await _unitOfWork.User
                .GetFirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            if (currentUser == null)
                return RedirectToAction("Login", controllerName: "Account");

            var userOutlays = await _unitOfWork.UserOutlay
                .GetAllByAsync(x => x.UserId == currentUser.Id, includeProperties: "Outlay");

            List<Outlay> outlays = new List<Outlay>();
            foreach (var userOutlay in userOutlays)
                outlays.Add(await _unitOfWork.Outlay.GetFirstOrDefaultAsync(c => c.Id == userOutlay.OutlayId, includeProperties: "Category"));
            return Json(new { data = outlays });
        }
        #endregion
    }
}
