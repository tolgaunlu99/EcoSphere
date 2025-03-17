using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblUpperrealm
{
    public int RealmId { get; set; }

    public string? RealmName { get; set; }

    public string? ScientificName { get; set; }

    public virtual ICollection<TblCreature> TblCreatures { get; set; } = new List<TblCreature>();


    public virtual ICollection<TblKingdom> TblKingdoms { get; set; } = new List<TblKingdom>();
}
