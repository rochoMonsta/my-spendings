using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySpendings.DataAccess.Repository.IRepository;
using MySpendings.Models;
using MySpendings.Models.ViewModels;
using System.Diagnostics;

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

            var categorySpendings = GetCategorySpendingsDictionary(outlays);

            return View(new OutlayChartViewModel() 
            { 
                CategoryOutlays = categorySpendings, 
                OutlaysData = DateTime.Now.ToString("yyyy MMMM"),
                Outlays = outlays
            });
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
        #endregion
    }
}