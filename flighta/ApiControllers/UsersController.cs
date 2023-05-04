using Flighta.Data;
using Flighta.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Flighta.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        public record UpdateUserDto(int userid, string username, string password, string email);
        public record CreateUserDto(string username, string password, string email);
        private readonly IConfiguration _config;
        private readonly FlightDB _db;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IConfiguration config, FlightDB db,ILogger<UsersController> logger)
        {
            _config = config;
            _db = db;
            _logger = logger;
        }
        // GET: api/<UsersController>
        [HttpGet]
        public async Task<List<User>> Get()
        {
            _logger.LogInformation($"successfully got users for {HttpContext.Connection.RemoteIpAddress} through api at {DateTime.Now}");
            return await _db.GetUsers();
        }

        // GET api/<UsersController>/5
        [HttpGet("{emailOrUsername}")]
        public async Task<User?> Get(string emailOrUsername)
        {
            _logger.LogInformation($"Getting userid for {emailOrUsername} through api at {DateTime.Now}");
            return await _db.GetUser(emailOrUsername);
        }

        // POST api/<UsersController>
        //[AllowAnonymous] <--- should we allow anyone to register through the api ? not sure
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateUserDto user)
        {
            _logger.LogInformation($"Creating user {user} thourgh api");
            if (user != null)
            {
                _logger.LogInformation($"Created user {user} thorugh api");
                User toBeCreated = new User(user.username, user.password, user.email);
                await _db.CreateUser(toBeCreated);
                return Ok(await _db.GetUserId(user));
            }
            _logger.LogWarning($"Couldn't create user through api at {DateTime.Now}\n request data {JsonConvert.SerializeObject(HttpContext.Request)}");
            return BadRequest();
        }

        // PUT api/<UsersController>/5
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UpdateUserDto user)
        {
            _logger.LogInformation($"Updating user {user} thourgh api");
            if (user != null)
            {
                try
                {
                    User toBeUpdated = new User(user.userid, user.username, user.password, user.email);
                    if (await _db.UpdateUser(toBeUpdated))
                    {
                        _logger.LogInformation($"Updated user {user} thorugh api");
                        return Ok(user);
                    }
                    return BadRequest("no users were affected");
                }
                catch (Exception e)
                {
                    _logger.LogError(e,$"an error has occured while updating user {user} thorugh api at {DateTime.Now}");
                    return BadRequest($"An error occured while updating the user {user.userid}\n{e.Message}");
                }
            }
            _logger.LogWarning($"invalid user was specified while updating thorugh api \n user data: {user} ");
            return BadRequest("Could not update user");
        }

        // DELETE api/<UsersController>/5
        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromQuery] int id)
        {
            _logger.LogInformation($"Deleting user {id} thourgh api");
            try
            {
                _logger.LogInformation($"Deleted user {id} thourgh api");
                return Ok(await _db.DeleteUser(id));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error has occured while deleting user through api at {DateTime.Now}");
                return BadRequest($"Unable to delete user: {id}\n{e.Message}");
            }
        }
    }
}
