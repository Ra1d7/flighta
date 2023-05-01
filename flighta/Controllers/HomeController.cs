using flighta.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace flighta.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //var routeValues = new RouteValueDictionary {
            //                                  { "keyFillter","test" },
            //                                  { "valueFillter","test" }
            //                                };
            //return RedirectToAction("DisplayTable", "Dashboard", routeValues);
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}