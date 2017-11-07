using Microsoft.AspNetCore.Mvc;

namespace BrightIdeas.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Default()
        {
            return View("~/Views/Shared/Errors/Default.cshtml");
        }
    }
}