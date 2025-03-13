using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblEndemicstatus
{
    public int EndemicStatusId { get; set; }

    public string? EndemicStatus { get; set; }

    public virtual ICollection<TblMaintable> TblMaintables { get; set; } = new List<TblMaintable>();
}
