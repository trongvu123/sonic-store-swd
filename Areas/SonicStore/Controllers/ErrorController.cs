using Microsoft.AspNetCore.Mvc;

namespace SonicStore.Areas.SonicStore.Controllers
{
    [Area("SonicStore")]
    public class ErrorController : Controller
    {
        [HttpGet("error")]
        public async Task<IActionResult> ErrorScreen()
        {
            return View();
        }
    }
}
