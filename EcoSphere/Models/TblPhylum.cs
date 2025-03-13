using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblPhylum
{
    public int PhylumId { get; set; }

    public int? KingdomId { get; set; }

    public string? PhylumName { get; set; }

    public string? ScientificName { get; set; }

    public virtual TblKingdom? Kingdom { get; set; }

    public virtual ICollection<TblClass> TblClasses { get; set; } = new List<TblClass>();

    public virtual ICollection<TblCreature> TblCreatures { get; set; } = new List<TblCreature>();
}
