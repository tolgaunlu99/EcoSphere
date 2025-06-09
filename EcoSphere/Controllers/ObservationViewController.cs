using EcoSphere.Caching;
using EcoSphere.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO; // GeoJSON için gerekli
using System.Linq;
using System.Threading.Tasks;

namespace EcoSphere.Controllers
{
    public class ObservationViewController : Controller
    {
        private readonly MyDbContext _context;

        public ObservationViewController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var roleID = GetCurrentUserRoleId();
            ViewBag.UserRoleId = roleID;
            int? UserroleId = HttpContext.Session.GetInt32("Role_ID");

            if (UserroleId == null || (UserroleId != 1 && UserroleId != 2))
            {
                return RedirectToAction("AccessDenied", "UserView");
            }

            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> GetObservations()
        //{
        //    try
        //    {
        //        var data = await _context.VwObservations
        //            .Select(v => new ObservationViewModel
        //            {
        //                Id = v.Id,  // ID'yi buraya ekledik
        //                CreatureName = v.ScientificName,
        //                UserName = v.Username,
        //                provincename = v.ProvinceName,
        //                DistrictName = v.DistrictName,
        //                EndemicStatName = v.EndemicStatus,
        //                ProjectName = v.ProjectName,
        //                ReferenceName = v.ReferenceName,
        //                LocationType = v.LocationType,
        //                GenderName = v.GenderName,
        //                Lat = v.Lat,
        //                Long = v.Long,
        //                Activity = v.Activity,
        //                SeenTime = v.SeenTime,
        //                CreationDate = v.CreationDate
        //            })
        //            .ToListAsync();

        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
        //    }
        //}
        [HttpPost]
        public IActionResult GetObservations()
        {
            try
            {
                var data = ObservationCache.GetCachedObservations();
                return Json(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }


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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitObservation(ObservationViewModel model)
        {
            var roleID = GetCurrentUserRoleId();
            ViewBag.UserRoleId = roleID;

            if (!ModelState.IsValid)
            {
                model.CreatureNamed = _context.TblCreatures.Select(x => new SelectListItem { Value = x.CreatureId.ToString(), Text = x.ScientificName }).ToList();
                model.Usernamed = _context.TblUsers.Select(x => new SelectListItem { Value = x.UserId.ToString(), Text = x.Name }).ToList();
                model.RegionNamed = _context.TblRegions.Select(x => new SelectListItem { Value = x.RegionId.ToString(), Text = x.RegionName }).ToList();
                return View("AddObservation", model);
            }

            string? savedFileName = null;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                // Benzersiz dosya adı oluştur
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                savedFileName = uniqueFileName; // Veritabanına sadece dosya adını kaydedeceğiz
            }

            var province = await _context.TblProvinces.FirstOrDefaultAsync(p => p.ProvinceName == model.HiddenProvinceName);
            var district = await _context.TblDistricts.FirstOrDefaultAsync(d => d.DistrictName == model.HiddenDistrictName);

            if (province == null || district == null)
            {
                ModelState.AddModelError("", "Seçilen koordinat için il veya ilçe bulunamadı.");
                return View("AddObservation", model);
            }

            var userId = HttpContext.Session.GetInt32("UserID");
            var username = _context.TblUsers.FirstOrDefault(u => u.UserId == userId)?.Username;

            var newObs = new TblMaintable
            {
                CreatureId = model.CreatureId,
                UserId = userId,
                CityId = province.ProvinceId,
                DistrictId = district.DistrictId,
                Long = model.Long,
                Lat = model.Lat,
                Activity = model.Activity,
                SeenTime = model.SeenTime,
                CreationDate = DateTime.UtcNow,
                RegionId = null,
                LocalityId = null,
                NeighborhoodId = null,
                MigrationStatusId = model.MigrationStatusId,
                EndemicStatusId = model.EndemicStatusId,
                ProjectId = model.ProjectId,
                CitationId = model.CitationId,
                ReferenceId = model.ReferenceId,
                LocationTypeId = model.LocationTypeId,
                LocationRangeId = model.LocationRangeId,
                GenderId = model.GenderId,
                ImagePath = savedFileName // sadece dosya adı (örneğin: abc123.jpg)
            };

            _context.TblMaintables.Add(newObs);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Observation added successfully.";
            var action = new TblUseraction
            {
                UserId = userId,
                Action = username + " adlı kullanıcı " + newObs.Id + " ID'li gözlemi ekledi!",
                ActionTime = DateTime.Now
            };

            _context.TblUseractions.Add(action);
            _context.SaveChanges();

            return RedirectToAction("AddObservation");
        }

        [HttpGet]
        public IActionResult AddObservation()
        {
            int? UserroleId = HttpContext.Session.GetInt32("Role_ID");

            if (UserroleId == null || (UserroleId != 1 && UserroleId != 2 && UserroleId != 3)) // 1=Admin, 2=Expert , 3=Observer
            {
                return RedirectToAction("AccessDenied", "UserView");
            }
            var roleID = GetCurrentUserRoleId();
            ViewBag.UserRoleId = roleID;
            var model = new ObservationViewModel
            {
                CreatureNamed = _context.TblCreatures.Select(x => new SelectListItem { Value = x.CreatureId.ToString(), Text = x.ScientificName }).ToList(),
                RegionNamed = _context.TblRegions.Select(x => new SelectListItem { Value = x.RegionId.ToString(), Text = x.RegionName }).ToList(),
                MigrationstatNamed = _context.TblMigrationstatuses.Select(x => new SelectListItem { Value = x.MigrationStatusId.ToString(), Text = x.MigrationStatusName }).ToList(),
                EndemicstatNamed = _context.TblEndemicstatuses.Select(x => new SelectListItem { Value = x.EndemicStatusId.ToString(), Text = x.EndemicStatus }).ToList(),
                ProjectNamed = _context.TblProjects.Select(x => new SelectListItem { Value = x.ProjectId.ToString(), Text = x.ProjectName }).ToList(),
                CitationNamed = _context.TblCitations.Select(x => new SelectListItem { Value = x.CitationId.ToString(), Text = x.CitationName }).ToList(),
                ReferenceNamed = _context.TblReferences.Select(x => new SelectListItem { Value = x.ReferenceId.ToString(), Text = x.ReferenceName }).ToList(),
                LocationtypeNamed = _context.TblLocationtypes.Select(x => new SelectListItem { Value = x.LocationTypeId.ToString(), Text = x.LocationType }).ToList(),
                LocationRangeNamed = _context.TblLocationranges.Select(x => new SelectListItem { Value = x.LocationRangeId.ToString(), Text = x.LocationRangeValue }).ToList(),
                GenderNamed = _context.TblGenders.Select(x => new SelectListItem { Value = x.GenderId.ToString(), Text = x.GenderName }).ToList()
            };

            return View(model);
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var obs = _context.TblMaintables.FirstOrDefault(o => o.Id == id);
            if (obs == null)
            {
                return Json(new { success = false, message = "Kayıt bulunamadı." });
            }

            var userId = HttpContext.Session.GetInt32("UserID");
            var username = _context.TblUsers.FirstOrDefault(u => u.UserId == userId)?.Username ?? "Unknown";

            _context.TblMaintables.Remove(obs);

            var action = new TblUseraction
            {
                UserId = userId ?? 0,
                Action = $"{id} ID'li kayıt {username} tarafından silindi.",
                ActionTime = DateTime.Now
            };

            _context.TblUseractions.Add(action);

            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetObservationsByProvince(string province)
        {
            int? roleID = HttpContext.Session.GetInt32("Role_ID");

            // Endemic Status kontrolü eğer ihtiyaç varsa (şu an View'da bu kolon yok gibi görünüyor)
            // Bu kısım VwMap view'ına eklenmediyse veya gerekmiyorsa iptal edilebilir.

            // Sorgu
            var observations = _context.VwMaps
                .Where(v => v.Lat != null
                            && v.Long != null
                            && v.ProvinceName == province)
                .OrderByDescending(v => v.SeenTime)
                .Select(v => new
                {
                    v.Id,
                    v.Lat,
                    v.Long,
                    Name = v.ScientificName,
                    Kingdom = v.KingdomName,
                    SeenTime = v.SeenTime
                })
                .ToList();  // <-- Tüm verileri getir

            return Json(observations);
        }


        [HttpGet]
        public IActionResult GetObservationsByDistrict(string district)
        {
            int? roleID = HttpContext.Session.GetInt32("Role_ID");

            // Burada EndemicStatusId kontrolü View'da yoksa atlayabiliriz;
            // VwMap'e eklenirse role bazlı filtreyi kullanabiliriz.

            var observations = _context.VwMaps
                .Where(v => v.Lat != null
                            && v.Long != null
                            && v.DistrictName == district)
                .OrderByDescending(v => v.SeenTime)
                .Select(v => new
                {
                    v.Id,
                    v.Lat,
                    v.Long,
                    Name = v.ScientificName,
                    Kingdom = v.KingdomName,
                    SeenTime = v.SeenTime
                })
                .ToList();  // <-- Tüm verileri getir

            return Json(observations);
        }



        [HttpGet]
        public IActionResult Details(int id)
        {
            var roleID = GetCurrentUserRoleId();
            ViewBag.UserRoleId = roleID;

            var obs = (from m in _context.TblMaintables
                       join c in _context.TblCreatures on m.CreatureId equals c.CreatureId
                       join u in _context.TblUsers on m.UserId equals u.UserId
                       where m.Id == id
                       select new ObservationViewModel
                       {
                           Id = m.Id,
                           CreatureName = c.ScientificName,
                           Long = m.Long,
                           Lat = m.Lat,
                           SeenTime = m.SeenTime,
                           CreationDate = m.CreationDate,
                           ImagePath = m.ImagePath, // yalnızca dosya adı olmalı, örn: "abc.jpg"
                           UserName = u.Name,
                           UsersurName = u.Surname
                       }).FirstOrDefault();

            if (obs == null)
                return NotFound();

            return View(obs);
        }





    }
}