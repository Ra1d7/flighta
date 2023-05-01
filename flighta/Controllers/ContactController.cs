using Microsoft.AspNetCore.Mvc;

namespace flighta.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
