using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblLocationrange
{
    public int LocationRangeId { get; set; }

    public string? LocationRangeValue { get; set; }

    public virtual ICollection<TblMaintable> TblMaintables { get; set; } = new List<TblMaintable>();
}
