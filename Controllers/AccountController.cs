using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Models;
using WeatherApp.ViewModels;
using System.Security.Claims;
using WeatherApp.Services;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using BC = BCrypt.Net.BCrypt;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly WeatherAppDbContext _context;
        private readonly IAccountService _accountService;

        public AccountController(WeatherAppDbContext context,
            IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginModel? model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);

                if (user != null && BC.EnhancedVerify(model.Password, user!.Password, HashType.SHA512))
                {
                    await Authenticate(model.UserName);

                    // Set authentication cookie
                    Response.Cookies.Append("UserId", user.Id.ToString(), new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(30) // Cookie expires in 30 days
                    });

                    if (model.RememberMe)
                    {
                        // Set authentication cookie
                        Response.Cookies.Append("IsAuthenticated", "true", new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddDays(30) // Cookie expires in 30 days
                        });
                    }

                    return RedirectToAction("SearchCity", "Forecast");
                }

                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
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
        public async Task<IActionResult> Register([FromForm] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);

                if (user == null)
                {
                    _context.Users.Add(new User
                    {
                        UserName = model.UserName,
                        Password = BC.EnhancedHashPassword(model.Password, 13, HashType.SHA512),
                        Email = model.Email,
                    });

                    await _context.SaveChangesAsync();

                    await Authenticate(model.UserName);

                    var newUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);

                    // Set authentication cookie
                    Response.Cookies.Append("UserId", newUser.Id.ToString(), new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(30) // Cookie expires in 30 days
                    });

                    if (model.RememberMe)
                    {
                        // Set authentication cookie
                        Response.Cookies.Append("IsAuthenticated", "true", new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddDays(30) // Cookie expires in 30 days
                        });
                    }

                    return RedirectToAction("SearchCity", "Forecast");
                }
                else
                {
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                }
            }

            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };

            ClaimsIdentity id = new(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("IsAuthenticated");
            Response.Cookies.Delete("UserId");
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string login, string oldPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("confirmPassword", "New password and confirm password do not match.");
                return View(); // Return the view with validation errors
            }

            // Call the service method only if passwords match
            var statusCode = await _accountService.ChangePasswordAsync(login, oldPassword, newPassword);

            if (statusCode == 200)
            {
                // Password change was successful
                TempData["SuccessMessage"] = "Password changed successfully.";
                return RedirectToAction("SearchCity", "Forecast"); // Redirect to a confirmation page or any other desired page
            }
            else if (statusCode == 400)
            {
                // Password change failed due to incorrect old password
                ModelState.AddModelError("", "Incorrect old password."); // Add error message to model state
                return View(); // Return the view with validation errors
            }
            else
            {
                // Password change failed due to an unexpected error
                TempData["ErrorMessage"] = "An unexpected error occurred while changing the password.";
                return RedirectToAction("SearchCity", "Forecast"); // Redirect to an error page or any other desired page
            }
        }

        [HttpGet]
        public IActionResult ChangeEmail()
        {
            return View();
        }

        public async Task<IActionResult> ChangeEmail(string login, string email)
        {
            
            var statusCode = await _accountService.ChangeEmailAsync(login, email);

            if (statusCode == 200)
            {
                // Password change was successful
                TempData["SuccessMessage"] = "Email changed successfully.";
                return RedirectToAction("SearchCity", "Forecast"); // Redirect to a confirmation page or any other desired page
            }
            else if (statusCode == 400)
            {
                // Password change failed due to incorrect old password
                ModelState.AddModelError("", "This email is already in use."); // Add error message to model state
                return View(); // Return the view with validation errors
            }
            else
            {
                // Password change failed due to an unexpected error
                TempData["ErrorMessage"] = "An unexpected error occurred while changing the email.";
                return RedirectToAction("SearchCity", "Forecast"); // Redirect to an error page or any other desired page
            }
        }

        [HttpGet("GetNotifications/{userId:int}")]
        public IActionResult GetNotifications(int userId)
        {
            var user = new User()
            {
                Id = userId
            };

            return View(user);
        }

        [HttpPost("GetNotifications/{userId:int}")]
        public async Task<IActionResult> GetNotifications(int userId, string city, bool flag)
        {
            if (flag) await _accountService.EnableSubscriptionAsync(userId, city);
            else await _accountService.DisableSubscriptionAsync(userId, city);

            return RedirectToAction("SearchCity", "Forecast");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            return await _accountService.ResetPasswordAsync(email);
        }
    }
}
