using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblKingdom
{
    public int KingdomId { get; set; }

    public int? RealmId { get; set; }

    public string? KingdomName { get; set; }

    public string? ScientificName { get; set; }

    public virtual TblUpperrealm? Realm { get; set; }

    public virtual ICollection<TblCreature> TblCreatures { get; set; } = new List<TblCreature>();

    public virtual ICollection<TblPhylum> TblPhylums { get; set; } = new List<TblPhylum>();
}
