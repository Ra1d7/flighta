using Flighta.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Flighta.ApiControllers.BookingController;

namespace flighta.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly FlightDB _db;

        public BookingController(FlightDB db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            if(HttpContext.User.Identity.IsAuthenticated)
            {
            var claims = HttpContext.User.Claims.ToList();
            if (claims[1].Value == "Client")
            {
                int.TryParse(claims[2].Value, out int id);
                return View(await _db.GetUserBookings(id));
            }else if (claims[1].Value == "Admin")
            {
                return View(await _db.GetBookings());
            }
            }
            TempData["error"] = "Please login to see your bookings";
            return View(new List<BookingDetails>());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(string details)
        {
            string reason = Request.Form["reason"];
            var obj = JsonConvert.DeserializeObject<BookingDetails>(details);
            int.TryParse(HttpContext.User.Claims.ToList()[2].Value, out var id);
            bool isAdmin = HttpContext.User.IsInRole("Admin");
            if (reason is null || reason == "") reason = "Deleted by user";
            reason = isAdmin ? "Deleted by admin" : reason;
            if(id == obj.userid || isAdmin)
            {
            TempData["success"] = "Booking has been deleted!";
            var DetailsDto = new BookingDetailsDto(obj.userid,obj.flightid);
            await _db.DeleteBookingWithReason(DetailsDto,reason);
            }
            else
            {
                TempData["error"] = "you don't have permissions to delete this booking";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
