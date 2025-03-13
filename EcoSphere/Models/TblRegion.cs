using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblRegion
{
    public int RegionId { get; set; }

    public string? RegionName { get; set; }

    public virtual ICollection<TblMaintable> TblMaintables { get; set; } = new List<TblMaintable>();

    public virtual ICollection<TblProvince> TblProvinces { get; set; } = new List<TblProvince>();
}
