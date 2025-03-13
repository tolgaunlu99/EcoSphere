using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblClass
{
    public int ClassId { get; set; }

    public int? PhylumId { get; set; }

    public string? ClassName { get; set; }

    public string? ScientificName { get; set; }

    public virtual TblPhylum? Phylum { get; set; }

    public virtual ICollection<TblCreature> TblCreatures { get; set; } = new List<TblCreature>();

    public virtual ICollection<TblOrder> TblOrders { get; set; } = new List<TblOrder>();
}
