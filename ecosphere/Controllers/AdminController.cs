using EcoSphere.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

        var today = DateTime.Today;
        var last7Days = Enumerable.Range(0, 7)
                                   .Select(i => today.AddDays(-i))
                                   .OrderBy(d => d)
                                   .ToList();

        // EF'den verileri çek
#pragma warning disable CS8629 // Nullable value type may be null.
        var userActions = _context.TblUseractions
                                   .Where(x => x.ActionTime.HasValue)
                                   .Select(x => x.ActionTime.Value)
                                   .ToList();
#pragma warning restore CS8629 // Nullable value type may be null.

        // C# LINQ ile günlük sayıları hesapla
        var dailyLogins = last7Days
            .Select(day => userActions.Count(x => x.Date == day.Date))
            .ToList();

        // CPU ve RAM kullanımı

        double cpuUsage = GetCpuUsagePercentage();
        double ramUsage = GetMemoryUsagePercentage();

        var dashboard = new DashboardViewModel
        {
            TotalUsers = _context.TblUsers.Count(),
            TotalCreatures = _context.TblCreatures.Count(),
            TotalObservations = _context.TblMaintables.Count(),
            TodayLogins = userActions.Count(x => x.Date == today.Date),
            TotalLogs = _context.TblUseractions.Count(),
            RecentLogs = _context.TblUseractions
                .OrderByDescending(x => x.ActionTime)
                .Take(5)
                .AsEnumerable()
                .Select(x => new RecentLog
                {
                    Username = _context.TblUsers.FirstOrDefault(u => u.UserId == x.UserId)?.Username ?? "Sistem",
                    Action = x.Action,
                    ActionTime = x.ActionTime ?? DateTime.MinValue
                }).ToList(),
            Last7Days = last7Days.Select(d => d.ToString("dd.MM")).ToList(),
            DailyLogins = dailyLogins,
            CpuUsage = cpuUsage,
            RamUsage = ramUsage
        };
        dashboard.RoleDescriptions = new Dictionary<string, string>
{
    { "Admin", "Sistemde tüm işlemleri yapabilir." },
    { "Expert", "Raporları inceleyebilir." },
    { "Observer", "Sadece gözlem yapabilir." },
    { "User", "Sistemi temel olarak kullanabilir." }
};
        // Role Chart
        var roleGroups = _context.TblUserRoles
            .GroupBy(ur => ur.RoleId)
            .Select(g => new
            {
                RoleId = g.Key,
                Count = g.Count()
            }).ToList();

        var roleNames = new List<string>();
        var roleCounts = new List<int>();

        foreach (var roleGroup in roleGroups)
        {
            var roleName = _context.TblRoles
                .Where(r => r.RoleId == roleGroup.RoleId)
                .Select(r => r.RoleName)
                .FirstOrDefault() ?? "Unknown";

            roleNames.Add(roleName);
            roleCounts.Add(roleGroup.Count);
        }

        dashboard.RoleNames = roleNames;
        dashboard.RoleCounts = roleCounts;

        return View(dashboard);
    }

    // CPU Kullanımı (örnek)
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private double GetCpuUsagePercentage()
    {
        try
        {
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue(); // ilk okuma her zaman 0 verir
            System.Threading.Thread.Sleep(500); // biraz bekle
            return Math.Round(cpuCounter.NextValue(), 2);
        }
        catch
        {
            return 0;
        }
    }

    // RAM Kullanımı (örnek)
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private double GetMemoryUsagePercentage()
    {
        try
        {
            var totalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
            var usedMemory = totalMemory - GC.GetGCMemoryInfo().HighMemoryLoadThresholdBytes;

            double usedMemoryPercentage = ((double)usedMemory / totalMemory) * 100;
            return Math.Round(usedMemoryPercentage, 2);
        }
        catch
        {
            return 0;
        }
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
        var currentUserId = HttpContext.Session.GetInt32("UserID");
        var users = (from u in _context.TblUsers
                     join ur in _context.TblUserRoles on u.UserId equals ur.UserId
                     join r in _context.TblRoles on ur.RoleId equals r.RoleId
                     where u.UserId != currentUserId
                     select new UserManagementViewModel
                     {
                         UserId = u.UserId,
                         Name = u.Name,
                         Surname = u.Surname,
                         Username = u.Username,
                         Email = u.Email,
                         RoleName = r.RoleName,
                         CreationDate = u.CreationDate,
                         UpdatedDate = u.UpdatedDate
                     }).ToList();

        return View(users);
    }
    public IActionResult LogManagement()
    {
        var roleID = GetCurrentUserRoleId();
        ViewBag.UserRoleId = roleID;

        int? UserroleId = HttpContext.Session.GetInt32("Role_ID");
        if (UserroleId == null || UserroleId != 1)  // 1=Admin
        {
            return RedirectToAction("AccessDenied", "UserView");
        }

        var logs = (from action in _context.TblUseractions
                    join user in _context.TblUsers
                    on action.UserId equals user.UserId
                    select new UserActionViewModel
                    {
                        UserActionID = action.UserActionId,
                        UserName = user.Username,
                        Action = action.Action,
                        ActionTime = action.ActionTime ?? DateTime.MinValue
                    }).ToList();

        return View(logs);
    }

    [HttpPost]
    public JsonResult FilterLogs(string filterType)
    {
        try
        {
            List<UserActionViewModel> filteredLogs = new List<UserActionViewModel>();

            switch (filterType)
            {
                case "login-logout":
                    filteredLogs = _context.TblUseractions
                        .Where(x => x.Action.Contains("Giriş Yapıldı") || x.Action.Contains("Çıkış Yapıldı"))
                        .Join(_context.TblUsers,
                            action => action.UserId,
                            user => user.UserId,
                            (action, user) => new UserActionViewModel
                            {
                                UserActionID = action.UserActionId,
                                UserName = user.Username,
                                Action = action.Action,
                                ActionTime = action.ActionTime ?? DateTime.MinValue
                            })
                        .OrderByDescending(x => x.ActionTime)
                        .ToList();
                    break;

                case "add":
                    filteredLogs = _context.TblUseractions
                        .Where(x => x.Action.Contains("ekledi"))
                        .Join(_context.TblUsers,
                            action => action.UserId,
                            user => user.UserId,
                            (action, user) => new UserActionViewModel
                            {
                                UserActionID = action.UserActionId,
                                UserName = user.Username,
                                Action = action.Action,
                                ActionTime = action.ActionTime ?? DateTime.MinValue
                            })
                        .OrderByDescending(x => x.ActionTime)
                        .ToList();
                    break;

                case "role-change":
                    filteredLogs = _context.TblUseractions
                        .Where(x => x.Action.Contains("rolünü"))
                        .Join(_context.TblUsers,
                            action => action.UserId,
                            user => user.UserId,
                            (action, user) => new UserActionViewModel
                            {
                                UserActionID = action.UserActionId,
                                UserName = user.Username,
                                Action = action.Action,
                                ActionTime = action.ActionTime ?? DateTime.MinValue
                            })
                        .OrderByDescending(x => x.ActionTime)
                        .ToList();
                    break;

                case "delete-user":
                    filteredLogs = _context.TblUseractions
                        .Where(x => x.Action.Contains("kullanıcısını sildi"))
                        .Join(_context.TblUsers,
                            action => action.UserId,
                            user => user.UserId,
                            (action, user) => new UserActionViewModel
                            {
                                UserActionID = action.UserActionId,
                                UserName = user.Username,
                                Action = action.Action,
                                ActionTime = action.ActionTime ?? DateTime.MinValue
                            })
                        .OrderByDescending(x => x.ActionTime)
                        .ToList();
                    break;

                case "delete-records":
                    filteredLogs = _context.TblUseractions
                        .Where(x => x.Action.Contains("tarafından Silindi"))
                        .Join(_context.TblUsers,
                            action => action.UserId,
                            user => user.UserId,
                            (action, user) => new UserActionViewModel
                            {
                                UserActionID = action.UserActionId,
                                UserName = user.Username,
                                Action = action.Action,
                                ActionTime = action.ActionTime ?? DateTime.MinValue
                            })
                        .OrderByDescending(x => x.ActionTime)
                        .ToList();
                    break;

                default:
                    filteredLogs = new List<UserActionViewModel>();
                    break;
            }

            return Json(new { success = true, data = filteredLogs });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
    public IActionResult ObservationManagement()
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
    public IActionResult SpeciesManagement()
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
    [HttpPost]
    public JsonResult DeleteUser(int id)
    {
        try
        {
            var userActions = _context.TblUseractions.Where(x => x.UserId == id).ToList();
            if (userActions.Any())
            {
                _context.TblUseractions.RemoveRange(userActions);
            }
            // 1️⃣ tbl_user_roles tablosundan sil
            var userRole = _context.TblUserRoles.FirstOrDefault(x => x.UserId == id);
            if (userRole != null)
            {
                _context.TblUserRoles.Remove(userRole);
            }

            // 2️⃣ tbl_users tablosundan sil
            var user = _context.TblUsers.FirstOrDefault(x => x.UserId == id);
            string deletedUsername = "";
            if (user != null)
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                deletedUsername = user.Username;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                _context.TblUsers.Remove(user);
            }

            // 3️⃣ tbl_useractions tablosuna log ekle
            var userId = HttpContext.Session.GetInt32("UserID");
            var username = _context.TblUsers.FirstOrDefault(u => u.UserId == userId)?.Username ?? "Sistem";
            var action = new TblUseraction
            {
                UserId = userId,
                Action = $"{username} kullanıcısı, '{deletedUsername}' kullanıcısını sildi.",
                ActionTime = DateTime.Now
            };
            _context.TblUseractions.Add(action);

            // 4️⃣ Değişiklikleri kaydet
            _context.SaveChanges();

            return Json(new { success = true, message = "Kullanıcı başarıyla silindi!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
        }
    }
    [HttpGet]
    public JsonResult GetRoles()
    {
        var roles = _context.TblRoles
                    .Select(r => new { r.RoleId, r.RoleName })
                    .ToList();
        return Json(roles);
    }
    [HttpPost]
    public JsonResult UpdateUserRole(int userId, int roleId)
    {
        try
        {
            var userRole = _context.TblUserRoles.FirstOrDefault(ur => ur.UserId == userId);
            if (userRole != null)
            {
                userRole.RoleId = roleId;
                _context.SaveChanges();

                // Seçilen RoleName'i bul ve dön
                var roleName = _context.TblRoles.FirstOrDefault(r => r.RoleId == roleId)?.RoleName;
                var userIdd = HttpContext.Session.GetInt32("UserID");
                var username = _context.TblUsers.FirstOrDefault(u => u.UserId == userIdd)?.Username ?? "Sistem";
                var username2 = _context.TblUsers.FirstOrDefault(u => u.UserId == userId)?.Username ?? "Sistem";
                var action = new TblUseraction
                {
                    UserId = userIdd,
                    Action = $"{username} kullanıcısı, '{username2}' kullanıcısının rolünü '{roleName}' olarak değiştirdi.",
                    ActionTime = DateTime.Now
                };
                _context.TblUseractions.Add(action);

                // 4️⃣ Değişiklikleri kaydet
                _context.SaveChanges();

                return Json(new { success = true, message = "Kullanıcı rolü başarıyla güncellendi!", newRoleName = roleName });
            }
            else
            {
                return Json(new { success = false, message = "Kullanıcı rolü bulunamadı." });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
        }
    }
    [HttpPost]
    [HttpPost]
public IActionResult LogUserExport(string exportType, string sourceTable)
{
    try
    {
        var userId = HttpContext.Session.GetInt32("UserID");
        if (userId == null)
        {
            return Json(new { success = false, message = "UserID session bilgisi bulunamadı." });
        }

        var username = _context.TblUsers.FirstOrDefault(u => u.UserId == userId)?.Username ?? "Unknown";

        var userAction = new TblUseraction
        {
            UserId = userId,
            Action = $"{username} '{sourceTable}' tablosunu '{exportType}' formatında indirdi.",
            ActionTime = DateTime.Now
        };

        _context.TblUseractions.Add(userAction);
        _context.SaveChanges();

        return Json(new { success = true });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
    }
}