using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcoSphere.Models;

namespace EcoSphere.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MyDbContext _context;
    public HomeController(ILogger<HomeController> logger, MyDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var roleID = GetCurrentUserRoleId();
        ViewBag.UserRoleId = roleID;
        if (TempData["SuccessMessage"] != null)
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
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
            return userRole?.RoleId ?? 0;  // E�er kullan�c� yoksa, misafir i�in 0 d�nd�r
        }


        return 0;  // Misafir rol�
    }
    public IActionResult Explore()
    {
        var roleID = GetCurrentUserRoleId();
        ViewBag.UserRoleId = roleID;
        if (TempData["SuccessMessage"] != null)
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
        }
        var totalCreatures = _context.TblCreatures.Count();
        var totalObservations = _context.TblMaintables.Count();
        var model = new DashboardViewModel
        {
            TotalCreatures = totalCreatures,
            TotalObservations = totalObservations
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        var roleID = GetCurrentUserRoleId();
        ViewBag.UserRoleId = roleID;
        return View();
    }
   

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var roleID = GetCurrentUserRoleId();
        ViewBag.UserRoleId = roleID;
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
