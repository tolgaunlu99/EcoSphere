using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblSubspecy
{
    public int SubspeciesId { get; set; }

    public string? SubspeciesName { get; set; }

    public string? ScientificName { get; set; }

    public virtual ICollection<TblCreature> TblCreatures { get; set; } = new List<TblCreature>();
}
