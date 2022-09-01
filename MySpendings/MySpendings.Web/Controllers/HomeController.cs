using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySpendings.DataAccess.Repository.IRepository;
using MySpendings.Models;
using MySpendings.Models.ViewModels;
using System.Diagnostics;
using System.Globalization;

namespace MySpendings.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
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

            var outlays = await GetAllUserOutlaysAsync(currentUser.Id);
            var currentMonthOutlays = GetUserOutlaysByDate(outlays, DateTime.Now.Year, DateTime.Now.Month);

            var categories = await GetUserCategoriesAsync(currentUser.Id);

            return View(new OutlayChartViewModel() 
            { 
                CurrentMonthCategoryOutlays = GetCategorySpendingsDictionary(currentMonthOutlays),
                OutlaysData = DateTime.Now.ToString("yyyy MMMM"),
                CurrentMonthOutlays = currentMonthOutlays.ToList(),
                MonthList = GetMonthList(outlays).ToList(),
                YearsList = GetYearsList(outlays).ToList(),
                CategoryStatuses = GetCategoryStatuses(currentMonthOutlays, categories).ToList(),
                SelectedMonth = DateTime.Now.Month,
                SelectedYear = DateTime.Now.Year,
                User = currentUser
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(OutlayChartViewModel filter)
        {
            var currentUser = await _unitOfWork.User
                .GetFirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            if (currentUser == null)
                return RedirectToAction("Login", controllerName: "Account");

            var outlays = await GetAllUserOutlaysAsync(currentUser.Id);
            var currentMonthUserOutlays = GetUserOutlaysByDate(outlays, filter.SelectedYear, filter.SelectedMonth);

            List<Outlay> currentMonthOutlays = new List<Outlay>();
            foreach (var userOutlay in currentMonthUserOutlays)
            {
                if (filter.CategoryStatuses.Where(x => x.IsActive).Any(o => o.CategoryId == userOutlay.CategoryId))
                    currentMonthOutlays.Add(userOutlay);
            }

            var categories = await GetUserCategoriesAsync(currentUser.Id);

            return View(new OutlayChartViewModel()
            {
                CurrentMonthCategoryOutlays = GetCategorySpendingsDictionary(currentMonthOutlays),
                OutlaysData = new DateTime(filter.SelectedYear, filter.SelectedMonth, 1).ToString("yyyy MMMM"),
                CurrentMonthOutlays = currentMonthOutlays.ToList(),
                MonthList = GetMonthList(outlays).ToList(),
                YearsList = GetYearsList(outlays).ToList(),
                CategoryStatuses = GetCategoryStatuses(currentMonthOutlays, categories).ToList(),
                SelectedMonth = filter.SelectedMonth,
                SelectedYear = filter.SelectedYear,
                User = currentUser
            });
        }

        [Authorize]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Helpers Methods
        private async Task<List<Outlay>> GetAllUserOutlaysAsync(int userId)
        {
            var userOutlays = await _unitOfWork.UserOutlay
               .GetAllByAsync(x => x.UserId == userId, includeProperties: "Outlay");

            List<Outlay> outlays = new List<Outlay>();
            foreach (var userOutlay in userOutlays)
                outlays.Add(await _unitOfWork.Outlay.GetFirstOrDefaultAsync(c => c.Id == userOutlay.OutlayId, includeProperties: "Category"));

            return outlays;
        }

        private List<Outlay> GetUserOutlaysByDate(List<Outlay> userOutlays, int year, int month)
        {
            return userOutlays.Where(x => x.CreatedDate.Month == month && x.CreatedDate.Year == year).ToList();
        }

        private async Task<List<Category>> GetUserCategoriesAsync(int userId)
        {
            var userCategories = await _unitOfWork.UserCategory.GetAllByAsync(x => x.UserId == userId, includeProperties: "Category");
            return userCategories.Select(x => x.Category).ToList();
        }

        private Dictionary<string, float> GetCategorySpendingsDictionary(IEnumerable<Outlay> outlays)
        {
            var categorySpendings = outlays
                .GroupBy(x => x.Category)
                .Select(c => new { 
                    CategoryName = c.Key, 
                    SpendingsSum = c.Sum(o => o.Cost) })
                .ToDictionary(d => d.CategoryName.Name, d => d.SpendingsSum);

            return categorySpendings;
        }

        private IEnumerable<SelectListItem> GetMonthList(IEnumerable<Outlay> outlays)
        {
            return outlays
                .GroupBy(o => o.CreatedDate.Date.Month)
                .Select(x => new SelectListItem() { 
                    Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Key), 
                    Value = x.Key.ToString() 
                });
        }

        private IEnumerable<SelectListItem> GetYearsList(IEnumerable<Outlay> outlays)
        {
            return outlays
                .GroupBy(o => o.CreatedDate.Year)
                .Select(x => new SelectListItem() { 
                    Text = x.Key.ToString(), 
                    Value = x.Key.ToString() 
                });
        }

        private IEnumerable<CategoryStatusViewModel> GetCategoryStatuses(IEnumerable<Outlay> outlays, IEnumerable<Category> categories)
        {
            var categoryStatuses = new List<CategoryStatusViewModel>();
            foreach (var category in categories)
            {
                var categoryStatus = new CategoryStatusViewModel() { CategoryId = category.Id, CategoryName = category.Name};

                if (outlays.Any(o => o.CategoryId == category.Id))
                    categoryStatus.IsActive = true;
                else
                    categoryStatus.IsActive = false;

                categoryStatuses.Add(categoryStatus);

            }
            return categoryStatuses;
        }
        #endregion
    }
}