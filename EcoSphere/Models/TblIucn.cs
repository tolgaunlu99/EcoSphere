using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblIucn
{
    public int IucnId { get; set; }

    public string? IucnCode { get; set; }

    public string? IucnName { get; set; }

    public virtual ICollection<TblCreature> TblCreatures { get; set; } = new List<TblCreature>();
}
