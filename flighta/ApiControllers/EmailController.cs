using Flighta.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Flighta.Helpers.EmailSender;

namespace Flighta.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailSender emailSender,ILogger<EmailController> logger)
        {
            _emailSender = emailSender;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> SendEmailAsync(EmailData data)
        {
            _logger.LogInformation($"Successfully send an email through the api with data {JsonConvert.SerializeObject(data)} at {DateTime.Now} for {HttpContext.Connection.RemoteIpAddress}");
            var result = await _emailSender.SendEmailAsync(data);
            return Ok(result);
        }
    }
}
