using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblLocationtype
{
    public int LocationTypeId { get; set; }

    public string? LocationType { get; set; }

    public virtual ICollection<TblMaintable> TblMaintables { get; set; } = new List<TblMaintable>();
}
