using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblDistrict
{
    public int DistrictId { get; set; }

    public int? ProvinceId { get; set; }

    public string? DistrictName { get; set; }

    public virtual TblProvince? Province { get; set; }

    public virtual ICollection<TblLocality> TblLocalities { get; set; } = new List<TblLocality>();

    public virtual ICollection<TblMaintable> TblMaintables { get; set; } = new List<TblMaintable>();
}
