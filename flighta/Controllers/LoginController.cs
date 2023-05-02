﻿using Flighta.Data;
using Flighta.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace flighta.Controllers
{
    public class LoginController : Controller
    {
        private readonly FlightDB _db;

        public LoginController(FlightDB db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if(claimUser.Identity.IsAuthenticated )
            {
                string[] claimvalues = claimUser.Claims.Select(c => c.Value).ToArray();
                TempData["error"] = $"Already Logged in! as {claimvalues[0]}\nwhich is an {claimvalues[1]}";
                return RedirectToAction("Index","Home");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(VMLogin login)
        {
            bool result = await _db.LoginUser(login.Username,login.Password);
            if (!result)
            {
                TempData["error"] = "Wrong username or password";
                return View();
            }
            Roles role = await _db.GetRole(login.Username);
            List<Claim> claims = new List<Claim>() {
            new Claim(ClaimTypes.NameIdentifier,login.Username),
            new Claim(ClaimTypes.Role,role.ToString())
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties() { 
            AllowRefresh = true,
            IsPersistent = login.KeepLogged     
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),properties);
            return RedirectToAction("Index","Home");
        }
    }
}