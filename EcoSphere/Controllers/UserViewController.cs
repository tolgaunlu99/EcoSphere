using EcoSphere.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http; // Session için gerekli
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
            var usernames = _context.TblUsers
                .Select(ur => new SelectListItem
                {
                    Value = ur.UserId.ToString(), // ID string olarak gidiyor
                    Text = ur.Username,
                }).ToList();

            var model = new UserViewModel
            {
                UserNamed = usernames
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult Login(UserViewModel model)
        {
            // UserId boş ise hata mesajı göster
            if (model.UserId == 0 || string.IsNullOrEmpty(model.Password))
            {
                ViewBag.ErrorMessage = "Please select a user and enter a password.";
                model.UserNamed = _context.TblUsers
                    .Select(u => new SelectListItem
                    {
                        Value = u.UserId.ToString(),
                        Text = u.Username
                    }).ToList();
                return View("SignIN", model);
            }

            var user = _context.TblUsers
                .FirstOrDefault(u => u.UserId == model.UserId && u.Password == model.Password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserID", user.UserId); // Kullanıcı ID'sini Session'a kaydet
                return RedirectToAction("Index", "Home"); // Başarılı girişte yönlendir
            }

            ViewBag.ErrorMessage = "Invalid username or password.";
            model.UserNamed = _context.TblUsers
                .Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = u.Username })
                .ToList();

            return View("SignIN", model);
        }

        public IActionResult SignUP()
        {
            return View();
        }
    }
}