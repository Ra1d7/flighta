using Flighta.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Flighta.ApiControllers.BookingController;

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int flightid)
        {
            var detailsList = await _db.GetBookings();
            TempData["success"] = "Booking has been deleted!";
            var userid = HttpContext.User.Claims.ToList()[2].Value;
            int.TryParse(userid, out var id);
            var DetailsDto = new BookingDetailsDto(id,flightid);
            await _db.DeleteBooking(DetailsDto);
            return RedirectToAction("Index");
        }
    }
}
