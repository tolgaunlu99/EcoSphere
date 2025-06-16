using EcoSphere.Models;

namespace EcoSphere.Caching
{
    public static class CreaturesCache
    {
        private static List<CreaturesViewModel> _cachedCreatures = new();
        private static bool _isLoaded = false;
        private static readonly object _lock = new();

        public static void LoadCache(MyDbContext context)
        {
            lock (_lock)
            {
                _cachedCreatures = LoadFromDatabase(context);
                _isLoaded = true;
            }
        }

        public static List<CreaturesViewModel> GetCachedCreatures()
        {
            lock (_lock)
            {
                return _cachedCreatures;
            }
        }

        public static void AddCreature(CreaturesViewModel model)
        {
            lock (_lock)
            {
                _cachedCreatures.Add(model);
            }
        }

        public static void RemoveCreature(int creatureId)
        {
            lock (_lock)
            {
                _cachedCreatures.RemoveAll(x => x.CreatureId == creatureId);
            }
        }

        public static void ReloadCache(MyDbContext context)
        {
            lock (_lock)
            {
                _cachedCreatures = LoadFromDatabase(context);
            }
        }

        private static List<CreaturesViewModel> LoadFromDatabase(MyDbContext context)
        {
            return context.VwSpecies
                .Select(v => new CreaturesViewModel
                {
                    CreatureId = v.CreatureId,
                    UpperRealmName = v.RealmName,
                    KingdomName = v.KingdomName,
                    PhylumName = v.PhylumName,
                    ClassName = v.ClassName,
                    OrderName = v.OrderName,
                    FamilyName = v.FamilyName,
                    GenusName = v.GenusName,
                    SpeciesName = v.SpeciesName,
                    CommonName = v.CommonName,
                    IucnCode = v.IucnCode
                })
                .ToList();
        }

        public static bool IsCacheEmpty()
        {
            lock (_lock)
            {
                return !_isLoaded;
            }
        }
    }
}
