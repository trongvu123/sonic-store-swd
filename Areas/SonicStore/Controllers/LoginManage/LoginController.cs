using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SonicStore.Areas.SonicStore.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authentication.Google;
using SonicStore.Areas.SonicStore.Utils;

namespace SonicStore.Areas.SonicStore.Controllers.LoginManage
{
    [Area("SonicStore")]
    [Route("[area]/[controller]")]
    public class LoginController : Controller
    {
        EncriptPassword EncriptPassword { get; set; }
        private readonly SonicStoreContext _context;

        public LoginController(SonicStoreContext context)
        {
            _context = context;
        }

        //chuyen huong den trang xac thuc bang google
        [HttpGet("login-google")]
        public IActionResult LoginGoogle(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", "Login", new { area = "SonicStore", returnUrl }, Request.Scheme)
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        //lay du lieu xac thucdanh tinh tu google
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            var claimsIdentity = result?.Principal?.Identities.FirstOrDefault();

            if (claimsIdentity == null)
            {
                return RedirectToAction("Index", "Home", new { area = "SonicStore" });
            }

            var emailClaim = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
            var nameClaim = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;


            if (emailClaim == null || nameClaim == null)
            {
                return RedirectToAction("HomeScreen", "Home", new { area = "SonicStore" });
            }

            //tim kiem du lieu nguoi dung trong database
            var userCheck = await _context.Users
                .Include(u => u.Account)
                .SingleOrDefaultAsync(u => u.Email == emailClaim);

            if (userCheck != null)
            {
                await SignInUser(userCheck);
                if (userCheck.Account.Status != "on")
                {
                    TempData["StatusMessageGoogle"] = "Tài khoản đã bị vô hiệu hoá!";
                    return RedirectToAction("Index");
                }
                HttpContext.Session.SetString("user", JsonConvert.SerializeObject(userCheck, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
                if (userCheck.RoleId == 2)
                {
                    return RedirectToAction("SaleScreen", "Sale");
                }
                else if (userCheck.RoleId == 3)
                {
                    return RedirectToAction("TechNewsListScreen", "TechNewsList");
                }
                else if (userCheck.RoleId == 4)
                {
                    return RedirectToAction("AdminDashboardScreen", "AdminDashboard", new { Area = "SonicStore" });
                }
                else
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return Redirect("/");
                    }
                }
            }

            HttpContext.Session.SetString("GoogleEmail", emailClaim);
            HttpContext.Session.SetString("GoogleName", nameClaim);
            return RedirectToAction("RegisterGoogle", new { returnUrl });
        }


        //dang nhap bang google
        private async Task SignInUser(User user)
        {
            var userClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Account.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, getRoleName(user.RoleId))
        };

            var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(claimsPrincipal);
        }


        [HttpGet("register-google")]
        public IActionResult RegisterGoogle()
        {
            var email = HttpContext.Session.GetString("GoogleEmail");
            var name = HttpContext.Session.GetString("GoogleName");

            if (email == null || name == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Email = email;
            ViewBag.Name = name;

            return View();
        }

        //tao mat khau random cho account google
        private string GenerateRandomPassword(int length)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            Random random = new Random();
            return new string(Enumerable.Repeat(validChars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        //bo sung them thong tin dang ki
        [HttpPost("complete-register-google")]
        public async Task<IActionResult> CompleteRegisterGoogle([FromForm] User users, [FromForm] string tinh, [FromForm] string huyen, [FromForm] string xa)
        {
            var email = HttpContext.Session.GetString("GoogleEmail");
            var name = HttpContext.Session.GetString("GoogleName");

            if (email == null || name == null)
            {
                return RedirectToAction("Index");
            }

            string randomPassword = GenerateRandomPassword(12);
            var encryptedPassword = EncriptPassword.HashPassword(randomPassword);

            var account = new Account
            {
                Username = email,
                Password = encryptedPassword,
                RegisterDate = DateOnly.FromDateTime(DateTime.Now),
                Status = "on",
                GoogleAccountStatus = true

            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var user = new User
            {
                FullName = name,
                Email = email,
                Phone = users.Phone,
                Gender = users.Gender,
                Dob = users.Dob,
                UpdateDate = DateTime.Now,
                UpdateBy = 1,
                AccountId = account.Id,
                RoleId = 1
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userAddress = new UserAddress
            {
                User_Address = $"{xa}, {huyen}, {tinh}",
                Status = true,
                UserId = user.Id
            };

            _context.UserAddresses.Add(userAddress);
            await _context.SaveChangesAsync();

            HttpContext.Session.Clear();

            await SignInUser(user);

            TempData["StatusMessage"] = "Registration successful!";
            return RedirectToAction("Success");
        }

        [HttpGet("success")]
        public IActionResult NoticeSuccess()
        {
            return View();
        }




        [HttpGet("login")]
        public IActionResult Index(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Index(Account account, string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (ModelState.IsValid)
            {
                var userCheck = await _context.Accounts
                    .Include(a => a.User)
                    .SingleOrDefaultAsync(a => a.Username == account.Username);

                if (userCheck == null)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản không tồn tại.");
                }
                else
                {
                    if (!EncriptPassword.VerifyPassword(account.Password, userCheck.Password))
                    {
                        ModelState.AddModelError(string.Empty, "Sai mật khẩu.");
                    }
                    else if (userCheck.Status != "on")
                    {
                        ModelState.AddModelError(string.Empty, "Tài khoản không còn hiệu lực.");
                    }
                    else
                    {
                        string fullName = userCheck.User?.FullName;
                        ViewBag.FullName = fullName;

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, userCheck.Username),
                            new Claim(ClaimTypes.Role, getRoleName(userCheck.User.RoleId))
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);

                        var serializerSettings = new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        };

                        var userJson = JsonConvert.SerializeObject(userCheck.User, serializerSettings);
                        HttpContext.Session.SetString("user", userJson);
                        if (userCheck.User.RoleId == 2)
                        {
                            return RedirectToAction("SaleScreen", "Sale");
                        }
                        else if (userCheck.User.RoleId == 3)
                        {
                            return RedirectToAction("SliderScreen", "Slider");
                        }
                        else if (userCheck.User.RoleId == 4)
                        {
                            return RedirectToAction("AdminDashboardScreen", "AdminDashboard", new { Area = "SonicStore" });
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                return Redirect("/");
                            }
                        }
                    }
                }
            }
            return View(account);
        }
        private string getRoleName(int id)
        {
            switch (id)
            {
                case 1:
                    return "customer";
                case 2:
                    return "saler";
                case 3:
                    return "marketing";
                case 4:
                    return "admin";
                default:
                    return "";
            }
        }


        [HttpGet("logoutz")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync();
            return RedirectToAction("HomeScreen", "Home", new { area = "SonicStore" });
        }




        [HttpGet("forget-password")]
        public IActionResult EnterEmail()
        {
            return View();
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> SendOTPToEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                TempData["StatusMessage"] = "Email không tồn tại.";
                return RedirectToAction("EnterEmail");
            }

            Random random = new Random();
            int OTP = random.Next(10000, 99999);
            HttpContext.Session.SetInt32("OTP", OTP);
            HttpContext.Session.SetString("Email", email);
            var fromAddress = new MailAddress("thuyhnhe176007@fpt.edu.vn");
            var toAddress = new MailAddress(email);
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
            return RedirectToAction("VerifyPasswordOTP");
        }

        [HttpGet("verify-password-otp")]
        public IActionResult VerifyPasswordOTP()
        {
            return View();
        }

        [HttpPost("verify-password-otp")]
        public IActionResult VerifyPasswordOTP(string otp)
        {
            var storedOtp = HttpContext.Session.GetInt32("OTP");
            if (int.TryParse(otp, out int parsedOtp) && parsedOtp == storedOtp)
            {
                TempData["StatusMessage"] = "OTP hợp lệ. Vui lòng nhập mật khẩu mới.";
                return RedirectToAction("ResetUserPassword");
            }

            TempData["StatusMessage"] = "OTP không hợp lệ.";
            return RedirectToAction("VerifyPasswordOTP");
        }

        [HttpGet("reset-user-password")]
        public IActionResult ResetUserPassword()
        {
            return View();
        }

        [HttpPost("reset-user-password")]
        public async Task<IActionResult> ResetUserPassword(string newPassword, string confirmPassword)
        {
            var email = HttpContext.Session.GetString("Email");
            var user = await _context.Users.Include(u => u.Account).FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                TempData["StatusMessage"] = "Đã xảy ra lỗi. Vui lòng thử lại.";
                return RedirectToAction("ResetUserPassword");
            }

            if (newPassword != confirmPassword)
            {
                TempData["StatusMessage"] = "Mật khẩu không khớp. Vui lòng thử lại.";
                return RedirectToAction("ResetUserPassword");
            }

            if (EncriptPassword.VerifyPassword(newPassword, user.Account.Password))
            {
                TempData["StatusMessage"] = "Mật khẩu mới không được trùng với mật khẩu cũ. Vui lòng thử lại.";
                return RedirectToAction("ResetUserPassword");
            }

            user.Account.Password = EncriptPassword.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Đặt lại mật khẩu thành công.";
            return RedirectToAction("PasswordResetSuccess");
        }

        [HttpGet("password-reset-success")]
        public IActionResult PasswordResetSuccess()
        {
            return View();
        }
    }
}
