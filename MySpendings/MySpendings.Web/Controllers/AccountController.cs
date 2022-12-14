using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySpendings.DataAccess.Repository.IRepository;
using MySpendings.Models;
using MySpendings.Models.ViewModels;
using System.Security.Claims;

namespace MySpendings.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AccountController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(model.Login);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("Login", "Incorrect Login or Password");
                ModelState.AddModelError("Password", "Incorrect Login or Password");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (model.Income < 0)
                ModelState.AddModelError("Income", "Income cannot be less than 0.");

            if (ModelState.IsValid)
            {
                var user = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.Login == model.Login);
                if (user == null)
                {
                    var newUser = new User()
                    {
                        Name = model.Name,
                        Login = model.Login,
                        Password = model.Password,
                        Email = model.Email,
                        Income = model.Income,
                        ImageUrl = "\\images\\profileImages\\blankImageProfile.png"
                    };

                    await _unitOfWork.User.AddAsync(newUser);
                    await _unitOfWork.SaveAsync();

                    await Authenticate(model.Login);
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("Login", "Incorrect Login or Password");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Settings()
        {
            var currentUser = await _unitOfWork.User
               .GetFirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            if (currentUser == null)
                return RedirectToAction("Login", controllerName: "Account");

            return View(currentUser);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(User user, IFormFile? file)
        {
            if (user.Income < 0)
                ModelState.AddModelError("Income", "Income cannot be less than 0.");

            if (ModelState.IsValid)
            {
                var wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var uploadsPath = Path.Combine(wwwRootPath, @"images\profileImages");
                    var extension = Path.GetExtension(file.FileName);

                    if (!string.IsNullOrWhiteSpace(user.ImageUrl) && user.ImageUrl != "\\images\\profileImages\\blankImageProfile.png")
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, user.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploadsPath, fileName + extension), FileMode.Create))
                        file.CopyTo(fileStreams);
                    user.ImageUrl = @"\images\profileImages\" + fileName + extension;
                }

                TempData["Success"] = "Profile updated seccessfully";
                _unitOfWork.User.Update(user);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Settings));
            }
            
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        #region Helpers Methods
        private async Task Authenticate(string userName)
        {
            
            var claims = new List<Claim>{ new Claim(ClaimsIdentity.DefaultNameClaimType, userName) };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        #endregion

        #region API CALLS
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            var user = await _unitOfWork.User.GetFirstOrDefaultAsync(p => p.Id == id);
            if (user == null)
                return Json(new { success = false, message = "Error while deleting" });

            var userOutlays = await _unitOfWork.UserOutlay.GetAllByAsync(x => x.UserId == user.Id, includeProperties: "Outlay");
            foreach (var userOutlay in userOutlays)
                _unitOfWork.Outlay.Remove(userOutlay.Outlay);

            var userCategories = await _unitOfWork.UserCategory.GetAllByAsync(x => x.UserId == user.Id, includeProperties: "Category");
            foreach (var userCategory in userCategories)
                _unitOfWork.Category.Remove(userCategory.Category);

            _unitOfWork.User.Remove(user);
            await _unitOfWork.SaveAsync();
            await Logout();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
