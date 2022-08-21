using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                ModelState.AddModelError("IncorrectLoginOrPassword", "Incorrect Login or Password");
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
                        Email = model.Email
                    };

                    await _unitOfWork.User.AddAsync(newUser);
                    await _unitOfWork.SaveAsync();

                    await Authenticate(model.Login);
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("IncorrectLoginOrPassword", "Incorrect Login or Password");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private async Task Authenticate(string userName)
        {
            
            var claims = new List<Claim>{ new Claim(ClaimsIdentity.DefaultNameClaimType, userName) };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
