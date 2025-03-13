using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblFamily
{
    public int FamilyId { get; set; }

    public int? OrderId { get; set; }

    public string? FamilyName { get; set; }

    public string? ScientificName { get; set; }

    public virtual TblOrder? Order { get; set; }

    public virtual ICollection<TblCreature> TblCreatures { get; set; } = new List<TblCreature>();

    public virtual ICollection<TblGenu> TblGenus { get; set; } = new List<TblGenu>();
}
