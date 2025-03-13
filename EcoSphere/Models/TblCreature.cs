using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblCreature
{
    public int CreatureId { get; set; }

    public int? UpperRealmId { get; set; }

    public int? KingdomId { get; set; }

    public int? PhylumId { get; set; }

    public int? ClassId { get; set; }

    public int? OrderId { get; set; }

    public int? FamilyId { get; set; }

    public int? GenusId { get; set; }

    public int? SpeciesId { get; set; }

    public int? SubspeciesId { get; set; }

    public int? IucnId { get; set; }

    public int? AuthorId { get; set; }

    public string? ScientificName { get; set; }

    public string? CommonName { get; set; }

    public virtual TblSpeciesauthor? Author { get; set; }

    public virtual TblClass? Class { get; set; }

    public virtual TblFamily? Family { get; set; }

    public virtual TblGenu? Genus { get; set; }

    public virtual TblIucn? Iucn { get; set; }

    public virtual TblKingdom? Kingdom { get; set; }

    public virtual TblOrder? Order { get; set; }

    public virtual TblPhylum? Phylum { get; set; }

    public virtual TblSpecy? Species { get; set; }

    public virtual TblSubspecy? Subspecies { get; set; }

    public virtual ICollection<TblMaintable> TblMaintables { get; set; } = new List<TblMaintable>();

    public virtual TblUpperrealm? UpperRealm { get; set; }
}
