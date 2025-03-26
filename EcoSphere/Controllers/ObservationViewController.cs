using EcoSphere.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            var observationsWithNames = from m in _context.TblMaintables
                                        join c in _context.TblCreatures on m.CreatureId equals c.CreatureId
                                        join u in _context.TblUsers on m.UserId equals u.UserId
                                        join r in _context.TblRegions on m.RegionId equals r.RegionId
                                        join p in _context.TblProvinces on m.CityId equals p.ProvinceId
                                        join d in _context.TblDistricts on m.DistrictId equals d.DistrictId
                                        join l in _context.TblLocalities on m.LocalityId equals l.LocalityId
                                        join n in _context.TblNeighbourhoods on m.NeighborhoodId equals n.NeighbourhoodId
                                        join ms in _context.TblMigrationstatuses on m.MigrationStatusId equals ms.MigrationStatusId
                                        join es in _context.TblEndemicstatuses on m.EndemicStatusId equals es.EndemicStatusId
                                        join pr in _context.TblProjects on m.ProjectId equals pr.ProjectId
                                        join ci in _context.TblCitations on m.CitationId equals ci.CitationId
                                        join re in _context.TblReferences on m.ReferenceId equals re.ReferenceId
                                        join lt in _context.TblLocationtypes on m.LocationTypeId equals lt.LocationTypeId
                                        join lr in _context.TblLocationranges on m.LocationRangeId equals lr.LocationRangeId
                                        join g in _context.TblGenders on m.GenderId equals g.GenderId
                                        select new ObservationViewModel
                                        {
                                            Id = m.Id,
                                            CreatureName = c.ScientificName,
                                            UserName = u.Name,
                                            UsersurName = u.Surname,
                                            RegionName = r.RegionName,
                                            provincename = p.ProvinceName,
                                            DistrictName = d.DistrictName,
                                            LocalityName = l.LocalityName,
                                            HoodName = n.HoodName,
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
        public ActionResult AddObservation()
        {
            // creatures combobox verisi
            var creatures = _context.TblCreatures
                .Select(ur => new SelectListItem
                {
                    Value = ur.CreatureId.ToString(),
                    Text = ur.ScientificName,
                }).ToList();

            // User combobox verisi
            var user = _context.TblUsers
                .Select(k => new SelectListItem
                {
                    Value = k.UserId.ToString(),
                    Text = k.Name 
                }).ToList();
           
            var region = _context.TblRegions
                .Select(p => new SelectListItem
                {
                    Value = p.RegionId.ToString(),
                    Text = p.RegionName
                }).ToList();

            var migrationstat = _context.TblMigrationstatuses
                .Select(s => new SelectListItem
                {
                    Value = s.MigrationStatusId.ToString(),
                    Text = s.MigrationStatusName
                }).ToList();

            var endemicstat = _context.TblEndemicstatuses
                .Select(ss => new SelectListItem
                {
                    Value = ss.EndemicStatusId.ToString(),
                    Text = ss.EndemicStatus
                }).ToList();

            var project = _context.TblProjects
                .Select(i => new SelectListItem
                {
                    Value = i.ProjectId.ToString(),
                    Text = i.ProjectName
                }).ToList();
            var citation = _context.TblCitations
                .Select(i => new SelectListItem
                {
                    Value = i.CitationId.ToString(),
                    Text = i.CitationName
                }).ToList();
            var reference = _context.TblReferences
                .Select(i => new SelectListItem
                {
                    Value = i.ReferenceId.ToString(),
                    Text = i.ReferenceName
                }).ToList();
            var locationtype = _context.TblLocationtypes
                .Select(i => new SelectListItem
                {
                    Value = i.LocationTypeId.ToString(),
                    Text = i.LocationType
                }).ToList();
            var locationrange = _context.TblLocationranges
                .Select(i => new SelectListItem
                {
                    Value = i.LocationRangeId.ToString(),
                    Text = i.LocationRangeValue
                }).ToList();
            var gender = _context.TblGenders
                .Select(i => new SelectListItem
                {
                    Value = i.GenderId.ToString(),
                    Text = i.GenderName
                }).ToList();

            // ViewModel'e tüm combobox verilerini gönderiyoruz
            var model = new ObservationViewModel
            {
                CreatureNamed = creatures,
                Usernamed = user,
                RegionNamed = region,
                MigrationstatNamed = migrationstat,
                EndemicstatNamed = endemicstat,
                ProjectNamed = project,
                CitationNamed = citation,
                ReferenceNamed = reference,
                LocationtypeNamed = locationtype,
                LocationRangeNamed = locationrange,
                GenderNamed = gender
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetCitysByRegion(int RegionID)
        {
            var cities = await _context.TblProvinces
                .Where(k => k.RegionId == RegionID)
                .Select(k => new SelectListItem
                {
                    Value = k.ProvinceId.ToString(),
                    Text = k.ProvinceName
                }).ToListAsync();

            return Json(cities);
        }
        [HttpGet]
        public async Task<IActionResult> GetDistrictsByCity(int CityID)
        {
            var Districts = await _context.TblDistricts
                .Where(k => k.ProvinceId == CityID)
                .Select(k => new SelectListItem
                {
                    Value = k.DistrictId.ToString(),
                    Text = k.DistrictName
                }).ToListAsync();

            return Json(Districts);
        }
        [HttpGet]
        public async Task<IActionResult> GetLocalitiesByDistrict(int DistrictID)
        {
            var Localities = await _context.TblLocalities
                .Where(k => k.DistrictId == DistrictID)
                .Select(k => new SelectListItem
                {
                    Value = k.LocalityId.ToString(),
                    Text = k.LocalityName
                }).ToListAsync();
            return Json(Localities);
        }
        [HttpGet]
        public async Task<IActionResult> GetNeighbourhoodsByLocality(int LocalityID)
        {
            var Neighbourhoods = await _context.TblNeighbourhoods
                .Where(k => k.LocalityId == LocalityID)
                .Select(k => new SelectListItem
                {
                    Value = k.NeighbourhoodId.ToString(),
                    Text = k.HoodName
                }).ToListAsync();
            return Json(Neighbourhoods);
        }
        [HttpPost]
        public async Task<IActionResult> AddObservation(ObservationViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                // Eğer Project ekleniyorsa
                if (!string.IsNullOrEmpty(model.ProjectName2))
                {
                    var newProject = new TblProject
                    {
                        ProjectName = model.ProjectName2
                    };
                    _context.TblProjects.Add(newProject);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage2"] = "Project added successfully.";
                }
                // Eğer Citation ekleniyorsa
                if (!string.IsNullOrEmpty(model.CitationName2))
                {
                    var newCitation = new TblCitation
                    {
                        CitationName = model.CitationName2
                    };
                    _context.TblCitations.Add(newCitation);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage2"] = "Citation added successfully.";
                }
                // Eğer Reference ekleniyorsa
                if (!string.IsNullOrEmpty(model.ReferenceName2))
                {
                    var newReference = new TblReference
                    {
                        ReferenceName = model.ReferenceName2
                    };
                    _context.TblReferences.Add(newReference);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage2"] = "Reference added successfully.";
                }
                // Eğer Reference ekleniyorsa
                if (!string.IsNullOrEmpty(model.LocationRange2))
                {
                    var newLocationRange = new TblLocationrange
                    {
                        LocationRangeValue = model.LocationRange2
                    };
                    _context.TblLocationranges.Add(newLocationRange);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage2"] = "Location Range added successfully.";
                }


                return RedirectToAction("AddObservation");
            }

            TempData["ErrorMessage"] = "There was an error saving the data.";
            return RedirectToAction("AddObservation");
        }


    }
}