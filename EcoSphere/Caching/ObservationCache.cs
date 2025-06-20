using EcoSphere.Models;

namespace EcoSphere.Caching
{
    public static class ObservationCache
    {
        private static List<ObservationViewModel> _cachedObservations = new();
        private static bool _isLoaded = false;
        private static readonly object _lock = new();

        public static void LoadCache(MyDbContext context)
        {
            lock (_lock)
            {
                _cachedObservations = LoadFromDatabase(context);
                _isLoaded = true;
            }
        }

        public static List<ObservationViewModel> GetCachedObservations()
        {
            lock (_lock)
            {
                return _cachedObservations;
            }
        }

        public static void AddObservation(ObservationViewModel model)
        {
            lock (_lock)
            {
                _cachedObservations.Add(model);
            }
        }

        public static void RemoveObservation(int id)
        {
            lock (_lock)
            {
                _cachedObservations.RemoveAll(x => x.Id == id);
            }
        }

        public static void ReloadCache(MyDbContext context)
        {
            lock (_lock)
            {
                _cachedObservations = LoadFromDatabase(context);
            }
        }

        private static List<ObservationViewModel> LoadFromDatabase(MyDbContext context)
        {
            return context.VwObservations
                .Where(v => v.status == 1) // 👈 Sadece onaylı gözlemler
                .Select(v => new ObservationViewModel
                {
                    Id = v.Id,
                    CreatureName = v.ScientificName,
                    UserName = v.Username,
                    provincename = v.ProvinceName,
                    DistrictName = v.DistrictName,
                    EndemicStatName = v.EndemicStatus,
                    ProjectName = v.ProjectName,
                    ReferenceName = v.ReferenceName,
                    LocationType = v.LocationType,
                    GenderName = v.GenderName,
                    Lat = v.Lat,
                    Long = v.Long,
                    Activity = v.Activity,
                    SeenTime = v.SeenTime,
                    CreationDate = v.CreationDate
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
