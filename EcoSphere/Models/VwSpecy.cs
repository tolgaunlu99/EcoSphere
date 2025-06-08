using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSphere.Models;

public partial class VwSpecy
{
    [Column("Creature_ID")]
    public int CreatureId { get; set; }

    [Column("realm_name")]
    public string? RealmName { get; set; }

    [Column("kingdom_name")]
    public string? KingdomName { get; set; }

    [Column("phylum_name")]
    public string? PhylumName { get; set; }

    [Column("class_name")]
    public string? ClassName { get; set; }

    [Column("order_name")]
    public string? OrderName { get; set; }

    [Column("family_name")]
    public string? FamilyName { get; set; }

    [Column("genus_name")]
    public string? GenusName { get; set; }

    [Column("species_name")]
    public string? SpeciesName { get; set; }

    [Column("IUCN_code")]
    public string? IucnCode { get; set; }

    [Column("scientific_name")]
    public string? ScientificName { get; set; }

    [Column("common_name")]
    public string? CommonName { get; set; }
}
