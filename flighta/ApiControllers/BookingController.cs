using Flighta.Data;
using Flighta.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Flighta.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BookingController : ControllerBase
    {
        public record BookingDetailsDto(int userid, int flightid);
        public record BookingDetails(int userid, int flightid, DateTime time,string? reason = "");
        private readonly IConfiguration _config;
        private readonly FlightDB _db;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IConfiguration config, FlightDB db, ILogger<BookingController> logger)
        {
            _config = config;
            _db = db;
            _logger = logger;
        }
        [HttpGet]
        public async Task<List<BookingDetails>> GetBookings()
        {
            _logger.LogInformation($"successfully returned all bookings for {HttpContext.Connection.RemoteIpAddress} at {DateTime.Now}");
            return await _db.GetBookings();
        }
        [HttpGet("{userid}")]
        [Authorize(Policy = "Admin")]
        public async Task<BookingDetails?> GetBooking(int userid)
        {
            _logger.LogInformation($"successfully returned user {userid} bookings for {HttpContext.Connection.RemoteIpAddress} at {DateTime.Now}");
            return await _db.GetBooking(userid);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BookFlight([FromBody] BookingDetailsDto bookingDetails)
        {
            try
            {
                bool result = await _db.BookFlight(bookingDetails);
                if (result) return Ok("Flight Booked!");
                _logger.LogWarning($"ip {HttpContext.Connection.RemoteIpAddress} Couldn't book flight for user {bookingDetails.userid} at flight {bookingDetails.flightid} at {DateTime.Now}");
                return BadRequest($"Couldn't book flight for user {bookingDetails.userid} at flight {bookingDetails.flightid}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"An error has occured while booking a flight at {DateTime.Now} for {HttpContext.Connection.RemoteIpAddress}");
                return BadRequest($"An error has occured while trying to book the flight {ex.Message}");
            }
        }
        [HttpDelete]
        [Authorize(Policy = "Admin",AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<bool> DeleteBooking([FromBody] BookingDetailsDto details)
        {
            _logger.LogInformation($"Successfully deleted booking {JsonConvert.SerializeObject(details)} at {DateTime.Now} for {HttpContext.Connection.RemoteIpAddress}");
            return await _db.DeleteBooking(details);
        }
    }
}
