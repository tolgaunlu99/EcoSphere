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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitObservation(ObservationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CreatureNamed = _context.TblCreatures.Select(x => new SelectListItem { Value = x.CreatureId.ToString(), Text = x.ScientificName }).ToList();
                model.Usernamed = _context.TblUsers.Select(x => new SelectListItem { Value = x.UserId.ToString(), Text = x.Name }).ToList();
                model.RegionNamed = _context.TblRegions.Select(x => new SelectListItem { Value = x.RegionId.ToString(), Text = x.RegionName }).ToList();
                return View("AddObservation", model);
            }

            var province = await _context.TblProvinces.FirstOrDefaultAsync(p => p.ProvinceName == model.HiddenProvinceName);
            var district = await _context.TblDistricts.FirstOrDefaultAsync(d => d.DistrictName == model.HiddenDistrictName);

            if (province == null || district == null)
            {
                ModelState.AddModelError("", "Seçilen koordinat için il veya ilçe bulunamadı.");
                return View("AddObservation", model);
            }

            var newObs = new TblMaintable
            {
                CreatureId = model.CreatureId,
                UserId = model.UserId,
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
                GenderId = model.GenderId
            };

            _context.TblMaintables.Add(newObs);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Observation added successfully.";
            return RedirectToAction("AddObservation");
        }

        [HttpGet]
        public IActionResult AddObservation()
        {
            var model = new ObservationViewModel
            {
                CreatureNamed = _context.TblCreatures.Select(x => new SelectListItem { Value = x.CreatureId.ToString(), Text = x.ScientificName }).ToList(),
                Usernamed = _context.TblUsers.Select(x => new SelectListItem { Value = x.UserId.ToString(), Text = x.Name }).ToList(),
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



        [HttpGet]
        public async Task<IActionResult> GetCitysByRegion(int RegionID)
        {
            var cities = await _context.TblProvinces
                .Where(k => k.RegionId == RegionID)
                .Select(x => new SelectListItem(x.ProvinceName, x.ProvinceId.ToString()))
                .ToListAsync();
            return Json(cities);
        }

        [HttpGet]
        public async Task<IActionResult> GetDistrictsByCity(int CityID)
        {
            var districts = await _context.TblDistricts
                .Where(k => k.ProvinceId == CityID)
                .Select(x => new SelectListItem(x.DistrictName, x.DistrictId.ToString()))
                .ToListAsync();
            return Json(districts);
        }

        [HttpGet]
        public async Task<IActionResult> GetLocalitiesByDistrict(int DistrictID)
        {
            var localities = await _context.TblLocalities
                .Where(k => k.DistrictId == DistrictID)
                .Select(x => new SelectListItem(x.LocalityName, x.LocalityId.ToString()))
                .ToListAsync();
            return Json(localities);
        }

        [HttpGet]
        public async Task<IActionResult> GetNeighbourhoodsByLocality(int LocalityID)
        {
            var hoods = await _context.TblNeighbourhoods
                .Where(k => k.LocalityId == LocalityID)
                .Select(x => new SelectListItem(x.HoodName, x.NeighbourhoodId.ToString()))
                .ToListAsync();
            return Json(hoods);
        }

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
            var obs = (from m in _context.TblMaintables
                       join c in _context.TblCreatures on m.CreatureId equals c.CreatureId
                       where m.Id == id
                       select new ObservationViewModel
                       {
                           Id = m.Id,
                           CreatureName = c.ScientificName,
                           Long = m.Long,
                           Lat = m.Lat,
                           SeenTime = m.SeenTime,
                           CreationDate = m.CreationDate
                           // ihtiyacın olan diğer alanları da ekleyebilirsin
                       }).FirstOrDefault();

            if (obs == null)
                return NotFound();

            return View(obs);  // Views/ObservationView/Details.cshtml olacak
        }




    }
}
