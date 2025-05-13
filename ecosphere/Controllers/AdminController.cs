using EcoSphere.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AdminController : Controller
{
    private readonly MyDbContext _context;
    public AdminController(MyDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        var roleID = GetCurrentUserRoleId();
        ViewBag.UserRoleId = roleID;
        int? UserroleId = HttpContext.Session.GetInt32("Role_ID");
        if (UserroleId == null || UserroleId != 1)  // 1=Admin
        {
            return RedirectToAction("AccessDenied", "UserView");
        }
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
    public IActionResult UserManagement()
    {
        var roleID = GetCurrentUserRoleId();
        ViewBag.UserRoleId = roleID;
        int? UserroleId = HttpContext.Session.GetInt32("Role_ID");
        if (UserroleId == null || UserroleId != 1)  // 1=Admin
        {
            return RedirectToAction("AccessDenied", "UserView");
        }
        return View();
    }
}