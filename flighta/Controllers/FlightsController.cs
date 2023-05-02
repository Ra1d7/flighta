using Flighta.Data;
using Flighta.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Flighta.ApiControllers.BookingController;

namespace flighta.Controllers
{
    public class FlightsController : Controller
    {
        private readonly FlightDB _db;

        public FlightsController(FlightDB db)
        {
            _db = db;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var claims = HttpContext.User.Claims.ToList();
            if (claims[0].Value is not null)
            {

            var user = await _db.GetUserIdbyUsername(claims[0].Value);
            var bookings = await _db.GetUserBookings(user.userId); //get all user bookings
            var allFlights = await _db.GetFlights(); //get all flights
            allFlights.RemoveAll(x => bookings.Any(y => y.flightid == x.flightId)); //don't display flights that the user already booked
            return View(allFlights);
            }
            TempData["error"] = "user has no claims";
            return View(new List<FlightM>());
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        //GET
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(FlightM flight)
        {
            if (flight.flightTime < DateTime.Now)
            {
                ModelState.AddModelError("flightTime", "Flight time cannot be in the past!");
            }
            if (ModelState.IsValid)
            {
                await _db.AddFLight(flight);
                TempData["success"] = "Flight has been Created Successfully!";
                return RedirectToAction("Index");
            }
            return View(flight);
        }
        public async Task<IActionResult> Edit(int? flightid)
        {
            if (flightid == null || flightid == 0)
            {
                return NotFound();
            }
            var flight = await _db.GetFlight((int)flightid);
            return View(flight);
        }
        public async Task<IActionResult> Delete(int? flightid)
        {
            if (flightid == null || flightid == 0)
            {
                return NotFound();
            }
            var flight = await _db.GetFlight((int)flightid);
            return View(flight);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(FlightM flight)
        {
            if (flight.flightTime < DateTime.Now)
            {
                ModelState.AddModelError("flightTime", "Flight time cannot be in the past!");
            }
            if (ModelState.IsValid)
            {
                await _db.UpdateFlight(flight);
                TempData["success"] = $"Flight {flight.flightId} been Updated!";
                return RedirectToAction("Index");
            }
            TempData["error"] = $"Couldn't Update {flight.flightId}";
            return View(flight);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(FlightM flight)
        {
            try
            {
            await _db.DeleteFlight(flight.flightId);
            TempData["success"] = "Flight has been Deleted Successfully!";

            }
            catch (Exception ex)
            {
                TempData["error"] = $"Couldn't Update {flight.flightId}\n{ex.Message}";
            }
            return RedirectToAction("Index");
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Book(int flightid)
        {
            int.TryParse(HttpContext.User.Claims.ToList()[2].Value, out int userid);
            var details = new BookingDetailsDto(userid, flightid);
            await _db.BookFlight(details);
            TempData["success"] = $"Flight {details.flightid} booked for user {details.userid}";
            return RedirectToAction("Index");
        }
    }
}
