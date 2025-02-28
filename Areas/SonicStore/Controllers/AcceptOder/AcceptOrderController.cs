using Microsoft.AspNetCore.Mvc;

namespace SonicStore.Areas.SonicStore.Controllers.AcceptOder
{
    [Area("SonicStore")]
    public class AcceptOrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
