using EcoSphere.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace EcoSphere.Controllers
{
    public class UserViewController : Controller
    {
        private readonly MyDbContext _context;

        public UserViewController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var roleID = GetCurrentUserRoleId();
            ViewBag.UserRoleId = roleID;
            return View();
        }
        private int GetCurrentUserRoleId()
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId.HasValue)
            {
                var userRole = _context.TblUserRoles
                                       .FirstOrDefault(ur => ur.UserId == userId.Value);
                return userRole?.RoleId ?? 0;  // Eğer kullanıcı yoksa, misafir için 0 döndür
            }

            return 0;  // Misafir rolü
        }

        [HttpPost]
        public IActionResult Index(UserViewModel model)
        {
            var formAction = Request.Form["formAction"];

            if (formAction == "login")
            {
                var hashedPassword = HashPassword(model.Password);

                var user = _context.TblUsers.FirstOrDefault(u =>
                    u.Username == model.Username && u.Password == hashedPassword);

                if (user != null)
                {
                    var action= new TblUseraction
                    {
                        UserId = user.UserId,
                        Action = "Giriş Yapıldı!!",
                        ActionTime = DateTime.Now
                    };
                    _context.TblUseractions.Add(action);
                    _context.SaveChanges();
                    var userRole = _context.TblUserRoles.FirstOrDefault(r => r.UserId == user.UserId);

                    if (userRole != null && userRole.RoleId.HasValue)
                    {
                        HttpContext.Session.SetInt32("UserID", user.UserId);
                        HttpContext.Session.SetInt32("Role_ID", userRole.RoleId.Value); // Burada .Value kullanılıyor
                    }
                    else
                    {
                        return RedirectToAction("AccessDenied", "UserView");
                    }
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Home"), message = $"Welcome, {user.Username}!" });
                }

                return Json(new { success = false, message = "Invalid username or password." });
            }
            else if (formAction == "signup")
            {
                if (_context.TblUsers.Any(u => u.Username == model.Username))
                {
                    return Json(new { success = false, message = "Username already exists." });
                }
                if (_context.TblUsers.Any(u => u.Email == model.Email))
                {
                    return Json(new { success = false, message = "E-mail already exists." });
                }

                var newUser = new TblUser
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Username = model.Username,
                    Email = model.Email,
                    Password = HashPassword(model.Password),
                    CreationDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                _context.TblUsers.Add(newUser);
                _context.SaveChanges();
                var createdUserID=newUser.UserId;
                var userRole= new TblUserRole
                {
                    UserId = createdUserID,
                    RoleId = 4,
                    Status="available",
                    CreationDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
                _context.TblUserRoles.Add(userRole);
                _context.SaveChanges();
                return Json(new { success = true, message = "Registration successful. You can now log in." });
            }

            return Json(new { success = false, message = "Unknown form action." });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
        public IActionResult AccessDenied()
        {
            var roleID = GetCurrentUserRoleId();
            ViewBag.UserRoleId = roleID;
            return View();
        }
        public IActionResult ForgotPassword()
        {
            var roleID = GetCurrentUserRoleId();
            ViewBag.UserRoleId = roleID;
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            var roleID = GetCurrentUserRoleId();
            ViewBag.UserRoleId = roleID;
            var user = _context.TblUsers.FirstOrDefault(x => x.Email == email);
            if (user == null)
            {
                ViewBag.Error = "Bu e-mail sistemde kayıtlı değil.";
                return View();
            }

            var otp = new Random().Next(10000, 99999).ToString();
            HttpContext.Session.SetString("ResetEmail", email);
            HttpContext.Session.SetString("ResetOTP", otp);
            HttpContext.Session.SetString("OTPTime", DateTime.Now.ToString());

            // Gmail ile OTP gönder
            SendOtpMail(email, otp);

            ViewBag.EmailConfirmed = true;
            return View();
        }
        private void SendOtpMail(string toEmail, string otp)
        {
            var fromAddress = new MailAddress("dkmverifysystem@gmail.com", "Şifre Sıfırlama");
            var toAddress = new MailAddress(toEmail);
            const string subject = "Şifre Sıfırlama Kodu";
            string body = $"Merhaba,\n\nŞifre sıfırlama kodunuz: {otp}\n\nBu kodu kullanarak şifrenizi yenileyebilirsiniz.";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential("dkmverifysystem@gmail.com", "jmxw finn yywv phnb"),
                Timeout = 20000
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
        [HttpPost]
        public IActionResult VerifyOtp(string otp)
        {
            var roleID = GetCurrentUserRoleId();
            ViewBag.UserRoleId = roleID;
            var sessionOtp = HttpContext.Session.GetString("ResetOTP");
            var email = HttpContext.Session.GetString("ResetEmail");

            if (otp == sessionOtp)
            {
                return RedirectToAction("UpdatePassword");
            }

            ViewBag.EmailConfirmed = true;
            ViewBag.Error = "Kod yanlış. Lütfen tekrar deneyin.";
            return View("ForgotPassword");
        }
        [HttpGet]
        public IActionResult UpdatePassword()
        {
            var roleID = GetCurrentUserRoleId();
            ViewBag.UserRoleId = roleID;
            return View();
        }
        [HttpPost]
        public IActionResult UpdatePassword(string newPassword, string confirmPassword)
        {
            var roleID = GetCurrentUserRoleId();
            ViewBag.UserRoleId = roleID;

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Şifreler uyuşmuyor.";
                return View();
            }

            var email = HttpContext.Session.GetString("ResetEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            var user = _context.TblUsers.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Error = "Kullanıcı bulunamadı.";
                return View();
            }

            user.Password = HashPassword(newPassword);
            user.UpdatedDate = DateTime.Now;
            _context.SaveChanges();

            HttpContext.Session.Remove("ResetEmail");
            HttpContext.Session.Remove("ResetOTP");

            return RedirectToAction("Index", "UserView");
        }
    }
}