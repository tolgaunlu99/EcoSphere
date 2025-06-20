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

            if (UserroleId == null || (UserroleId != 1 && UserroleId != 2 && UserroleId != 3))
            {
                return RedirectToAction("AccessDenied", "UserView");
            }

            return View();
        }
        [HttpPost]
        public IActionResult GetObservations()
        {
            try
            {
                int? roleID = HttpContext.Session.GetInt32("Role_ID");
                var data = ObservationCache.GetCachedObservations();

                // Endemik türleri sadece RoleID 1 (admin) ve 2 (expertr) görebilir
                if (roleID != 1 && roleID != 2)
                {
                    data = data.Where(x => x.EndemicStatusId != 1).ToList(); // ID kontrolü
                }

                return Json(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }
        [HttpPost]
        public IActionResult GetPendingObservations()
        {
            try
            {
                int? roleID = HttpContext.Session.GetInt32("Role_ID");

                var data = _context.VwObservations
                    .Where(x => x.status == 0) // 👈 sadece pending (status = 0)
                    .AsQueryable();

                // Eğer kullanıcı admin (1) ya da expert (2) değilse endemikleri gösterme
                if (roleID != 1 && roleID != 2)
                {
                    data = data.Where(x => x.EndemicStatus != "True"); // veya EndemicStatusId != 1 ise
                }

                var result = data.Select(v => new
                {
                    v.Id,
                    v.ScientificName,
                    v.Username,
                    v.ProvinceName,
                    v.DistrictName,
                    v.EndemicStatus,
                    v.ProjectName,
                    v.ReferenceName,
                    v.LocationType,
                    v.GenderName,
                    v.Lat,
                    v.Long,
                    v.Activity,
                    v.SeenTime,
                    v.CreationDate                    
                }).ToList();

                return Json(result);
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
                CitationId = 1,
                ReferenceId = model.ReferenceId,
                LocationTypeId = 1,
                LocationRangeId = 1,
                GenderId = model.GenderId,
                ImagePath = savedFileName, // sadece dosya adı (örneğin: abc123.jpg)
                status = (roleID == 5) ? 0 : 1
            };

            _context.TblMaintables.Add(newObs);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Gözlem başarıyla eklendi.";
            var action = new TblUseraction
            {
                UserId = userId,
                Action = username + " adlı kullanıcı " + newObs.Id + " ID'li gözlemi ekledi!",
                ActionTime = DateTime.Now
            };

            _context.TblUseractions.Add(action);
            _context.SaveChanges();
            // ✅ Cache'e de ekle
            var addedCreature = _context.TblCreatures.FirstOrDefault(c => c.CreatureId == newObs.CreatureId);
            var addedProject = _context.TblProjects.FirstOrDefault(p => p.ProjectId == newObs.ProjectId);
            var addedReference = _context.TblReferences.FirstOrDefault(r => r.ReferenceId == newObs.ReferenceId);
            var addedLocationType = _context.TblLocationtypes.FirstOrDefault(l => l.LocationTypeId == newObs.LocationTypeId);
            var addedGender = _context.TblGenders.FirstOrDefault(g => g.GenderId == newObs.GenderId);
            var addedEndemic = _context.TblEndemicstatuses.FirstOrDefault(e => e.EndemicStatusId == newObs.EndemicStatusId);
            if (newObs.status == 1)
            {
                ObservationCache.AddObservation(new ObservationViewModel
            {
                Id = newObs.Id,
                CreatureName = addedCreature?.ScientificName,
                UserName = username,
                provincename = province.ProvinceName,
                DistrictName = district.DistrictName,
                EndemicStatName = addedEndemic?.EndemicStatus,
                ProjectName = addedProject?.ProjectName,
                ReferenceName = addedReference?.ReferenceName,
                LocationType = addedLocationType?.LocationType,
                GenderName = addedGender?.GenderName,
                Lat = newObs.Lat,
                Long = newObs.Long,
                Activity = newObs.Activity,
                SeenTime = newObs.SeenTime,
                CreationDate = newObs.CreationDate

            });
        }
            return RedirectToAction("AddObservation");
        }
        [HttpPost]
        public JsonResult AddProject(string projectName)
        {
            var existing = _context.TblProjects.FirstOrDefault(x => x.ProjectName == projectName);
            if (existing != null)
            {
                return Json(new { success = false, message = "Bu proje zaten var." });
            }

            var newProject = new TblProject { ProjectName = projectName };
            _context.TblProjects.Add(newProject);
            _context.SaveChanges();
            var userId = HttpContext.Session.GetInt32("UserID");
            var username = _context.TblUsers.FirstOrDefault(u => u.UserId == userId)?.Username;
            var action = new TblUseraction
            {
                UserId = userId,
                Action = username + " adlı kullanıcı " + projectName + " Adındaki Projeyi ekledi!",
                ActionTime = DateTime.Now
            };

            _context.TblUseractions.Add(action);
            _context.SaveChanges();

            return Json(new { success = true, id = newProject.ProjectId, name = newProject.ProjectName });
        }

        [HttpPost]
        public JsonResult AddReference(string referenceName)
        {
            var existing = _context.TblReferences.FirstOrDefault(x => x.ReferenceName == referenceName);
            if (existing != null)
            {
                return Json(new { success = false, message = "Bu referans zaten var." });
            }
            var newReference = new TblReference { ReferenceName = referenceName };
            _context.TblReferences.Add(newReference);
            _context.SaveChanges();
            var userId = HttpContext.Session.GetInt32("UserID");
            var username = _context.TblUsers.FirstOrDefault(u => u.UserId == userId)?.Username;
            var action = new TblUseraction
            {
                UserId = userId,
                Action = username + " adlı kullanıcı " + newReference + " Adındaki Referansı ekledi!",
                ActionTime = DateTime.Now
            };

            _context.TblUseractions.Add(action);
            _context.SaveChanges();
            return Json(new { success = true, id = newReference.ReferenceId, name = newReference.ReferenceName });
        }
        [HttpGet]
        public IActionResult AddObservation()
        {
            int? UserroleId = HttpContext.Session.GetInt32("Role_ID");

            if (UserroleId == null || (UserroleId != 1 && UserroleId != 2 && UserroleId != 3 && UserroleId != 5)) // 1=Admin, 2=Expert , 3=Observer,5=Volunteer
            {
                return RedirectToAction("AccessDenied", "UserView");
            }
            var roleID = GetCurrentUserRoleId();
            ViewBag.UserRoleId = roleID;
            var model = new ObservationViewModel
            {
                CreatureNamed = CreaturesCache.GetCachedCreatures().Select(x => new SelectListItem {  Value = x.CreatureId.ToString(),Text = x.ScientificName ?? x.SpeciesName}).ToList(),
                RegionNamed = _context.TblRegions.Select(x => new SelectListItem { Value = x.RegionId.ToString(), Text = x.RegionName }).ToList(),
                ProjectNamed = _context.TblProjects.Select(x => new SelectListItem { Value = x.ProjectId.ToString(), Text = x.ProjectName }).ToList(),
                ReferenceNamed=_context.TblReferences.Select(x => new SelectListItem { Value = x.ReferenceId.ToString(), Text = x.ReferenceName }).ToList(),    
            };

            return View(model);
        }
        [HttpGet]
        public JsonResult GetObservationCountByProvince(string province)
        {
            int? roleID = HttpContext.Session.GetInt32("Role_ID");

            var query = _context.VwMaps.Where(v => v.ProvinceName == province && v.status == 1);

            if (roleID != 1 && roleID != 2)
            {
                query = query.Where(v => v.EndemicstatID != 1);
            }

            int count = query.Count();
            return Json(new { count });
        }

        [HttpGet]
        public JsonResult GetObservationCountByDistrict(string district)
        {
            int? roleID = HttpContext.Session.GetInt32("Role_ID");

            var query = _context.VwMaps.Where(v => v.DistrictName == district && v.status == 1);

            if (roleID != 1 && roleID != 2)
            {
                query = query.Where(v => v.EndemicstatID != 1);
            }

            int count = query.Count();
            return Json(new { count });
        }
        [HttpGet]
        public JsonResult GetPlantAnimalCountsByProvince(string province)
        {
            int? roleID = HttpContext.Session.GetInt32("Role_ID");

            var query = _context.VwMaps.Where(v => v.ProvinceName == province && v.status == 1);

            if (roleID != 1 && roleID != 2)
                query = query.Where(v => v.EndemicstatID != 1);

            int plantCount = query.Count(v => v.KingdomName == "Plantae");
            int animalCount = query.Count(v => v.KingdomName == "Animalia");

            return Json(new { plantCount, animalCount });
        }
        [HttpGet]
        public JsonResult GetCreatureCountsByProvince(string province)
        {
            int? roleID = HttpContext.Session.GetInt32("Role_ID");

            var query = _context.VwMaps
                .Where(v => v.ProvinceName == province && v.status == 1);

            if (roleID != 1 && roleID != 2)
                query = query.Where(v => v.EndemicstatID != 1);

            var distinctCreatures = query
                .GroupBy(v => v.CreatureId)
                .Select(g => g.Key)
                .ToList();

            int totalCreatures = distinctCreatures.Count;

            return Json(new { totalCreatures });
        }

        [HttpGet]
        public JsonResult GetCreatureCountsByDistrict(string district)
        {
            int? roleID = HttpContext.Session.GetInt32("Role_ID");

            var query = _context.VwMaps
                .Where(v => v.DistrictName == district && v.status == 1);

            if (roleID != 1 && roleID != 2)
                query = query.Where(v => v.EndemicstatID != 1);

            var distinctCreatures = query
                .GroupBy(v => v.CreatureId)
                .Select(g => g.Key)
                .ToList();

            int totalCreatures = distinctCreatures.Count;

            return Json(new { totalCreatures });
        }

        [HttpGet]
        public JsonResult GetPlantAnimalCountsByDistrict(string district)
        {
            int? roleID = HttpContext.Session.GetInt32("Role_ID");

            var query = _context.VwMaps.Where(v => v.DistrictName == district && v.status == 1);

            if (roleID != 1 && roleID != 2)
                query = query.Where(v => v.EndemicstatID != 1);

            int plantCount = query.Count(v => v.KingdomName == "Plantae");
            int animalCount = query.Count(v => v.KingdomName == "Animalia");

            return Json(new { plantCount, animalCount });
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
            // ✅ Cache'den de sil
            ObservationCache.RemoveObservation(id);

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetObservationsByProvince(string province)
        {
            int? roleID = HttpContext.Session.GetInt32("Role_ID");

            var query = _context.VwMaps
                .Where(v => v.Lat != null
                            && v.Long != null
                            && v.ProvinceName == province
                            && v.status == 1);

            // Sadece admin (1) ve expert (2) endemikleri görebilsin
            if (roleID != 1 && roleID != 2)
            {
                query = query.Where(v => v.EndemicstatID != 1);
            }

            var observations = query
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
                .ToList();

            return Json(observations);
        }


        [HttpGet]
        public IActionResult GetObservationsByDistrict(string district)
        {
            int? roleID = HttpContext.Session.GetInt32("Role_ID");

            var query = _context.VwMaps
                .Where(v => v.Lat != null
                            && v.Long != null
                            && v.DistrictName == district
                            && v.status == 1);

            if (roleID != 1 && roleID != 2)
            {
                query = query.Where(v => v.EndemicstatID != 1);
            }

            var observations = query
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
                .ToList();

            return Json(observations);
        }

        [HttpPost]
        public JsonResult Approve(int id)
        {
            var main = _context.TblMaintables.FirstOrDefault(o => o.Id == id);
            if (main == null)
                return Json(new { success = false, message = "Kayıt bulunamadı." });

            main.status = 1;
            _context.SaveChanges(); // ✅ burada gerçek update gerçekleşiyor

            // Şimdi view'den bilgiyi çekip cache'e ekle
            var newCacheEntry = _context.VwObservations.FirstOrDefault(o => o.Id == id);
            if (newCacheEntry != null)
            {
                ObservationCache.AddObservation(new ObservationViewModel
                {
                    Id = newCacheEntry.Id,
                    CreatureName = newCacheEntry.ScientificName,
                    UserName = newCacheEntry.Username,
                    provincename = newCacheEntry.ProvinceName,
                    DistrictName = newCacheEntry.DistrictName,
                    EndemicStatName = newCacheEntry.EndemicStatus,
                    ProjectName = newCacheEntry.ProjectName,
                    ReferenceName = newCacheEntry.ReferenceName,
                    LocationType = newCacheEntry.LocationType,
                    GenderName = newCacheEntry.GenderName,
                    Lat = newCacheEntry.Lat,
                    Long = newCacheEntry.Long,
                    Activity = newCacheEntry.Activity,
                    SeenTime = newCacheEntry.SeenTime,
                    CreationDate = newCacheEntry.CreationDate
                });
            }

            var userId = HttpContext.Session.GetInt32("UserID");
            var username = _context.TblUsers.FirstOrDefault(u => u.UserId == userId)?.Username ?? "Unknown";

            _context.TblUseractions.Add(new TblUseraction
            {
                UserId = userId ?? 0,
                Action = $"{id} ID'li gözlem {username} tarafından onaylandı.",
                ActionTime = DateTime.Now
            });

            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var roleID = GetCurrentUserRoleId();
            ViewBag.UserRoleId = roleID;

            var observation = _context.VwObservations.FirstOrDefault(o => o.Id == id);

            if (observation == null)
                return NotFound();

            // 🌟 Endemikse ve yetkili rol değilse AccessDenied'a yönlendir
            if (observation.EndemicStatus == "True" && roleID != 1 && roleID != 2)
            {
                return RedirectToAction("AccessDenied", "UserView");
            }

            return View(observation);
        }





    }
}