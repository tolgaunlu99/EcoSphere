using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblProvince
{
    public int ProvinceId { get; set; }

    public int? RegionId { get; set; }

    public string? ProvinceName { get; set; }

    public virtual TblRegion? Region { get; set; }

    public virtual ICollection<TblDistrict> TblDistricts { get; set; } = new List<TblDistrict>();

    public virtual ICollection<TblMaintable> TblMaintables { get; set; } = new List<TblMaintable>();
}
