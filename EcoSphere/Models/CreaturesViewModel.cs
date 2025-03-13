using System.ComponentModel;

namespace EcoSphere.Models
{
    public class CreaturesViewModel
    {
        public int CreatureId { get; set; }
        [DisplayName("Upper Realm ID")]
        public int? UpperRealmId { get; set; }
        [DisplayName("Kingdom ID")]
        public int? KingdomId { get; set; }
        [DisplayName("Phylum ID")]
        public int? PhylumId { get; set; }
        [DisplayName("Class ID")]
        public int? ClassId { get; set; }
        [DisplayName("Order ID")]
        public int? OrderId { get; set; }
        [DisplayName("Family ID")]
        public int? FamilyId { get; set; }
        [DisplayName("Genus ID")]
        public int? GenusId { get; set; }
        [DisplayName("Species ID")]
        public int? SpeciesId { get; set; }
        [DisplayName("Subspecies ID")]
        public int? SubspeciesId { get; set; }
        [DisplayName("IUCN ID")]
        public int? IucnId { get; set; }
        [DisplayName("Author ID")]
        public int? AuthorId { get; set; }
        [DisplayName("ScientificName")]
        public string? ScientificName { get; set; }
        [DisplayName("CommonName")]
        public string? CommonName { get; set; }
        [DisplayName("UpperRealmName")]
        public string? UpperRealmName { get; set; }

        [DisplayName("KingdomName")]
        public string? KingdomName { get; set; }

        [DisplayName("PhylumName")]
        public string? PhylumName { get; set; }

        [DisplayName("ClassName")]
        public string? ClassName { get; set; }

        [DisplayName("OrderName")]
        public string? OrderName { get; set; }

        [DisplayName("FamilyName")]
        public string? FamilyName { get; set; }

        [DisplayName("GenusName")]
        public string? GenusName { get; set; }

        [DisplayName("SpeciesName")]
        public string? SpeciesName { get; set; }

        [DisplayName("SubspeciesName")]
        public string? SubspeciesName { get; set; }

        [DisplayName("IucnName")]
        public string? IucnName { get; set; }

        [DisplayName("AuthorName")]
        public string? AuthorName { get; set; }
    }
}
