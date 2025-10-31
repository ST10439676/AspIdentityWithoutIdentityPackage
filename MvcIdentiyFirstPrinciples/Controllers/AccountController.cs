using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MvcIdentiyFirstPrinciples.Models;
using MvcIdentiyFirstPrinciples.Services;
using MvcIdentiyFirstPrinciples.ViewModels;
using User = MvcIdentiyFirstPrinciples.Models.User;

namespace MvcIdentiyFirstPrinciples.Controllers
{
    public class AccountController : Controller
    {

        private UserDb _userDb;
        private ILogger<AccountController> _logger;
        private IPasswordHasher _passwordHasher;

        public AccountController(UserDb userDb, ILogger<AccountController> logger, IPasswordHasher passwordHasher)
        {
            _userDb = userDb;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                (string passwordHash, string salt) = _passwordHasher.GenerateHash(register.Password);
                var user = new User()
                {
                    Username = register.Username,
                    Email = register.Email,
                    PasswordHash = passwordHash,
                    Salt = salt,
                    Role = RoleDisplayName.GetDisplayName(register.Role)
                };
                _userDb.AddUser(user);
            }
            return View(register);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Searching for user {0}", login.Username);
                User? dbUser = _userDb.GetUserByName(login.Username);
                if (dbUser is not null)
                {
                    _logger.LogInformation("Found user: {0}", dbUser.Username);
                }
                if (dbUser is not null && _passwordHasher.VerifyPassword(login.Password, dbUser.PasswordHash, dbUser.Salt))
                {
                    return SignIn(new ClaimsPrincipal(Models.User.CreateClaimIdentity(dbUser)), CookieAuthenticationDefaults.AuthenticationScheme);
                }
                ModelState.AddModelError(nameof(login.Username), "Invalid Username or Password");
                ModelState.AddModelError(nameof(login.Password), "Invalid Username or Password");
                return View(login);

            }
            return View(login);
        }

        [HttpGet, Authorize]
        public IActionResult Logout()
        {
            return View();
        }

        [HttpPost, Authorize]
        public IActionResult Logout([FromForm(Name = "yes")] string? yes, [FromForm(Name = "no")] string? no, [FromQuery(Name = "ReturnUrl")] string? ReturnUrl)
        {
            _logger.LogInformation("Logging out");
            _logger.LogInformation("yes: {0}, no: {1}", yes ?? "'null'", no ?? "'null");
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Checking for yes or no in loggout");
                if (yes is not null && no is null)
                {
                    _logger.LogInformation("Siging out");
                    return SignOut(new AuthenticationProperties() { RedirectUri = Url.Action("Dashboard", "Home") });
                }
                if (no is not null && Url.IsLocalUrl(ReturnUrl))
                {
                    _logger.LogInformation("Not Signing out return to {0}", ReturnUrl);
                    return LocalRedirect(ReturnUrl);
                }
            }
            _logger.LogInformation("Could not logout and did not know where to return to");
            return RedirectToAction("Dashboard", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
