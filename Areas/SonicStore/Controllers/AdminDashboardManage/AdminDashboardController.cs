using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SonicStore.Areas.SonicStore.Models;
using SonicStore.Areas.SonicStore.Utils;

namespace SonicStore.Areas.SonicStore.Controllers.AdminDashboardManage
{
    [Authorize(Roles = "admin")]
    [Area("SonicStore")]
    public class AdminDashboardController : Controller
    {
        private readonly SonicStoreContext _context;

        public AdminDashboardController(SonicStoreContext context)
        {
            _context = context;
        }
        public class userInput
        {
            public string fullName { get; set; }
            public int role {  get; set; }
            public string gender { get; set; }
            public DateTime dob { get; set; }
            public string tinh { get; set; }
            public string huyen { get; set; }
            public string xa {  get; set; }

        }
        [HttpGet("admin-dashboard")]
        public async Task<IActionResult> AdminDashboardScreen()
        {
            var userJson = HttpContext.Session.GetString("user");
            var userSession = JsonConvert.DeserializeObject<User>(userJson);
            return View(userSession);
        }
        [HttpGet("loadData-users")]
        public async Task<JsonResult> loadData()
        {
            var listUser = await _context.Users.Include(u => u.Account).Include(u=>u.Role).Where(u=>u.RoleId != 4).Select(u=> new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.Phone,
                u.Role.RoleName,
                u.Account.Status,
                u.Account.ByAdmin
            }).OrderByDescending(u=>u.ByAdmin).ToListAsync();
            return Json(new
            {
                data = listUser,
                status = true
            });
        }
        [HttpPost("change-status-user")]
        public async Task<JsonResult> ChangeStatusUser(string change, int id) 
        {            
            try
            {
                var accountUser = await _context.Users.Include(u => u.Account).Where(u => u.Id == id).Select(a => a.Account).FirstOrDefaultAsync();
                if(change != null)
                {
                    if(change == "on")
                    {
                        accountUser.Status = "on";
                        _context.Accounts.Update(accountUser);
                    }
                    else
                    {
                        accountUser.Status = "off";
                        _context.Accounts.Update(accountUser);
                    }
                    await _context.SaveChangesAsync();  
                }
            }catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
            return Json(new { status = true });
        }
      
  
        [HttpPost("change-role-user")]
        public async Task<JsonResult> ChangeRole(string roleName, int id)
        {
            try
            {

                var user = await _context.Users.Include(u=>u.Role).Where(u=>u.Id == id).FirstOrDefaultAsync();
                if(roleName == "sale")
                {
                    user.RoleId = 2;
                    _context.Users.Update(user);
                }
                else if(roleName == "customer")
                {
                    user.RoleId = 1;
                    _context.Users.Update(user);
                }
                else if(roleName == "marketing")
                {
                    user.RoleId = 3;
                    _context.Users.Update(user);
                }
                else
                {
                    user.RoleId = 4;
                    _context.Users.Update(user);
                }
                 await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                await Console.Out.WriteLineAsync(ex.Message);
            }
            return Json(new { status = true }); 
        }
        [HttpPost("add-user-admin")]
        public async Task<JsonResult> AddUser(string userData)
        {
            var userJson = JsonConvert.DeserializeObject<userInput>(userData);
            var account = new Account
            {
                Username = RandomInfomation.GetUserName(),
                Password = EncriptPassword.HashPassword("123"),
                RegisterDate = DateOnly.FromDateTime(DateTime.Now),
                Status = "on",
                ByAdmin = true
            };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            var user = new User
            {
                FullName = userJson.fullName,
                Dob = userJson.dob,
                Email = RandomInfomation.MakeEmail(),
                Gender = userJson.gender,
                Phone = RandomInfomation.GetRandomPhoneNumber(10),
                UpdateDate = DateTime.Now,
                UpdateBy = 1,
                AccountId = account.Id,
                RoleId = userJson.role
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            string addressInput = $"{userJson.xa}, {userJson.huyen}, {userJson.tinh}";

            var userAddress = new UserAddress
            {
                User_Address = addressInput,
                Status = true,
                UserId = user.Id,
                
            };
            _context.UserAddresses.Add(userAddress);
            await _context.SaveChangesAsync();
            return Json(new { status = true });
        }
        [HttpPost("get-username")]
        public async Task<JsonResult> GetUsername(int id)
        {
            var username = await _context.Users.Include(u=>u.Account).Where(u=>u.Id == id).Select(u=>u.Account.Username).FirstOrDefaultAsync();
            return Json(new {username = username});
        }
        [HttpGet("role-list")]
        public async Task<IActionResult> RoleListScreen()
        {
            return View();
        }
    }
}
