using flighta.Helpers;
using Flighta.Helpers;
using Microsoft.AspNetCore.Mvc;
using static Flighta.Helpers.EmailSender;

namespace flighta.Controllers
{
    public class ContactController : Controller
    {
        private readonly IEmailSender _sender;
        private readonly ILogger _logger;

        public ContactController(IEmailSender sender, ILogger logger)
        {
            _sender = sender;
            _logger = logger;
        }
        public IActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {

                _logger.MvcControllerLog("Contact", "Index", DateTime.Now, HttpContext.User.Identity.Name);
            }
            _logger.LogInformation($"An unauthenticated user with ip {HttpContext.Connection.RemoteIpAddress} viewed the contacts page at {DateTime.Now}");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(EmailData data)
        {
            if (ModelState.IsValid)
            {
                TempData["info"] = $"thank you for your feedback {data.fullname} !";
                _sender.SendEmailAsync(data);
                _logger.EmailSendLog(data.fullname, data.email, DateTime.Now);
                return RedirectToAction("Index");

            }
            return RedirectToAction("Index");
        }
    }
}
