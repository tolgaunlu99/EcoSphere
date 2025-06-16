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
                        message = "Şube başarıyla eklendi.",
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
                        message = "Sınıf başarıyla eklendi.",
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
                        message = "Takım başarıyla eklendi.",
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
                        message = "Familya başarıyla eklendi.",
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
                        message = "Cins başarıyla eklendi.",
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
                        message = "Tür başarıyla eklendi.",
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
                        message = "Alttür başarıyla eklendi.",
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
                        message = "Yazar başarıyla eklendi.",
                        authorId = newAuthor.AuthorId,
                        authorName = newAuthor.AuthorName
                    });
                }



                return RedirectToAction("AddCreature");
            }

            TempData["ErrorMessage"] = "Veriler kaydedilirken bir hata oluştu.";
            return RedirectToAction("AddCreature");
        }
        [HttpPost]
        public IActionResult DeleteSpecies(int creatureId)
        {
            var creature = _context.TblCreatures.FirstOrDefault(c => c.CreatureId == creatureId);
            if (creature == null)
                return Json(new { success = false, message = "Kayıt bulunamadı." });

            // 1. Bu creature'a ait gözlemler varsa önce onları sil
            var relatedObservations = _context.TblMaintables
                .Where(o => o.CreatureId == creatureId)
                .ToList();

            if (relatedObservations.Any())
            {
                // ObservationCache'ten de sil
                foreach (var obs in relatedObservations)
                {
                    ObservationCache.RemoveObservation(obs.Id);
                }

                _context.TblMaintables.RemoveRange(relatedObservations);
            }

            // 2. Creature'ı sil
            _context.TblCreatures.Remove(creature);

            // 3. CreaturesCache'ten sil
            CreaturesCache.RemoveCreature(creatureId);

            // 4. Loglama işlemi
            var userId = HttpContext.Session.GetInt32("UserID");
            var username = _context.TblUsers.FirstOrDefault(u => u.UserId == userId)?.Username ?? "Sistem";

            var action = new TblUseraction
            {
                UserId = userId,
                Action = $"{username} adlı kullanıcı {creatureId} ID'li türü ve ilişkili gözlemleri sildi!",
                ActionTime = DateTime.Now
            };

            _context.TblUseractions.Add(action);

            // 5. Değişiklikleri kaydet
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult UpdateSpecies(int creatureId, string commonName, int? iucnId)
        {
            var existing = _context.TblCreatures.FirstOrDefault(c => c.CreatureId == creatureId);
            if (existing == null)
                return NotFound();

            existing.CommonName = commonName;
            existing.IucnId = iucnId;
            _context.SaveChanges();

            var cached = CreaturesCache.GetCachedCreatures().FirstOrDefault(c => c.CreatureId == creatureId);
            if (cached != null)
            {
                cached.CommonName = commonName;
                cached.IucnCode = _context.TblIucns.FirstOrDefault(i => i.IucnId == iucnId)?.IucnCode ?? "";
            }
            var userId = HttpContext.Session.GetInt32("UserID");
            var username = _context.TblUsers.FirstOrDefault(u => u.UserId == userId)?.Username ?? "Sistem";

            var action = new TblUseraction
            {
                UserId = userId,
                Action = $"{username} adlı kullanıcı {creatureId} ID'li türü Güncelledi!",
                ActionTime = DateTime.Now
            };

            _context.TblUseractions.Add(action);

            // 5. Değişiklikleri kaydet
            _context.SaveChanges();
            return Json(new { success = true });
        }
        [HttpGet]
        public JsonResult GetIucnList()
        {
            var list = _context.TblIucns
                .Select(i => new { i.IucnId, i.IucnCode })
                .ToList();
            return Json(list);
        }

        [HttpGet]
        public JsonResult GetCreatureById(int id)
        {
            var creature = _context.TblCreatures
                .Where(c => c.CreatureId == id)
                .Select(c => new
                {
                    c.CreatureId,
                    c.CommonName,
                    c.IucnId
                })
                .FirstOrDefault();

            return Json(creature);
        }
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

            // Cache'e yeni kayıt ekle
            var cachedCreature = new CreaturesViewModel
            {
                CreatureId = NewCreature.CreatureId,
                UpperRealmName = _context.TblUpperrealms.FirstOrDefault(r => r.RealmId == NewCreature.UpperRealmId)?.RealmName ?? "",
                KingdomName = _context.TblKingdoms.FirstOrDefault(k => k.KingdomId == NewCreature.KingdomId)?.KingdomName ?? "",
                PhylumName = _context.TblPhylums.FirstOrDefault(p => p.PhylumId == NewCreature.PhylumId)?.PhylumName ?? "",
                ClassName = _context.TblClasses.FirstOrDefault(c => c.ClassId == NewCreature.ClassId)?.ClassName ?? "",
                OrderName = _context.TblOrders.FirstOrDefault(o => o.OrderId == NewCreature.OrderId)?.OrderName ?? "",
                FamilyName = _context.TblFamilies.FirstOrDefault(f => f.FamilyId == NewCreature.FamilyId)?.FamilyName ?? "",
                GenusName = _context.TblGenus.FirstOrDefault(g => g.GenusId == NewCreature.GenusId)?.GenusName ?? "",
                SpeciesName = model.ScientificName,
                CommonName = model.CommonName,
                IucnCode = _context.TblIucns.FirstOrDefault(i => i.IucnId == NewCreature.IucnId)?.IucnCode ?? ""
            };

            CreaturesCache.AddCreature(cachedCreature);

            TempData["SuccessMessage"] = "Canlı başarıyla eklendi.";
            return RedirectToAction("AddCreature");
        }


    }
}
