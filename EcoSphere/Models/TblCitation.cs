using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblCitation
{
    public int CitationId { get; set; }

    public string? CitationName { get; set; }

    public virtual ICollection<TblMaintable> TblMaintables { get; set; } = new List<TblMaintable>();
}
