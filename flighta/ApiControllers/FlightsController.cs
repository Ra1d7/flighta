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
    public class FlightsController : ControllerBase
    {
        public record CreateFlightDto(string from, string to, int seats, DateTime time, decimal cost);
        public record UpdateFlightDto(int id, string from, string to, int seats, DateTime time, decimal cost);
        private readonly IConfiguration _config;
        private readonly FlightDB _db;
        private readonly ILogger<FlightsController> _logger;

        public FlightsController(IConfiguration config, FlightDB db,ILogger<FlightsController> logger)
        {
            _config = config;
            _db = db;
            _logger = logger;
        }
        // GET: api/<FlightsController>
        [HttpGet]
        [Authorize(Policy = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<List<FlightM>> Get()
        {
            var flights = await _db.GetFlights();
            _logger.LogInformation($"got {flights.Count} flights through api for {HttpContext.Connection.RemoteIpAddress} at {DateTime.Now}");
            return flights;
        }

        // GET api/<FlightsController>/5
        [HttpGet("{id}")]
        [Authorize(Policy = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<FlightM?> Get(int id)
        {
            _logger.LogInformation($"got flight {id} through api for {HttpContext.Connection.RemoteIpAddress}");
            return await _db.GetFlight(id);
        }

        // POST api/<FlightsController>
        [HttpPost]
        [Authorize(Policy = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Post([FromBody] CreateFlightDto flight)
        {
            if (flight != null)
            {
                FlightM toBeCreated = new FlightM(flight.from, flight.to, flight.seats, flight.time, flight.cost);
                await _db.AddFLight(toBeCreated);
                _logger.LogInformation($"Created flight {JsonConvert.SerializeObject(flight)} for {HttpContext.Connection.RemoteIpAddress} at {DateTime.Now}");
                return Ok(await _db.GetFlightid(flight));
            }
            return BadRequest();
        }

        // PUT api/<FlightsController>/5
        [HttpPut]
        [Authorize(Policy = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Put([FromBody] UpdateFlightDto flight)
        {
            _logger.LogInformation($"Updating flight {flight.id} for {HttpContext.Connection.RemoteIpAddress} at {DateTime.Now}");
            if (flight != null)
            {
                try
                {
                    FlightM toBeUpdated = new(flight.id, flight.from, flight.to, flight.seats, flight.time, flight.cost);
                    if (await _db.UpdateFlight(toBeUpdated))
                    {
                        _logger.LogInformation($"Updated flight {flight.id} for {HttpContext.Connection.RemoteIpAddress} at {DateTime.Now} \n flight data {JsonConvert.SerializeObject(flight)}");
                        return Ok(flight);
                    }
                    _logger.LogWarning($"Couldn't update flight {flight.id} for {HttpContext.Connection.RemoteIpAddress} at {DateTime.Now} \n request data: {JsonConvert.SerializeObject(HttpContext.Request)}");
                    return BadRequest("no users were affected");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An error has occured while updating flight {flight.id} at {DateTime.Now} ");
                    return BadRequest($"An error occured while updating the user {flight.id}\n{e.Message}");
                }
            }
            _logger.LogWarning($"Couldn't update flight {flight.id} for {HttpContext.Connection.RemoteIpAddress} at {DateTime.Now} \n request data: {JsonConvert.SerializeObject(HttpContext.Request)}");
            return BadRequest("Could not update user");
        }

        // DELETE api/<FlightsController>/5
        [HttpDelete]
        [Authorize(Policy = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteFlight([FromQuery] int id)
        {
            _logger.LogInformation($"Deleting flight {id} thourgh api for {HttpContext.Connection.RemoteIpAddress}");
            try
            {
                _logger.LogInformation($"Deleted flight {id} thourgh api for {HttpContext.Connection.RemoteIpAddress}");
                return Ok(await _db.DeleteFlight(id));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error has occured while deleting flight {id} at {DateTime.Now} for {HttpContext.Connection.RemoteIpAddress}");
                return BadRequest($"Unable to delete user: {id}\n{e.Message}");
            }
        }
    }
}
