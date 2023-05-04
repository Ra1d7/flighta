using flighta.Helpers;
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
        private readonly ILogger<BookingController> _logger;

        public BookingController(FlightDB db, ILogger<BookingController> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            _logger.MvcControllerLog("Booking", "Index", DateTime.Now, HttpContext.User.Identity.Name);
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var claims = HttpContext.User.Claims.ToList();
                if (HttpContext.User.IsInRole("Client"))
                {
                    int.TryParse(claims[2].Value, out int id);
                    _logger.LogInformation($"Successfully got bookings for user {id} at {DateTime.Now}");
                    return View(await _db.GetUserBookings(id));
                }
                else if (HttpContext.User.IsInRole("Admin"))
                {
                    _logger.LogInformation($"Successfully got bookings for Admin {HttpContext.User.Identity.Name} at {DateTime.Now}");
                    return View(await _db.GetBookings());
                }
            }
            _logger.LogWarning($"An unauthenticated user with ip {HttpContext.Connection.RemoteIpAddress} tried to view bookings at {DateTime.Now}");
            TempData["error"] = "Please login to see your bookings";
            return View(new List<BookingDetails>());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(string details)
        {
            _logger.MvcControllerLog("Booking", "Delete", DateTime.Now, HttpContext.User.Identity.Name);
            string reason = Request.Form["reason"];
            var obj = JsonConvert.DeserializeObject<BookingDetails>(details);
            int.TryParse(HttpContext.User.Claims.ToList()[2].Value, out var id);
            bool isAdmin = HttpContext.User.IsInRole("Admin");
            if (reason is null || reason == "") reason = "Deleted by user";
            reason = isAdmin ? "Deleted by admin" : reason;
            if (id == obj.userid || isAdmin)
            {
                TempData["success"] = "Booking has been deleted!";
                var DetailsDto = new BookingDetailsDto(obj.userid, obj.flightid);
                _logger.LogInformation($"User {obj.userid} deleted flight {obj.flightid} at {DateTime.Now}");
                await _db.DeleteBookingWithReason(DetailsDto, reason);
            }
            else
            {
                TempData["error"] = "you don't have permissions to delete this booking";
                _logger.LogWarning($"User {obj.userid} tried to delete flight {obj.flightid} at {DateTime.Now}");
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
