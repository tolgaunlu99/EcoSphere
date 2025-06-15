using EcoSphere.Models;
using System.Timers;
using Timer = System.Timers.Timer;

namespace EcoSphere.Caching
{
    public static class ObservationCache
    {
        private static List<ObservationViewModel> _cachedObservations = new();
        private static bool _isLoaded = false;
        private static readonly object _lock = new();

        private static Timer? _refreshTimer;

        public static void LoadCache(MyDbContext context)
        {
            lock (_lock)
            {
                _cachedObservations = LoadFromDatabase(context);
                _isLoaded = true;
            }

            StartAutoRefresh(context); // Periyodik yenileme başlat
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

        private static void StartAutoRefresh(MyDbContext context)
        {
            if (_refreshTimer == null)
            {
                _refreshTimer = new Timer(30 * 60 * 1000); // 30 dakika
                _refreshTimer.Elapsed += (sender, e) =>
                {
                    ReloadCache(context);
                };
                _refreshTimer.AutoReset = true;
                _refreshTimer.Enabled = true;
            }
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