
﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace EcoSphere.Models
{
    public class ObservationViewModel
    {
        [BindNever]
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

        [DisplayName("Image File")]
        public IFormFile? ImageFile { get; set; }

        [DisplayName("Image Path")]
        public string? ImagePath { get; set; }

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
        [DisplayName("Project Name2")]
        public string? ProjectName2 { get; set; }

        [DisplayName("Citation Name")]
        public string? CitationName { get; set; }
        [DisplayName("Citation Name2")]
        public string? CitationName2 { get; set; }

        [DisplayName("Reference Name")]
        public string? ReferenceName { get; set; }
        [DisplayName("Reference Name2")]
        public string? ReferenceName2 { get; set; }

        [DisplayName("Location Type")]
        public string? LocationType { get; set; }

        [DisplayName("Location Range")]
        public string? LocationRange { get; set; }
        [DisplayName("Location Range2")]
        public string? LocationRange2 { get; set; }

        [DisplayName("Gender Name")]
        public string? GenderName { get; set; }

        [DisplayName("Full Name")]
        public string? FullName => $"{UserName} {UsersurName}";

        public int SelectedObservationId { get; set; }
        public IEnumerable<SelectListItem> AddObservations { get; set; } = new List<SelectListItem>();

        [DisplayName("Creatures")]
        public IEnumerable<SelectListItem> CreatureNamed { get; set; } = new List<SelectListItem>();

        [DisplayName("User")]
        public IEnumerable<SelectListItem> Usernamed { get; set; } = new List<SelectListItem>();

        [DisplayName("Region")]
        public IEnumerable<SelectListItem> RegionNamed { get; set; } = new List<SelectListItem>();

        [DisplayName("Province")]
        public IEnumerable<SelectListItem> ProvinceNamed { get; set; } = new List<SelectListItem>();

        [DisplayName("District")]
        public IEnumerable<SelectListItem> DistrictNamed { get; set; } = new List<SelectListItem>();

        [DisplayName("Locality")]
        public IEnumerable<SelectListItem> LocalityNamed { get; set; } = new List<SelectListItem>();

        [DisplayName("Neighborhood")]
        public IEnumerable<SelectListItem> HoodNamed { get; set; } = new List<SelectListItem>();

        [DisplayName("Migration Status")]
        public IEnumerable<SelectListItem> MigrationstatNamed { get; set; } = new List<SelectListItem>();

        [DisplayName("Endemic Status")]
        public IEnumerable<SelectListItem> EndemicstatNamed { get; set; } = new List<SelectListItem>();

        [DisplayName("Project")]
        public IEnumerable<SelectListItem> ProjectNamed { get; set; } = new List<SelectListItem>();

        [DisplayName("Citation")]
        public IEnumerable<SelectListItem> CitationNamed { get; set; } = new List<SelectListItem>();

        [DisplayName("Reference")]
        public IEnumerable<SelectListItem> ReferenceNamed { get; set; } = new List<SelectListItem>();

        [DisplayName("Location Type")]
        public IEnumerable<SelectListItem> LocationtypeNamed { get; set; } = new List<SelectListItem>();

        [DisplayName("Location Range")]
        public IEnumerable<SelectListItem> LocationRangeNamed { get; set; } = new List<SelectListItem>();

        [DisplayName("Gender")]
        public IEnumerable<SelectListItem> GenderNamed { get; set; } = new List<SelectListItem>();

        // Haritadan gelen veriler
        public string HiddenProvinceName { get; set; }
        public string HiddenDistrictName { get; set; }

        

    }
}