using Flighta.Helpers;
using Microsoft.AspNetCore.Mvc;
using static Flighta.Helpers.EmailSender;

namespace flighta.Controllers
{
    public class ContactController : Controller
    {
        private readonly IEmailSender _sender;

        public ContactController(IEmailSender sender)
        {
            _sender = sender;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(EmailData data)
        {
            if(ModelState.IsValid)
            {
            TempData["info"] = $"thank you for your feedback {data.fullname} !";
            _sender.SendEmailAsync(data);
            return RedirectToAction("Index");

            }
            return RedirectToAction("Index");
        }
    }
}
