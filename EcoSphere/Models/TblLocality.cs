using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblLocality
{
    public int LocalityId { get; set; }

    public int? DistrictId { get; set; }

    public string? LocalityName { get; set; }

    public virtual TblDistrict? District { get; set; }

    public virtual ICollection<TblNeighbourhood> TblNeighbourhoods { get; set; } = new List<TblNeighbourhood>();
}
