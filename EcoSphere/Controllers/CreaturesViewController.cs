using EcoSphere.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public ActionResult AddCreature()
        {
            // UpperRealm combobox verisi
            var upperRealms = _context.TblUpperrealms
                .Select(ur => new SelectListItem
                {
                    Value = ur.RealmId.ToString(),
                    Text = ur.RealmName
                }).ToList();

            // Kingdom combobox verisi
            var kingdoms = _context.TblKingdoms
                .Select(k => new SelectListItem
                {
                    Value = k.KingdomId.ToString(),
                    Text = k.KingdomName
                }).ToList();

            // Diğer combobox verilerini benzer şekilde alıyoruz
            var phylums = _context.TblPhylums
                .Select(p => new SelectListItem
                {
                    Value = p.PhylumId.ToString(),
                    Text = p.PhylumName
                }).ToList();

            var classes = _context.TblClasses
                .Select(c => new SelectListItem
                {
                    Value = c.ClassId.ToString(),
                    Text = c.ClassName
                }).ToList();

            var orders = _context.TblOrders
                .Select(o => new SelectListItem
                {
                    Value = o.OrderId.ToString(),
                    Text = o.OrderName
                }).ToList();

            var families = _context.TblFamilies
                .Select(f => new SelectListItem
                {
                    Value = f.FamilyId.ToString(),
                    Text = f.FamilyName
                }).ToList();

            var genus = _context.TblGenus
                .Select(g => new SelectListItem
                {
                    Value = g.GenusId.ToString(),
                    Text = g.GenusName
                }).ToList();

            var species = _context.TblSpecies
                .Select(s => new SelectListItem
                {
                    Value = s.SpeciesId.ToString(),
                    Text = s.SpeciesName
                }).ToList();

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

            // ViewModel'e tüm combobox verilerini gönderiyoruz
            var model = new CreaturesViewModel
            {
                UpperRealmNamed = upperRealms,
                KingdomNamed = kingdoms,
                PhylumNamed = phylums,
                ClassNamed = classes,
                OrderNamed = orders,
                FamilyNamed = families,
                GenusNamed = genus,
                SpeciesNamed = species,
                SubspeciesNamed = subspecies,
                IucnCoded = iucn,
                AuthorNamed = authors
            };

            return View(model);
        }
    }
}
