using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblSpecy
{
    public int SpeciesId { get; set; }

    public int? GenusId { get; set; }

    public string? SpeciesName { get; set; }

    public string? ScientificName { get; set; }

    public virtual TblGenu? Genus { get; set; }

    public virtual ICollection<TblCreature> TblCreatures { get; set; } = new List<TblCreature>();
}
