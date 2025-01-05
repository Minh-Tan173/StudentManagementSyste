using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Lib.Models;
using StudentManagementSystem.WEB.ViewModels;
using System.Threading.Tasks;

namespace StudentManagementSystem.WEB.Controllers {
    public class AuthController : Controller {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: AuthController/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login() {
            return View();
        }

        // POST: AuthController/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel loginVM, string returnUrl) {
            if (!ModelState.IsValid) {
                return View(loginVM);
            }

            var result = await _signInManager.PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RememberMe, false);

            if (result.Succeeded) {
                if (string.IsNullOrEmpty(returnUrl))
                    return RedirectToAction("Index", "Students");
                else
                    return LocalRedirect(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt!");

            return View(loginVM);
        }

        // POST: AuthController/Logout
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Students");
        }

        // GET: AuthController/Register
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register() {
            return View();
        }

        // POST: AuthController/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel registerVM) {
            if (!ModelState.IsValid) {
                return View(registerVM);
            }

            ApplicationUser user = new ApplicationUser {
                Email = registerVM.Email,
                UserName = registerVM.Email,
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerVM.Password);

            if (result.Succeeded) {
                return RedirectToAction("Login", "Auth");
            }

            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(registerVM);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AccessDenied() {
            return View();
        }
    }
}