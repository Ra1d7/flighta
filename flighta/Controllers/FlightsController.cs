using Flighta.Data;
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
        public async Task<IActionResult> Create()
        {
            return View();
        }
        //GET
        [HttpPost]
        [ValidateAntiForgeryToken]
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
    }
}
