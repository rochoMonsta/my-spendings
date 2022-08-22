﻿using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> Index()
        {
            var currentUser = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            if (currentUser == null)
                return RedirectToAction("Login", controllerName: "Account");

            var outlays = await _unitOfWork.Outlay.GetAllByAsync(o => o.UserId == currentUser.Id, includeProperties: "Category");
            return View(outlays);
        }

        [Authorize]
        public async Task<IActionResult> Upsert(int? id)
        {
            var outlayViewModel = new OutlayViewModel()
            {
                Outlay = new Outlay() { CreatedDate = DateTimeOffset.Now }
            };

            UpdateMinMaxDate(outlayViewModel);
            await UpdateOutlayCategories(outlayViewModel);

            if (id == null || id == 0)
                return View(outlayViewModel);
            else
            {
                outlayViewModel.Outlay = await _unitOfWork.Outlay.GetFirstOrDefaultAsync(c => c.Id == id);
                return View(outlayViewModel);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(OutlayViewModel outlayViewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.Login == User.Identity.Name);

                if (currentUser == null)
                    return RedirectToAction("Login", controllerName: "Account");

                outlayViewModel.Outlay.UserId = currentUser.Id;

                TempData["Success"] = outlayViewModel.Outlay.Id == 0 ? "Category created seccessfully" : "Category edited seccessfully";

                if (outlayViewModel.Outlay.Id == 0)
                    await _unitOfWork.Outlay.AddAsync(outlayViewModel.Outlay);
                else
                    _unitOfWork.Outlay.Update(outlayViewModel.Outlay);

                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }

            UpdateMinMaxDate(outlayViewModel);
            await UpdateOutlayCategories(outlayViewModel);

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

        private async Task UpdateOutlayCategories(OutlayViewModel outlayViewModel)
        {
            var categories = await _unitOfWork.Category.GetAllAsync();
            outlayViewModel.Categories = categories.Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
        }
        #endregion
    }
}