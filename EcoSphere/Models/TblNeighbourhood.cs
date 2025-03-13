using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblNeighbourhood
{
    public int NeighbourhoodId { get; set; }

    public int? LocalityId { get; set; }

    public string? HoodName { get; set; }

    public virtual TblLocality? Locality { get; set; }

    public virtual ICollection<TblMaintable> TblMaintables { get; set; } = new List<TblMaintable>();
}
