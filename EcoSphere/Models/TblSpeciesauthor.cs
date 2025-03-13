using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblSpeciesauthor
{
    public int AuthorId { get; set; }

    public string? AuthorName { get; set; }

    public virtual ICollection<TblCreature> TblCreatures { get; set; } = new List<TblCreature>();
}
