using EcoSphere.Caching;
using EcoSphere.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//merhaba togla
namespace EcoSphere.Controllers
{
    public class CreaturesViewController : Controller
    {
        private readonly MyDbContext _context;

        public CreaturesViewController(MyDbContext context)
        {
            _context = context;
            if (CreaturesCache.IsCacheEmpty())
            {
                CreaturesCache.LoadCache(_context);
            }
        }
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
        //public async Task<IActionResult> GetCreatures()
        //{
        //    try
        //    {
        //        var data = await _context.VwSpecies
        //            .Select(v => new CreaturesViewModel
        //            {
        //                CreatureId = v.CreatureId,
        //                UpperRealmName = v.RealmName,
        //                KingdomName = v.KingdomName,
        //                PhylumName = v.PhylumName,
        //                ClassName = v.ClassName,
        //                OrderName = v.OrderName,
        //                FamilyName = v.FamilyName,
        //                GenusName = v.GenusName,
        //                SpeciesName = v.SpeciesName,
        //                CommonName = v.CommonName,
        //                IucnCode = v.IucnCode
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
        public IActionResult GetCreatures()
        {
            try
            {
                var data = CreaturesCache.GetCachedCreatures();
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
                var userRole = _context.TblUserRoles.FirstOrDefault(ur => ur.UserId == userId.Value);
                return userRole?.RoleId ?? 0;
            }
            return 0;
        }

        [HttpGet]
        public IActionResult AddCreature()
        {
            int? UserroleId = HttpContext.Session.GetInt32("Role_ID");

            if (UserroleId == null || (UserroleId != 1 && UserroleId != 2))
            {
                return RedirectToAction("AccessDenied", "UserView");
            }

            var roleID = GetCurrentUserRoleId();
            ViewBag.UserRoleId = roleID;

            var upperRealms = _context.TblUpperrealms
                .Select(ur => new SelectListItem
                {
                    Value = ur.RealmId.ToString(),
                    Text = ur.RealmName
                }).ToList();

            var model = new CreaturesViewModel
            {
                UpperRealmNamed = upperRealms,
                KingdomNamed = new List<SelectListItem>()
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

        // --- Kalan GetXByY metotları (değişmedi, aynen kullanılabilir) ---

        public async Task<IActionResult> GetKingdomsByUpperRealm(int upperRealmId) =>
            Json(await _context.TblKingdoms.Where(k => k.RealmId == upperRealmId)
                .Select(k => new SelectListItem { Value = k.KingdomId.ToString(), Text = k.KingdomName })
                .ToListAsync());

        public async Task<IActionResult> GetPhylumsByKingdom(int kingdomId) =>
            Json(await _context.TblPhylums.Where(k => k.KingdomId == kingdomId)
                .Select(k => new SelectListItem { Value = k.PhylumId.ToString(), Text = k.PhylumName })
                .ToListAsync());

        public async Task<IActionResult> GetClassByPhylum(int phylumId) =>
            Json(await _context.TblClasses.Where(k => k.PhylumId == phylumId)
                .Select(k => new SelectListItem { Value = k.ClassId.ToString(), Text = k.ClassName })
                .ToListAsync());

        public async Task<IActionResult> GetOrderByClass(int classId) =>
            Json(await _context.TblOrders.Where(k => k.ClassId == classId)
                .Select(k => new SelectListItem { Value = k.OrderId.ToString(), Text = k.OrderName })
                .ToListAsync());

        public async Task<IActionResult> GetFamilyByOrder(int orderId) =>
            Json(await _context.TblFamilies.Where(k => k.OrderId == orderId)
                .Select(k => new SelectListItem { Value = k.FamilyId.ToString(), Text = k.FamilyName })
                .ToListAsync());

        public async Task<IActionResult> GetGenusByFamily(int familyId) =>
            Json(await _context.TblGenus.Where(k => k.FamilyId == familyId)
                .Select(k => new SelectListItem { Value = k.GenusId.ToString(), Text = k.GenusName })
                .ToListAsync());

        public async Task<IActionResult> GetSpeciesByGenus(int genusId) =>
            Json(await _context.TblSpecies.Where(k => k.GenusId == genusId)
                .Select(k => new SelectListItem { Value = k.SpeciesId.ToString(), Text = k.SpeciesName })
                .ToListAsync());

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
                    return Json(new
                    {
                        success = true,
                        message = "Phylum added successfully.",
                        phylumId = newPhylum.PhylumId,
                        phylumName = newPhylum.PhylumName
                    });
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
                    return Json(new
                    {
                        success = true,
                        message = "Class added successfully.",
                        classId = newClass.ClassId,
                        className = newClass.ClassName
                    });
                }
                // Eğer Order ekleniyorsa
                if (!string.IsNullOrEmpty(model.OrderName2) && !string.IsNullOrEmpty(model.OrderScientificName))
                {
                    var newOrder = new TblOrder
                    {
                        ClassId = model.ClassId,
                        OrderName = model.OrderName2,
                        ScientificName = model.OrderScientificName
                    };
                    _context.TblOrders.Add(newOrder);
                    await _context.SaveChangesAsync();
                    return Json(new
                    {
                        success = true,
                        message = "Order added successfully.",
                        orderId = newOrder.OrderId,
                        orderName = newOrder.OrderName
                    });
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
                    return Json(new
                    {
                        success = true,
                        message = "Family added successfully.",
                        familyId = newFamily.FamilyId,
                        familyName = newFamily.FamilyName
                    });
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
                    return Json(new
                    {
                        success = true,
                        message = "Genus added successfully.",
                        genusId = newGenus.GenusId,
                        genusName = newGenus.GenusName
                    });
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
                    return Json(new
                    {
                        success = true,
                        message = "Species added successfully.",
                        speciesId = newSpecies.SpeciesId,
                        speciesName = newSpecies.SpeciesName
                    });
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
                    return Json(new
                    {
                        success = true,
                        message = "Subspecies added successfully.",
                        subspeciesId = newSubspecies.SubspeciesId,
                        subspeciesName = newSubspecies.SubspeciesName
                    });
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
                    return Json(new
                    {
                        success = true,
                        message = "Author added successfully.",
                        authorId = newAuthor.AuthorId,
                        authorName = newAuthor.AuthorName
                    });
                }



                return RedirectToAction("AddCreature");
            }

            TempData["ErrorMessage"] = "There was an error saving the data.";
            return RedirectToAction("AddCreature");
        }
        //public async Task<IActionResult> SubmitCreature(CreaturesViewModel model)
        //{
        //      var NewCreature = new TblCreature
        //        {
        //            UpperRealmId = model.UpperRealmId,
        //            KingdomId = model.KingdomId,
        //            PhylumId = model.PhylumId,
        //            ClassId = model.ClassId,
        //            OrderId = model.OrderId,
        //            FamilyId = model.FamilyId,
        //            GenusId = model.GenusId,
        //            SpeciesId = model.SpeciesId,
        //            SubspeciesId = model.SubspeciesId,
        //            IucnId = model.IucnId,
        //            AuthorId = model.AuthorId,
        //            CommonName = model.CommonName,
        //            ScientificName = model.ScientificName

        //        };
        //        _context.TblCreatures.Add(NewCreature);
        //        await _context.SaveChangesAsync();
        //        TempData["SuccessMessage"] = "Creature added successfully.";
        //        return RedirectToAction("AddCreature");


        //}
        [HttpPost]
        public async Task<IActionResult> SubmitCreature(CreaturesViewModel model)
        {
            var NewCreature = new TblCreature
            {
                UpperRealmId = model.UpperRealmId,
                KingdomId = model.KingdomId,
                PhylumId = model.PhylumId,
                ClassId = model.ClassId,
                OrderId = model.OrderId,
                FamilyId = model.FamilyId,
                GenusId = model.GenusId,
                SpeciesId = model.SpeciesId,
                SubspeciesId = model.SubspeciesId,
                IucnId = model.IucnId,
                AuthorId = model.AuthorId,
                CommonName = model.CommonName,
                ScientificName = model.ScientificName
            };

            _context.TblCreatures.Add(NewCreature);
            await _context.SaveChangesAsync();

            // Cache'e yeni veriyi ekle
            CreaturesCache.AddCreature(new CreaturesViewModel
            {
                CreatureId = NewCreature.CreatureId,
                UpperRealmName = "", // Detay istersen ekleyebilirsin
                KingdomName = "",
                PhylumName = "",
                ClassName = "",
                OrderName = "",
                FamilyName = "",
                GenusName = "",
                SpeciesName = model.ScientificName,
                CommonName = model.CommonName,
                IucnCode = ""
            });

            TempData["SuccessMessage"] = "Creature added successfully.";
            return RedirectToAction("AddCreature");
        }


    }
}
