using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblGenu
{
    public int GenusId { get; set; }

    public int? FamilyId { get; set; }

    public string? GenusName { get; set; }

    public string? ScientificName { get; set; }

    public virtual TblFamily? Family { get; set; }

    public virtual ICollection<TblCreature> TblCreatures { get; set; } = new List<TblCreature>();

    public virtual ICollection<TblSpecy> TblSpecies { get; set; } = new List<TblSpecy>();
}
