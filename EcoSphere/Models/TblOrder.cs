using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblOrder
{
    public int OrderId { get; set; }

    public int? ClassId { get; set; }

    public string? OrderName { get; set; }

    public string? ScientificName { get; set; }

    public virtual TblClass? Class { get; set; }

    public virtual ICollection<TblCreature> TblCreatures { get; set; } = new List<TblCreature>();

    public virtual ICollection<TblFamily> TblFamilies { get; set; } = new List<TblFamily>();
}
