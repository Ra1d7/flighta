using Flighta.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace flighta.Controllers
{
    public class BookingController : Controller
    {
        private readonly FlightDB _db;

        public BookingController(FlightDB db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var detailsList = await _db.GetBookings();
            return View(detailsList);
        }
    }
}
