using flighta.Helpers;
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
        private readonly ILogger<FlightsController> _logger;

        public FlightsController(FlightDB db,ILogger<FlightsController> logger)
        {
            _db = db;
            _logger = logger;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            _logger.MvcControllerLog("Flights", "Index", DateTime.Now, HttpContext.User.Identity.Name);
            if (HttpContext.User.Identity.IsAuthenticated)
            {

                int.TryParse(HttpContext.User.Claims.ToList()[2].Value, out int userid);
                var bookings = await _db.GetUserBookings(userid); //get all user bookings
                var allFlights = await _db.GetFlights(); //get all flights
                allFlights.RemoveAll(x => bookings.Any(y => y.flightid == x.flightId && y.reason.Length == 0)); //don't display flights that the user already booked
                _logger.LogInformation($"User {userid} viewed his avaliable flights which are {allFlights.Count}");
                return View(allFlights);
            }
            _logger.LogInformation($"An unauthenticated user with ip {HttpContext.Connection.RemoteIpAddress} tried to view the flights page at {DateTime.Now}");
            TempData["error"] = "You're not authenticated!";
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
            _logger.MvcControllerLog("Flights", "Create", DateTime.Now, HttpContext.User.Identity.Name);
            if (flight.flightTime < DateTime.Now)
            {
                ModelState.AddModelError("flightTime", "Flight time cannot be in the past!");
                _logger.LogWarning($"Admin {HttpContext.User.Identity.Name} tried to put an invalid flight time while creating a flight at {DateTime.Now}");
            }
            if (ModelState.IsValid)
            {
                await _db.AddFLight(flight);
                TempData["success"] = "Flight has been Created Successfully!";
                var flights = await _db.GetFlights();
                _logger.LogInformation($"Admin {HttpContext.User.Identity.Name} has successfully created a flight! Total flights are now {flights.Count}");
                return RedirectToAction("Index");
            }
            return View(flight);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? flightid)
        {
            _logger.MvcControllerLog("Flights", "Edit", DateTime.Now, HttpContext.User.Identity.Name);
            if (flightid == null || flightid == 0)
            {
                _logger.LogWarning($"Admin {HttpContext.User.Identity.Name} tried to edit a flight that doesn't exist! at {DateTime.Now}");
                return NotFound();
            }
            _logger.LogInformation($"Admin {HttpContext.User.Identity.Name} has edited flight {flightid} successfully at {DateTime.Now}");
            var flight = await _db.GetFlight((int)flightid);
            return View(flight);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? flightid)
        {
            _logger.MvcControllerLog("Flights", "Delete", DateTime.Now, HttpContext.User.Identity.Name);
            if (flightid == null || flightid == 0)
            {
                _logger.LogWarning($"Admin {HttpContext.User.Identity.Name} tried to Delete a flight that doesn't exist! at {DateTime.Now}");
                return NotFound();
            }
            _logger.LogWarning($"Found flight to delete for admin {HttpContext.User.Identity.Name} at {DateTime.Now}");
            var flight = await _db.GetFlight((int)flightid);
            return View(flight);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(FlightM flight)
        {
            _logger.MvcControllerLog("Flights", "Edit-Post", DateTime.Now, HttpContext.User.Identity.Name);
            if (flight.flightTime < DateTime.Now)
            {
                _logger.LogWarning($"Admin {HttpContext.User.Identity.Name} tried to put an invalid flight time while editing a flight at {DateTime.Now}");
                ModelState.AddModelError("flightTime", "Flight time cannot be in the past!");
            }
            if (ModelState.IsValid)
            {
                await _db.UpdateFlight(flight);
                TempData["success"] = $"Flight {flight.flightId} been Updated!";
                _logger.LogInformation($"Flight {flight.flightId} has been edited successfully by Admin {HttpContext.User.Identity.Name}");
                return RedirectToAction("Index");
            }
            _logger.LogError($"An error has occured while Admin {HttpContext.User.Identity.Name} was trying to edit flight {flight.flightId} at {DateTime.Now}");
            TempData["error"] = $"Couldn't Update {flight.flightId}";
            return View(flight);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(FlightM flight)
        {
            _logger.MvcControllerLog("Flights", "Delete-Post", DateTime.Now, HttpContext.User.Identity.Name);
            try
            {
                await _db.DeleteFlight(flight.flightId);
                _logger.LogInformation($"Flight {flight.flightId} has been deleted successfully by Admin {HttpContext.User.Identity.Name} at {DateTime.Now}");
                TempData["success"] = "Flight has been Deleted Successfully!";

            }
            catch (Exception ex)
            {
                _logger.LogError($"Couldn't Delete flight {flight.flightId} for Admin {HttpContext.User.Identity.Name} at {DateTime.Now}");
                TempData["error"] = $"Couldn't Delete {flight.flightId}\n{ex.Message}";
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<IActionResult> Book(int flightid)
        {
            _logger.MvcControllerLog("Flights", "Book", DateTime.Now, HttpContext.User.Identity.Name);
            int.TryParse(HttpContext.User.Claims.ToList()[2].Value, out int userid);
            var details = new BookingDetailsDto(userid, flightid);
            await _db.BookFlight(details);
            _logger.LogInformation($"Successfully booked flight {flightid} for user {userid} at {DateTime.Now}");
            TempData["success"] = $"Flight {details.flightid} booked for user {details.userid}";
            return RedirectToAction("Index");
        }
    }
}
