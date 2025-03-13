using EcoSphere.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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
                                         IucnName = i.IucnName,
                                         AuthorName = a.AuthorName,
                                         ScientificName = c.ScientificName,
                                         CommonName = c.CommonName
                                     };

            var viewModel = await creaturesWithNames.ToListAsync();
            return View(viewModel);
        }
    }
}
