using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblMigrationstatus
{
    public int MigrationStatusId { get; set; }

    public string? MigrationStatusName { get; set; }

    public virtual ICollection<TblMaintable> TblMaintables { get; set; } = new List<TblMaintable>();
}
