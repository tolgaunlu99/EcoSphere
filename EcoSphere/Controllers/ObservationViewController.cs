using EcoSphere.Models;
using Microsoft.AspNetCore.Mvc;
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
    }
}