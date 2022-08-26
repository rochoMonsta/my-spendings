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

            var userOutlays = await _unitOfWork.UserOutlay
                .GetAllByAsync(x => x.UserId == currentUser.Id, includeProperties: "Outlay");

            List<Outlay> outlays = new List<Outlay>();
            foreach (var userOutlay in userOutlays)
                outlays.Add(await _unitOfWork.Outlay.GetFirstOrDefaultAsync(c => c.Id == userOutlay.OutlayId, includeProperties: "Category"));

            var currentMonthOutlays = outlays.Where(o => 
                o.CreatedDate.DateTime.Month == DateTime.Now.Month && 
                o.CreatedDate.DateTime.Year == DateTime.Now.Year);

            return View(new OutlayChartViewModel() 
            { 
                CurrentMonthCategoryOutlays = GetCategorySpendingsDictionary(currentMonthOutlays),
                OutlaysData = DateTime.Now.ToString("yyyy MMMM"),
                CurrentMonthOutlays = currentMonthOutlays,
                MonthList = GetMonthList(outlays),
                YearsList = GetYearsList(outlays),
                CategoryStatuses = GetCategoryStatuses(currentMonthOutlays),
                SelectedMonth = DateTime.Now.Month,
                SelectedYear = DateTime.Now.Year,
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(OutlayChartViewModel outlayChartViewModel)
        {
            return null;
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Helpers Methods
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

        private IEnumerable<CategoryStatusViewModel> GetCategoryStatuses(IEnumerable<Outlay> outlays)
        {
            return outlays
                .GroupBy(x => x.Category)
                .Select(c => new CategoryStatusViewModel() { 
                    Category = c.Key, 
                    IsActive = true 
                });
        }
        #endregion
    }
}