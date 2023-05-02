using Flighta.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace flighta.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly FlightDB _db;

        public UsersController(FlightDB db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _db.GetUsers();
            return View(users);
        }
    }
}
