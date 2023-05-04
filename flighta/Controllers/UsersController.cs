using flighta.Helpers;
using Flighta.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace flighta.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly FlightDB _db;
        private readonly ILogger<UsersController> _logger;

        public UsersController(FlightDB db,ILogger<UsersController> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            _logger.MvcControllerLog("Users", "Index", DateTime.Now, HttpContext.User.Identity.Name);
            var users = await _db.GetUsers();
            return View(users);
        }
    }
}
