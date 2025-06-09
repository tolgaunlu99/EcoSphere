using EcoSphere.Models;

namespace EcoSphere.Caching
{
    public static class CreaturesCache
    {
        private static List<CreaturesViewModel> _cachedCreatures = new();
        private static bool _isLoaded = false;
        private static readonly object _lock = new();

        // Cache yükleme
        public static void LoadCache(MyDbContext context)
        {
            lock (_lock)
            {
                if (!_isLoaded)
                {
                    _cachedCreatures = context.VwSpecies
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

                    _isLoaded = true;
                }
            }
        }

        // Cache getirme
        public static List<CreaturesViewModel> GetCachedCreatures()
        {
            lock (_lock)
            {
                return _cachedCreatures;
            }
        }

        // Cache'e yeni kayıt ekleme
        public static void AddCreature(CreaturesViewModel model)
        {
            lock (_lock)
            {
                _cachedCreatures.Add(model);
            }
        }

        // Cache’in yüklü olup olmadığını kontrol et
        public static bool IsCacheEmpty()
        {
            lock (_lock)
            {
                return !_isLoaded;
            }
        }
    }
}
