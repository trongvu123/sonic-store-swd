using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SonicStore.Areas.SonicStore.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using SonicStore.Areas.SonicStore.Utils;

namespace SonicStore.Areas.SonicStore.Controllers.ProfileManage
{
    [Authorize]
    [Area("SonicStore")]
    public class ProfileController : Controller
    {
        private readonly SonicStoreContext _context;

        public ProfileController(SonicStoreContext context)
        {
            _context = context;
        }
        public class inputUser
        {
            public string fullName { get; set; }
            public string email { get; set; }
            public string gender { get; set; }
            public string phone { get; set; }
            public DateTime dob { get; set; }
            public string tinh { get; set; }
            public string huyen { get; set; }
            public string xa { get; set; }
        }
        [HttpGet("user-profile")]
        public async Task<IActionResult> ProfileScreen()
        {
            var userJson = HttpContext.Session.GetString("user");
            var userSession = JsonConvert.DeserializeObject<User>(userJson);
            var googleCheck = _context.Users.Include(u => u.Role).Include(a => a.Account).Where(u => u.Id == userSession.Id).Select(u => u.Account.GoogleAccountStatus).FirstOrDefault();
            var roleId = userSession.RoleId;
            ViewBag.googleCheck = googleCheck;
            ViewBag.roleId = roleId;
            return View(userSession);
        }
        [HttpGet("loadData-user")]
        public async Task<JsonResult> loadDataUser()
        {
            var userJson = HttpContext.Session.GetString("user");
            var userSession = JsonConvert.DeserializeObject<User>(userJson);

            var updatedUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userSession.Id);

            if (updatedUser != null)
            {
                var updatedUserJson = JsonConvert.SerializeObject(updatedUser);
                HttpContext.Session.SetString("user", updatedUserJson);

                string address = await _context.UserAddresses
                    .Where(u => u.UserId == updatedUser.Id && u.Status == true)
                    .Select(u => u.User_Address)
                    .FirstOrDefaultAsync();

                string[] arr = address?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
                ViewBag.AddressParts = arr;

                return Json(new
                {
                    fullName = updatedUser.FullName,
                    dob = updatedUser.Dob.ToString("yyyy-MM-dd"),
                    email = updatedUser.Email,
                    phone = updatedUser.Phone,
                    gender = updatedUser.Gender,
                    updateDate = updatedUser.UpdateDate,
                    address = arr,
                    status = true
                });
            }
            else
            {
                // Xử lý trường hợp không tìm thấy user
                return Json(new { status = false, message = "User not found" });
            }
        }
        [HttpPost("save-user")]
        public async Task<JsonResult> SaveData(string strUser)
        {
            int status = 1;
            try
            {

                var userJson = HttpContext.Session.GetString("user");
                var userSession = JsonConvert.DeserializeObject<User>(userJson);
                var userInput = JsonConvert.DeserializeObject<inputUser>(strUser);
                var emailExist = await _context.Users.Where(u => u.Email.Equals(userInput.email) && u.Id != userSession.Id).AnyAsync();
                var phoneExist = await _context.Users.Where(u => u.Phone == userInput.phone && u.Id != userSession.Id).AnyAsync();
                if (emailExist && phoneExist)
                {
                    status = 4;
                    return Json(new { status = status, phone = userSession.Phone, email = userSession.Email });
                }
                else if (emailExist)
                {
                    status = 3;
                    return Json(new { status = status, email = userSession.Email });
                }
                else if (phoneExist)
                {
                    status = 2;
                    return Json(new { status = status, phone = userSession.Phone });
                }
                else
                {
                    if (userSession.Email != userInput.email)
                    {

                        Random random = new Random();
                        int OTP = random.Next(10000, 99999);
                        HttpContext.Session.SetInt32("OTP", OTP);

                        var fromAddress = new MailAddress("thuyhnhe176007@fpt.edu.vn");
                        var toAddress = new MailAddress(userInput.email);
                        const string fromPass = "bgep snbt jokm vatc";
                        const string subject = "OTP Code";
                        string body = $@"
                <html>
                <head>
                    <style>
                        h1 {{
                            color: #333;
                            font-family: Arial, sans-serif;
                        }}
                        .otp {{
                            color: #e74c3c;
                            font-size: 24px;
                            font-weight: bold;
                        }}
                        .container {{
                            border: 1px solid #ddd;
                            padding: 20px;
                            max-width: 600px;
                            margin: auto;
                            font-family: Arial, sans-serif;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h1>Your OTP Code</h1>
                        <p class='otp'>{OTP}</p>
                        <p>Please use this code to complete your transaction.</p>
                    </div>
                </body>
                </html>";

                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(fromAddress.Address, fromPass),
                            Timeout = 200000
                        };

                        using (var message = new MailMessage(fromAddress, toAddress))
                        {
                            message.Subject = subject;
                            message.Body = body;
                            message.IsBodyHtml = true;
                            smtp.Send(message);
                        }
                        status = 5;
                        return Json(new { status = status });
                    }
                    else
                    {
                        await SaveUserInfo(userSession, userInput);
                        var updatedUserJson = HttpContext.Session.GetString("user");
                        var updatedUser = JsonConvert.DeserializeObject<User>(updatedUserJson);
                        status = 1;
                        return Json(new { status = status, user = updatedUser });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Json(new { status = status });
        }
        [HttpPost("verify-otp-profile")]
        public async Task<JsonResult> VerifyOTP(string otp, string strUser)
        {
            try
            {
                int status = 0;
                var sessionOTP = HttpContext.Session.GetInt32("OTP");
                if (sessionOTP.HasValue && sessionOTP.Value.ToString() == otp)
                {
                    var userJson = HttpContext.Session.GetString("user");
                    if (string.IsNullOrEmpty(userJson))
                    {
                        return Json(new { status = status, error = "User session not found" });
                    }

                    var userSession = JsonConvert.DeserializeObject<User>(userJson);
                    if (userSession == null)
                    {
                        return Json(new { status = status, error = "Invalid user session data" });
                    }

                    if (string.IsNullOrEmpty(strUser))
                    {
                        return Json(new { status = status, error = "User input data is missing" });
                    }

                    var userInput = JsonConvert.DeserializeObject<inputUser>(strUser);
                    if (userInput == null)
                    {
                        return Json(new { status = status, error = "Invalid user input data" });
                    }

                    await SaveUserInfo(userSession, userInput);
                    status = 1;
                }
                return Json(new { status = status });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, error = "An error occurred while processing your request" });
            }
        }

        private async Task SaveUserInfo(User userSession, inputUser userInput)
        {
            userSession.FullName = userInput.fullName;
            userSession.Email = userInput.email;
            userSession.Gender = userInput.gender;
            userSession.Dob = userInput.dob;
            userSession.Phone = userInput.phone;
            _context.Users.Update(userSession);
            var account = await _context.Accounts.Where(a=>a.Id == userSession.AccountId).FirstOrDefaultAsync();
            account.ByAdmin = false; 
            _context.Accounts.Update(account);
            var userAddress = await _context.UserAddresses.Where(u => u.UserId == userSession.Id && u.Status == true).FirstOrDefaultAsync();
            userAddress.User_Address = $"{userInput.xa}, {userInput.huyen}, {userInput.tinh}";
            _context.UserAddresses.Update(userAddress);
            await _context.SaveChangesAsync();
            var updatedUserJson = JsonConvert.SerializeObject(userSession, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            HttpContext.Session.SetString("user", updatedUserJson);
        }
        [HttpPost("old-password")]
        public async Task<JsonResult> OldPassword(string password)
        {
            int status = 1;
            try
            {
                var userJson = HttpContext.Session.GetString("user");
                var userSession = JsonConvert.DeserializeObject<User>(userJson);
                var passwordCheck = await _context.Accounts.Include(u => u.User).Where(c => c.User.Id == userSession.Id).FirstAsync();
                if (!EncriptPassword.VerifyPassword(password, passwordCheck.Password))
                {
                    status = 2;
                    return Json(new { status = status });
                }

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
            return Json(new { status = status });
        }
        [HttpPost("change-password")]
        public async Task<JsonResult> ChangePassword(string password)
        {
            int status = 1;
            var userJson = HttpContext.Session.GetString("user");
            var userSession = JsonConvert.DeserializeObject<User>(userJson);
            var account = await _context.Accounts.Where(c => c.User.Id == userSession.Id).FirstAsync();
            account.Password = EncriptPassword.HashPassword(password);
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return Json(new { status = status });
        }
    }
}