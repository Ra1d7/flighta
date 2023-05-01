using Flighta.DataAccess;
using Microsoft.AspNetCore.Mvc;

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
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
    }
}
