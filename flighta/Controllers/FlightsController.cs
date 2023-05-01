using Flighta.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace flighta.Controllers
{
    public class FlightsController : Controller
    {
        private readonly FlightDB _db;

        public FlightsController(FlightDB db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var flights = await _db.GetFlights();
            return View(flights);
        }
    }
}
