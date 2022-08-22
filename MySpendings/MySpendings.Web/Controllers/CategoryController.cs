using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySpendings.DataAccess.Repository.IRepository;
using MySpendings.Models;

namespace MySpendings.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _unitOfWork.User
                .GetFirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            if (currentUser == null)
                return RedirectToAction("Login", controllerName: "Account");

            var userCategories = await _unitOfWork.UserCategory
                .GetAllByAsync(x => x.UserId == currentUser.Id, includeProperties: "Category");

            var categories = userCategories.Select(x => x.Category);
            return View(categories);
        }

        [Authorize]
        public async Task<IActionResult> Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                var category = new Category();
                return View(category);
            }
            else
            {
                var category = await _unitOfWork.Category
                    .GetFirstOrDefaultAsync(c => c.Id == id);
                return View(category);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _unitOfWork.User
                    .GetFirstOrDefaultAsync(u => u.Login == User.Identity.Name);
                if (currentUser == null)
                    return RedirectToAction("Login", controllerName: "Account");

                TempData["Success"] = category.Id == 0 ? "Category created seccessfully" : "Category edited seccessfully";

                if (category.Id == 0)
                {
                    await _unitOfWork.Category.AddAsync(category);
                    await _unitOfWork.SaveAsync();
                    await _unitOfWork.UserCategory
                        .AddAsync(new UserCategory() { CategoryId = category.Id, UserId = currentUser.Id });
                }
                else
                    _unitOfWork.Category.Update(category);

                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        #region API CALLS
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            var category = await _unitOfWork.Category.GetFirstOrDefaultAsync(p => p.Id == id);
            if (category == null)
                return Json(new { success = false, message = "Error while deleting" });

            _unitOfWork.Category.Remove(category);
            await _unitOfWork.SaveAsync();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
