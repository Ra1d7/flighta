using flighta.Helpers;
using Flighta.Data;
using Flighta.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace flighta.Controllers
{
    public class LoginController : Controller
    {
        private readonly FlightDB _db;
        private readonly ILogger<LoginController> _logger;

        public LoginController(FlightDB db, ILogger<LoginController> logger)
        {
            _db = db;
            _logger = logger;
        }
        public IActionResult Index()
        {
            _logger.MvcControllerLog("Login", "Index", DateTime.Now, HttpContext.User.Identity.Name);
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated)
            {
                string[] claimvalues = claimUser.Claims.Select(c => c.Value).ToArray();
                _logger.LogInformation($"{claimvalues[1]} {claimUser.Identity.Name} with id {claimvalues[2]} has logged in successfully at {DateTime.Now}");
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(VMLogin login)
        {
            _logger.MvcControllerLog("Login", "Index-Post", DateTime.Now, HttpContext.User.Identity.Name);
            if (!ModelState.IsValid)
            {
                TempData["error"] = "invalid credentials or fields";
                _logger.LogWarning($"{HttpContext.Connection.RemoteIpAddress} tried to login with invalid fields at {DateTime.Now} \n Current model is : {JsonConvert.SerializeObject(ModelState)}");
                return View();
            }
            bool result = await _db.LoginUser(login.Username, login.Password);
            var user = await _db.GetUserIdbyUsername(login.Username);
            if (!result)
            {
                _logger.LogWarning($"{HttpContext.Connection.RemoteIpAddress} tried to login with invalid credentials at {DateTime.Now}");
                TempData["error"] = "Wrong username or password";
                return View();
            }
            Roles role = await _db.GetRole(login.Username);
            List<Claim> claims = new List<Claim>() {
            new Claim(ClaimTypes.NameIdentifier,login.Username),
            new Claim(ClaimTypes.Role,role.ToString()),
            new Claim("userid",user.userId.ToString())
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = login.KeepLogged
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity), properties);
            TempData["success"] = $"Logged in as {login.Username}";
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Register()
        {
            _logger.MvcControllerLog("Login", "Register", DateTime.Now, HttpContext.User.Identity.Name);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            _logger.MvcControllerLog("Login", "Register-Post", DateTime.Now, HttpContext.User.Identity.Name);
            if (ModelState.IsValid)
            {
                await _db.CreateUser(user);
                TempData["success"] = "Successfully registered";
                _logger.LogInformation($"Ip {HttpContext.Connection.RemoteIpAddress} has registered with username : {user.Username} \n email: {user.Email}");
                return RedirectToAction("Index");
            }
            _logger.LogWarning($"Ip {HttpContext.Connection.RemoteIpAddress} tried to  register with invalid fields at {DateTime.Now} \n Current model is : {JsonConvert.SerializeObject(ModelState)}");
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            var username = HttpContext.User.Identity.Name;
            try
            {
            _logger.MvcControllerLog("Login", "Logout", DateTime.Now, HttpContext.User.Identity.Name);
            await HttpContext.SignOutAsync();
            if (HttpContext.User.Claims.Count() == 0)
            {
                ViewData["role"] = "";
                TempData["success"] = "Signed out!";
                _logger.LogInformation($"Successfully logged out user {username} at {DateTime.Now}");
            }
            }
            catch
            {
                _logger.LogWarning($"Couldn't logout user {username} at {DateTime.Now} \n Request data is {JsonConvert.SerializeObject(HttpContext.Request)}");
            }
            return RedirectToAction("Index");
        }
    }
}
