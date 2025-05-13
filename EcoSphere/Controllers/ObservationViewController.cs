using EcoSphere.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.IO; // GeoJSON için gerekli

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
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            int? UserroleId = HttpContext.Session.GetInt32("Role_ID");

            if (UserroleId == null || (UserroleId != 1 && UserroleId != 2 && UserroleId !=3)) // 1=Admin, 2=Expert
            {
                return RedirectToAction("AccessDenied", "UserView");
            }
            var roleID = GetCurrentUserRoleId();
            ViewBag.UserRoleId = roleID;
            var observationsWithNames =
                from m in _context.TblMaintables
                join c in _context.TblCreatures on m.CreatureId equals c.CreatureId
                join u in _context.TblUsers on m.UserId equals u.UserId
                join p in _context.TblProvinces on m.CityId equals p.ProvinceId
                join d in _context.TblDistricts on m.DistrictId equals d.DistrictId
                join ms in _context.TblMigrationstatuses on m.MigrationStatusId equals ms.MigrationStatusId
                join es in _context.TblEndemicstatuses on m.EndemicStatusId equals es.EndemicStatusId
                join pr in _context.TblProjects on m.ProjectId equals pr.ProjectId
                join ci in _context.TblCitations on m.CitationId equals ci.CitationId
                join re in _context.TblReferences on m.ReferenceId equals re.ReferenceId
                join lt in _context.TblLocationtypes on m.LocationTypeId equals lt.LocationTypeId
                join lr in _context.TblLocationranges on m.LocationRangeId equals lr.LocationRangeId
                join g in _context.TblGenders on m.GenderId equals g.GenderId

                // LEFT JOIN yapılan kısımlar
                join r in _context.TblRegions on m.RegionId equals r.RegionId into regionGroup
                from r in regionGroup.DefaultIfEmpty()

                join l in _context.TblLocalities on m.LocalityId equals l.LocalityId into localityGroup
                from l in localityGroup.DefaultIfEmpty()

                join n in _context.TblNeighbourhoods on m.NeighborhoodId equals n.NeighbourhoodId into hoodGroup
                from n in hoodGroup.DefaultIfEmpty()

                select new ObservationViewModel
                {
                    Id = m.Id,
                    CreatureName = c.ScientificName,
                    UserName = u.Name,
                    UsersurName = u.Surname,
                    RegionName = r != null ? r.RegionName : "",
                    provincename = p.ProvinceName,
                    DistrictName = d.DistrictName,
                    LocalityName = l != null ? l.LocalityName : "",
                    HoodName = n != null ? n.HoodName : "",
                    MigrationStatName = ms.MigrationStatusName,
                    EndemicStatName = es.EndemicStatus,
                    ProjectName = pr.ProjectName,
                    CitationName = ci.CitationName,
                    ReferenceName = re.ReferenceName,
                    LocationType = lt.LocationType,
                    LocationRange = lr.LocationRangeValue,
                    GenderName = g.GenderName,
                    Long = m.Long,
                    Lat = m.Lat,
                    Activity = m.Activity,
                    SeenTime = m.SeenTime,
                    CreationDate = m.CreationDate
                };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                observationsWithNames = observationsWithNames.Where(o =>
                    o.CreatureName.Contains(searchTerm) ||
                    o.provincename.Contains(searchTerm));
            }

            ViewData["SearchTerm"] = searchTerm;
            var viewModel = await observationsWithNames.ToListAsync();
            return View(viewModel);
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
            var username = _context.TblUsers.FirstOrDefault(u => u.UserId == userId)?.Username;          

            _context.TblMaintables.Remove(obs);
            _context.SaveChanges();
            var action = new TblUseraction
            {
                UserId = userId,
                Action = id + "'li Kayıt " + username + " tarafından Silindi",
                ActionTime = DateTime.Now
            };

            _context.TblUseractions.Add(action);
            _context.SaveChanges();

            return Json(new { success = true });
        }


        //[HttpGet]
        //public async Task<IActionResult> GetCitysByRegion(int RegionID)
        //{
        //    var cities = await _context.TblProvinces
        //        .Where(k => k.RegionId == RegionID)
        //        .Select(x => new SelectListItem(x.ProvinceName, x.ProvinceId.ToString()))
        //        .ToListAsync();
        //    return Json(cities);
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetDistrictsByCity(int CityID)
        //{
        //    var districts = await _context.TblDistricts
        //        .Where(k => k.ProvinceId == CityID)
        //        .Select(x => new SelectListItem(x.DistrictName, x.DistrictId.ToString()))
        //        .ToListAsync();
        //    return Json(districts);
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetLocalitiesByDistrict(int DistrictID)
        //{
        //    var localities = await _context.TblLocalities
        //        .Where(k => k.DistrictId == DistrictID)
        //        .Select(x => new SelectListItem(x.LocalityName, x.LocalityId.ToString()))
        //        .ToListAsync();
        //    return Json(localities);
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetNeighbourhoodsByLocality(int LocalityID)
        //{
        //    var hoods = await _context.TblNeighbourhoods
        //        .Where(k => k.LocalityId == LocalityID)
        //        .Select(x => new SelectListItem(x.HoodName, x.NeighbourhoodId.ToString()))
        //        .ToListAsync();
        //    return Json(hoods);
        //}

        [HttpGet]
        public IActionResult GetObservationsByProvince(string province)
        {
            var observations = (from m in _context.TblMaintables
                                join c in _context.TblCreatures on m.CreatureId equals c.CreatureId
                                join p in _context.TblProvinces on m.CityId equals p.ProvinceId
                                where m.Lat != null && m.Long != null && p.ProvinceName == province
                                select new
                                {
                                    Id = m.Id,
                                    Lat = m.Lat,
                                    Long = m.Long,
                                    Name = c.ScientificName,
                                    SeenTime = m.SeenTime  // <<< burayı ekledik
                                }).ToList();

            return Json(observations);
        }

        [HttpGet]
        public IActionResult GetObservationsByDistrict(string district)
        {
            var observations = (from m in _context.TblMaintables
                                join c in _context.TblCreatures on m.CreatureId equals c.CreatureId
                                join d in _context.TblDistricts on m.DistrictId equals d.DistrictId
                                where m.Lat != null && m.Long != null && d.DistrictName == district
                                select new
                                {
                                    Id = m.Id,
                                    Lat = m.Lat,
                                    Long = m.Long,
                                    Name = c.ScientificName,
                                    SeenTime = m.SeenTime  // <<< burayı da
                                }).ToList();

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
