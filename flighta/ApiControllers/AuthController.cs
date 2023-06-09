﻿using Flighta.Data;
using Flighta.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Flighta.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly FlightDB _db;
        private readonly ILogger<AuthController> _logger;

        public record UserDataDto(string email, string password);
        public AuthController(IConfiguration config, FlightDB db, ILogger<AuthController> logger)
        {
            _config = config;
            _db = db;
            _logger = logger;
        }
        [HttpPost]
        [Route("CreateToken")]
        [AllowAnonymous]
        public async Task<IActionResult> GetToken([FromBody] UserDataDto data)
        {
            var user = await CheckData(data);
            _logger.LogInformation($"Successfully generated a token for {HttpContext.Connection.RemoteIpAddress} at {DateTime.Now}");
            return (user is null) ? Unauthorized() : Ok(GenToken(user));
        }

        private async Task<User?> CheckData(UserDataDto data)
        {
            var user = data.email;
            bool result = await _db.LoginUser(user, data.password);
            if (result)
            {
                User? usr = await _db.GetUser(user);
                return usr;
            }
            _logger.LogInformation($"invalid login details while generating a token for {HttpContext.Connection.RemoteIpAddress} at {DateTime.Now}");
            return null;
        }
        private string? GenToken(User user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("Authentication:SecretKey")));
            var SigningCreds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new();
            claims.Add(new(JwtRegisteredClaimNames.Sub, user.Username));
            claims.Add(new Claim("Role", user.Role.ToString()));
            var token = new JwtSecurityToken(
                _config.GetValue<string>("Authentication:Issuer"),
                _config.GetValue<string>("Authentication:Audience"),
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(20),
                SigningCreds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
