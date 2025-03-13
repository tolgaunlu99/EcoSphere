using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblReference
{
    public int ReferenceId { get; set; }

    public string? ReferenceName { get; set; }

    public virtual ICollection<TblMaintable> TblMaintables { get; set; } = new List<TblMaintable>();
}
