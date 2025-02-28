using Microsoft.AspNetCore.Mvc;
using SonicStore.Areas.SonicStore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace SonicStore.Areas.SonicStore.Controllers.UserManage
{
    [Area("SonicStore")]
    public class UserController : Controller
    {
        private readonly SonicStoreContext context;

        public UserController(SonicStoreContext context)
        {
            this.context = context;
        }

        [HttpGet("account")]
        public IActionResult Account()
        {
            // Retrieve user information from session
            var userJson = HttpContext.Session.GetString("user");

            // Check if userJson is not null or empty
            if (!string.IsNullOrEmpty(userJson))
            {
                // Deserialize JSON string to User object
                var user = JsonConvert.DeserializeObject<User>(userJson);

                // Pass the User object to the view
                return View(user);
            }
            else
            {
                // Handle the case where user information is not found in the session
                // Redirect to a login page or handle the scenario accordingly
                return RedirectToAction("Index", "Login");
            }
        }
    }
}
