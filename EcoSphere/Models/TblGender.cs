using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblGender
{
    public int GenderId { get; set; }

    public string? GenderName { get; set; }

    public virtual ICollection<TblMaintable> TblMaintables { get; set; } = new List<TblMaintable>();
}
