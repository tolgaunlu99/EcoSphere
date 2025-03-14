using System.ComponentModel;
namespace EcoSphere.Models
{
    public class ObservationViewModel
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("Creature ID")]
        public int? CreatureId { get; set; }

        [DisplayName("User ID")]
        public int? UserId { get; set; }

        [DisplayName("Region ID")]
        public int? RegionId { get; set; }

        [DisplayName("Province ID")]
        public int? CityId { get; set; }

        [DisplayName("District ID")]
        public int? DistrictId { get; set; }

        [DisplayName("Locality ID")]
        public int? LocalityId { get; set; }

        [DisplayName("Neighborhood ID")]
        public int? NeighborhoodId { get; set; }

        [DisplayName("Migration Status ID")]
        public int? MigrationStatusId { get; set; }

        [DisplayName("Endemic Status ID")]
        public int? EndemicStatusId { get; set; }

        [DisplayName("Project ID")]
        public int? ProjectId { get; set; }

        [DisplayName("Citation ID")]
        public int? CitationId { get; set; }

        [DisplayName("Reference ID")]
        public int? ReferenceId { get; set; }

        [DisplayName("Location Type ID")]
        public int? LocationTypeId { get; set; }

        [DisplayName("Location Range ID")]
        public int? LocationRangeId { get; set; }

        [DisplayName("Gender ID")]
        public int? GenderId { get; set; }

        [DisplayName("Long")]
        public string? Long { get; set; }

        [DisplayName("Lat")]
        public string? Lat { get; set; }

        [DisplayName("Activiy")]
        public string? Activity { get; set; }

        [DisplayName("Seen Time")]
        public DateTime? SeenTime { get; set; }

        [DisplayName("Creation Date")]
        public DateTime? CreationDate { get; set; }

        [DisplayName("Creature Name")]
        public string? CreatureName { get; set; }

        [DisplayName("User Name")]
        public string? UserName { get; set; }

        [DisplayName("User Surname")]
        public string? UsersurName { get; set; }

        [DisplayName("Region Name")]
        public string? RegionName { get; set; }

        [DisplayName("Province Name")]
        public string? provincename { get; set; }

        [DisplayName("District Name")]
        public string? DistrictName { get; set; }

        [DisplayName("Locality Name")]
        public string? LocalityName { get; set; }

        [DisplayName("Neighborhood Name")]
        public string? HoodName { get; set; }

        [DisplayName("Migration Status Name")]
        public string? MigrationStatName { get; set; }

        [DisplayName("Endemic Status Name")]
        public string? EndemicStatName { get; set; }

        [DisplayName("Project Name")]
        public string? ProjectName { get; set; }

        [DisplayName("Citation Name")]
        public string? CitationName { get; set; }

        [DisplayName("Reference Name")]
        public string? ReferenceName { get; set; }

        [DisplayName("Location Type")]
        public string? LocationType { get; set; }

        [DisplayName("Location Range")]
        public string? LocationRange { get; set; }

        [DisplayName("Gender Name")]
        public string? GenderName { get; set; }

        [DisplayName("Full Name")]
        public string? FullName => $"{UserName} {UsersurName}";


    }
}
