using System;
using System.Collections.Generic;

namespace EcoSphere.Models;

public partial class TblUseraction
{
    public int UserActionId { get; set; }

    public int? UserId { get; set; }

    public string? Action { get; set; }

    public DateTime? ActionTime { get; set; }

    public virtual TblUser? User { get; set; }
}
