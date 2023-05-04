using flighta.Helpers;
using flighta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace flighta.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.MvcControllerLog("Home", "Index", DateTime.Now, HttpContext.User.Identity.Name);
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            
            _logger.LogError($"An error has occured with serving page {HttpContext.Request.Path} to {HttpContext.Connection.RemoteIpAddress} at {DateTime.Now}\nHttp request data : {JsonConvert.SerializeObject(HttpContext.Request)}");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}