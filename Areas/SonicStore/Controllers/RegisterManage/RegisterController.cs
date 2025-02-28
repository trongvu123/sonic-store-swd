using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SonicStore.Areas.SonicStore.Dtos;
using SonicStore.Areas.SonicStore.Models;
using SonicStore.Areas.SonicStore.Utils;
using System;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SonicStore.Areas.SonicStore.Controllers
{
    [Area("SonicStore")]
    public class RegisterController : Controller
    {
        EncriptPassword EncriptPassword { get; set; }
        private readonly SonicStoreContext _context;
        public RegisterController(SonicStoreContext context)
        {
            _context = context;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View(new CompositeViewModel());
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] User users, [FromForm] Account accounts, [FromForm] string tinh, [FromForm] string huyen, [FromForm] string xa)
        {
            if (!ModelState.IsValid)
            {
                TempData["StatusMessage"] = "Đăng ký không thành công do dữ liệu không hợp lệ!";
                return RedirectToAction("Success");
            }

            try
            {   
                string addressInput = $"{xa}, {huyen}, {tinh}";
                HttpContext.Session.SetString("Username", accounts.Username);
                HttpContext.Session.SetString("Password", EncriptPassword.HashPassword(accounts.Password));
                HttpContext.Session.SetString("FullName", users.FullName);
                HttpContext.Session.SetString("Dob", users.Dob.ToString("o"));
                HttpContext.Session.SetString("Email", users.Email);
                HttpContext.Session.SetString("Phone", users.Phone);
                HttpContext.Session.SetString("Gender", users.Gender);
                HttpContext.Session.SetString("User_Address", addressInput);

                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Phone == users.Phone || u.Email == users.Email);
                if (existingUser != null)
                {
                    TempData["StatusMessage"] = "Số điện thoại hoặc Email đã tồn tại!";
                    return RedirectToAction("Success");
                }

                Random random = new Random();
                int OTP = random.Next(10000, 99999);
                HttpContext.Session.SetInt32("OTP", OTP);

                var fromAddress = new MailAddress("thuyhnhe176007@fpt.edu.vn");
                var toAddress = new MailAddress(users.Email);
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

                TempData["StatusMessage"] = "OTP đã được gửi đến email của bạn. Vui lòng kiểm tra email và nhập mã OTP để xác nhận!";
                return RedirectToAction("VerifyOTP");
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = "Đăng ký không thành công: " + ex.Message;
                return RedirectToAction("Success");
            }
        }


        [HttpGet("verify-otp")]
        public IActionResult VerifyOTP()
        {
            return View();
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP(string otp)
        {
            var storedOtp = HttpContext.Session.GetInt32("OTP");

            try
            {
                if (int.TryParse(otp, out int parsedOtp) && parsedOtp == storedOtp)
                {
                    var username = HttpContext.Session.GetString("Username");
                    var password = HttpContext.Session.GetString("Password");
                    var fullName = HttpContext.Session.GetString("FullName");
                    var dobString = HttpContext.Session.GetString("Dob");
                    var email = HttpContext.Session.GetString("Email");
                    var phone = HttpContext.Session.GetString("Phone");
                    var gender = HttpContext.Session.GetString("Gender");
                    var addressInput = HttpContext.Session.GetString("User_Address");

                    if (DateTime.TryParse(dobString, null, DateTimeStyles.RoundtripKind, out DateTime dob))
                    {
                        var account = new Account
                        {
                            Username = username,
                            Password = password,
                            RegisterDate = DateOnly.FromDateTime(DateTime.Now),
                            Status = "on",
                            ByAdmin = false
                        };

                        _context.Accounts.Add(account);
                        var accountSaveResult = await _context.SaveChangesAsync();

                        if (accountSaveResult == 0)
                        {
                            TempData["StatusMessage"] = "Thêm tài khoản không thành công!";
                            return RedirectToAction("Success");
                        }

                        var role = await _context.Roles.FindAsync(1);
                        if (role == null)
                        {
                            TempData["StatusMessage"] = "RoleId không hợp lệ!";
                            return RedirectToAction("Success");
                        }

                        var user = new User
                        {
                            FullName = fullName,
                            Dob = dob,
                            Email = email,
                            Phone = phone,
                            Gender = gender,
                            UpdateDate = DateTime.Now,
                            UpdateBy = 1,
                            AccountId = account.Id,
                            RoleId = 1
                        };

                        _context.Users.Add(user);
                        var userSaveResult = await _context.SaveChangesAsync();

                        if (userSaveResult == 0)
                        {
                            TempData["StatusMessage"] = "Thêm người dùng không thành công!";
                            return RedirectToAction("Success");
                        }

                        var userAddress = new UserAddress
                        {
                            User_Address = addressInput,
                            Status = true,
                            UserId = user.Id
                        };

                        _context.UserAddresses.Add(userAddress);
                        var addressSaveResult = await _context.SaveChangesAsync();

                        if (addressSaveResult == 0)
                        {
                            TempData["StatusMessage"] = "Thêm địa chỉ người dùng không thành công!";
                            return RedirectToAction("Success");
                        }

                        HttpContext.Session.Clear();

                        TempData["SignupSuccess"] = true;
                        TempData["StatusMessage"] = "Đăng ký thành công!";
                        return RedirectToAction("Success");
                    }
                    else
                    {
                        TempData["StatusMessage"] = "Invalid date of birth.";
                        return RedirectToAction("Success");
                    }
                }

                TempData["StatusMessage"] = "Invalid OTP";
                return RedirectToAction("VerifyOTP");
            }
            catch (Exception ex)
            {
                TempData["StatusMessage"] = "Xác minh OTP không thành công: " + ex.Message;
                return RedirectToAction("register");
            }
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            return View();
        }


        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("HomeScreen", "Home", new { area = "SonicStore" });
        }


    }
}
