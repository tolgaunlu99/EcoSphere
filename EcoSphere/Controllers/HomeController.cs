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
            return userRole?.RoleId ?? 0;  // Eðer kullanýcý yoksa, misafir için 0 döndür
        }


        return 0;  // Misafir rolü
    }
    public IActionResult Explore()
    {
        var roleID = GetCurrentUserRoleId();
        ViewBag.UserRoleId = roleID;

        if (TempData["SuccessMessage"] != null)
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
        }

        var totalCreatures = _context.VwSpecies.Count();

        var totalObservationsQuery = _context.VwMaps.AsQueryable();
        if (roleID != 1 && roleID != 2)
        {
            totalObservationsQuery = totalObservationsQuery.Where(v => v.EndemicstatID != 1);
        }

        var totalObservations = totalObservationsQuery.Count(v => v.status == 1);
        var totalPlants = totalObservationsQuery.Count(v => v.KingdomName == "Plantae" && v.status == 1);
        var totalAnimals = totalObservationsQuery.Count(v => v.KingdomName == "Animalia" && v.status == 1);

        var model = new DashboardViewModel
        {
            TotalCreatures = totalCreatures,
            TotalObservations = totalObservations,
            TotalPlants = totalPlants,
            TotalAnimals = totalAnimals
        };

        return View(model);
    }
    public IActionResult Privacy()
    {
        var roleID = GetCurrentUserRoleId();
        ViewBag.UserRoleId = roleID;
        return View();
    }
    public IActionResult PendingObservations()
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
