using EcoSphere.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace EcoSphere.Controllers
{
    public class UserViewController : Controller
    {
        private readonly MyDbContext _context;

        public UserViewController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignIN()
        {
            var model = new UserViewModel
            {
                UserNamed = _context.TblUsers
                    .Select(u => new SelectListItem
                    {
                        Value = u.UserId.ToString(),
                        Text = u.Username
                    }).ToList()
            };

            // Modeli SignIN view'ine doğru şekilde gönderiyoruz
            return View(model);
        }

        [HttpPost]
        public IActionResult Login(UserViewModel model)
        {
            // Kullanıcı adı ve şifrenin boş olmadığını kontrol et
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                ViewBag.ErrorMessage = "Please enter both username and password.";
                return View("SignIN", model);
            }

            // Kullanıcıyı veritabanında username ve password ile sorgula
            var user = _context.TblUsers
                .FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

            if (user != null)
            {
                // Başarılı giriş, session'da UserId'yi tutuyoruz
                HttpContext.Session.SetInt32("UserID", user.UserId);
                return RedirectToAction("Index", "Home");  // Giriş sonrası Home sayfasına yönlendir
            }

            // Geçersiz giriş durumu
            ViewBag.ErrorMessage = "Invalid username or password.";
            return View("SignIN", model);
        }




        public IActionResult SignUP()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
