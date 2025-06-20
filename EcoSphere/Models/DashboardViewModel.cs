using System.Diagnostics;

namespace EcoSphere.Models
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalCreatures { get; set; }
        public int TotalObservations { get; set; }
        public int TotalPlants { get; set; }
        public int TotalAnimals { get; set; }
        public int TodayLogins { get; set; }
        public int TotalLogs { get; set; }

        public List<RecentLog>? RecentLogs { get; set; }
        public List<string>? RoleNames { get; set; }
        public List<int>? RoleCounts { get; set; }
        public List<string>? Last7Days { get; set; }
        public List<int>? DailyLogins { get; set; }
        // Server Health
        public double CpuUsage { get; set; }
        public double RamUsage { get; set; }
        public Dictionary<string, string>? RoleDescriptions { get; set; } // Rol açıklamaları
    }
    public class RecentLog
    {
        public string? Username { get; set; }
        public string? Action { get; set; }
        public DateTime ActionTime { get; set; }
    }
}
