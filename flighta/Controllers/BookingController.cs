using Flighta.DataAccess;
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
        public async Task<IActionResult> Delete(BookingDetails details)
        {
            var detailsList = await _db.GetBookings();
            TempData["success"] = "Booking has been deleted!";
            var DetailsDto = new BookingDetailsDto(details.userid,details.flightid);
            await _db.DeleteBooking(DetailsDto);
            return RedirectToAction("Index");
        }
    }
}
