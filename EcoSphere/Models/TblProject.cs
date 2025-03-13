using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblProject
{
    public int ProjectId { get; set; }

    public string? ProjectName { get; set; }

    public virtual ICollection<TblMaintable> TblMaintables { get; set; } = new List<TblMaintable>();
}
