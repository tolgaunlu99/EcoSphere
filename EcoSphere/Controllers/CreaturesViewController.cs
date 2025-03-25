using EcoSphere.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
//merhaba togla
namespace EcoSphere.Controllers
{
    public class CreaturesViewController : Controller
    {
        private readonly MyDbContext _context;

        public CreaturesViewController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var creaturesWithNames = from c in _context.TblCreatures
                                     join ur in _context.TblUpperrealms on c.UpperRealmId equals ur.RealmId
                                     join k in _context.TblKingdoms on c.KingdomId equals k.KingdomId
                                     join p in _context.TblPhylums on c.PhylumId equals p.PhylumId
                                     join cl in _context.TblClasses on c.ClassId equals cl.ClassId
                                     join o in _context.TblOrders on c.OrderId equals o.OrderId
                                     join f in _context.TblFamilies on c.FamilyId equals f.FamilyId
                                     join g in _context.TblGenus on c.GenusId equals g.GenusId
                                     join s in _context.TblSpecies on c.SpeciesId equals s.SpeciesId
                                     join ss in _context.TblSubspecies on c.SubspeciesId equals ss.SubspeciesId
                                     join i in _context.TblIucns on c.IucnId equals i.IucnId
                                     join a in _context.TblSpeciesauthors on c.AuthorId equals a.AuthorId
                                     select new CreaturesViewModel
                                     {
                                         CreatureId = c.CreatureId,
                                         UpperRealmName = ur.RealmName,
                                         KingdomName = k.KingdomName,
                                         PhylumName = p.PhylumName,
                                         ClassName = cl.ClassName,
                                         OrderName = o.OrderName,
                                         FamilyName = f.FamilyName,
                                         GenusName = g.GenusName,
                                         SpeciesName = s.SpeciesName,
                                         SubspeciesName = ss.SubspeciesName,
                                         IucnCode = i.IucnCode,
                                         AuthorName = a.AuthorName,
                                         ScientificName = c.ScientificName,
                                         CommonName = c.CommonName
                                     };

            var viewModel = await creaturesWithNames.ToListAsync();
            return View(viewModel);
        }


        [HttpGet]
        public IActionResult AddCreature()
        {
            var upperRealms = _context.TblUpperrealms
                .Select(ur => new SelectListItem
                {
                    Value = ur.RealmId.ToString(),
                    Text = ur.RealmName
                }).ToList();

            var model = new CreaturesViewModel
            {
                UpperRealmNamed = upperRealms,
                KingdomNamed = new List<SelectListItem>() // Başlangıçta boş liste
            };

            var subspecies = _context.TblSubspecies
                .Select(ss => new SelectListItem
                {
                    Value = ss.SubspeciesId.ToString(),
                    Text = ss.SubspeciesName
                }).ToList();

            var iucn = _context.TblIucns
                .Select(i => new SelectListItem
                {
                    Value = i.IucnId.ToString(),
                    Text = i.IucnCode
                }).ToList();

            var authors = _context.TblSpeciesauthors
                .Select(a => new SelectListItem
                {
                    Value = a.AuthorId.ToString(),
                    Text = a.AuthorName
                }).ToList();

            model.SubspeciesNamed = subspecies;
            model.IucnCoded = iucn;
            model.AuthorNamed = authors;          
            
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetKingdomsByUpperRealm(int upperRealmId)
        {
            var kingdoms = await _context.TblKingdoms
                .Where(k => k.RealmId == upperRealmId)
                .Select(k => new SelectListItem
                {
                    Value = k.KingdomId.ToString(),
                    Text = k.KingdomName
                }).ToListAsync();

            return Json(kingdoms);
        }
        public async Task<IActionResult> GetPhylumsByKingdom(int KingdomID)
        {
            var phylums = await _context.TblPhylums
                .Where(k => k.KingdomId == KingdomID)
                .Select(k => new SelectListItem
                {
                    Value = k.PhylumId.ToString(),
                    Text = k.PhylumName
                }).ToListAsync();

            return Json(phylums);
        }
        public async Task<IActionResult> GetClassByPhylum(int PhylumID)
        {
            var classes = await _context.TblClasses
                .Where(k => k.PhylumId == PhylumID)
                .Select(k => new SelectListItem
                {
                    Value = k.ClassId.ToString(),
                    Text = k.ClassName
                }).ToListAsync();

            return Json(classes);
        }
        public async Task<IActionResult> GetOrderByClass(int ClassID)
        {
            var order = await _context.TblOrders
                .Where(k => k.ClassId == ClassID)
                .Select(k => new SelectListItem
                {
                    Value = k.OrderId.ToString(),
                    Text = k.OrderName
                }).ToListAsync();

            return Json(order);
        }
        public async Task<IActionResult> GetFamilyByOrder(int OrderID)
        {
            var family = await _context.TblFamilies
                .Where(k => k.OrderId == OrderID)
                .Select(k => new SelectListItem
                {
                    Value = k.FamilyId.ToString(),
                    Text = k.FamilyName
                }).ToListAsync();

            return Json(family);
        }
        public async Task<IActionResult> GetGenusByFamily(int FamilyID)
        {
            var genus = await _context.TblGenus
                .Where(k => k.FamilyId == FamilyID)
                .Select(k => new SelectListItem
                {
                    Value = k.GenusId.ToString(),
                    Text = k.GenusName
                }).ToListAsync();
            return Json(genus);
        }
        public async Task<IActionResult> GetSpeciesByGenus(int GenusID)
        {
            var species = await _context.TblSpecies
                .Where(k => k.GenusId == GenusID)
                .Select(k => new SelectListItem
                {
                    Value = k.SpeciesId.ToString(),
                    Text = k.SpeciesName
                }).ToListAsync();
            return Json(species);
        }

        [HttpPost]
        public async Task<IActionResult> AddCreature(CreaturesViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                // Eğer Phylum ekleniyorsa
                if (!string.IsNullOrEmpty(model.PhylumName2) && !string.IsNullOrEmpty(model.PhylumScientificName))
                {
                    var newPhylum = new TblPhylum
                    {
                        KingdomId = model.KingdomId,
                        PhylumName = model.PhylumName2,
                        ScientificName = model.PhylumScientificName
                    };

                    _context.TblPhylums.Add(newPhylum);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Phylum added successfully.";
                }

                // Eğer Class ekleniyorsa
                if (!string.IsNullOrEmpty(model.ClassName2) && !string.IsNullOrEmpty(model.ClassScientificName))
                {
                    var newClass = new TblClass
                    {
                        PhylumId = model.PhylumId,
                        ClassName = model.ClassName2,
                        ScientificName = model.ClassScientificName
                    };
                    _context.TblClasses.Add(newClass);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Class added successfully.";
                }
                // Eğer Order ekleniyorsa
                if (!string.IsNullOrEmpty(model.OrderName2) && !string.IsNullOrEmpty(model.OrderScientificName))
                {
                    var newClass = new TblOrder
                    {
                        ClassId = model.ClassId,
                        OrderName = model.OrderName2,
                        ScientificName = model.OrderScientificName
                    };
                    _context.TblOrders.Add(newClass);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Order added successfully.";
                }
                // Eğer Family ekleniyorsa
                if (!string.IsNullOrEmpty(model.FamilyName2) && !string.IsNullOrEmpty(model.FamilyScientificName))
                {
                    var newFamily = new TblFamily
                    {
                        OrderId = model.OrderId,
                        FamilyName = model.FamilyName2,
                        ScientificName = model.FamilyScientificName
                    };
                    _context.TblFamilies.Add(newFamily);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Family added successfully.";
                }
                // Eğer Genus ekleniyorsa
                if (!string.IsNullOrEmpty(model.GenusName2) && !string.IsNullOrEmpty(model.GenusScientificName))
                {
                    var newGenus = new TblGenu
                    {
                        FamilyId = model.FamilyId,
                        GenusName = model.GenusName2,
                        ScientificName = model.GenusScientificName
                    };
                    _context.TblGenus.Add(newGenus);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Genus added successfully.";
                }
                // Eğer Species ekleniyorsa
                if (!string.IsNullOrEmpty(model.SpeciesName2) && !string.IsNullOrEmpty(model.SpeciesScientificName))
                {
                    var newSpecies = new TblSpecy
                    {
                        GenusId = model.GenusId,
                        SpeciesName = model.SpeciesName2,
                        ScientificName = model.SpeciesScientificName
                    };
                    _context.TblSpecies.Add(newSpecies);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Species added successfully.";
                }
                // Eğer Subspecies ekleniyorsa
                if (!string.IsNullOrEmpty(model.SubspeciesName2) && !string.IsNullOrEmpty(model.SubspeciesScientificName))
                {
                    var newSubspecies = new TblSubspecy
                    {
                        SubspeciesName = model.SubspeciesName2,
                        ScientificName = model.SubspeciesScientificName
                    };
                    _context.TblSubspecies.Add(newSubspecies);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Subspecies added successfully.";
                }
                // Eğer Author ekleniyorsa
                if (!string.IsNullOrEmpty(model.AuthorName2))
                {
                    var newAuthor = new TblSpeciesauthor
                    {
                        AuthorName = model.AuthorName2
                    };
                    _context.TblSpeciesauthors.Add(newAuthor);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Author added successfully.";
                }


                return RedirectToAction("AddCreature");
            }

            TempData["ErrorMessage"] = "There was an error saving the data.";
            return RedirectToAction("AddCreature");
        }

    }
}
