using System.Threading.Tasks;
using FlexSearch.Panel.Helpers;
using FlexSearch.Panel.Models.ViewModels;
using FlexSearch.Panel.Services.Contract;
using Microsoft.AspNetCore.Mvc;

namespace FlexSearch.Panel.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _authService.SendAuthData(model))
                {
                    return RedirectToAction("Index", "Home", model);
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            UserHelper.CurrentUser = null;
            return RedirectToAction("Login");
        }
    }
}