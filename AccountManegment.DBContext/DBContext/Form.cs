using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class Form
{
    public int FormId { get; set; }

    public string? FormGroup { get; set; }

    public string FormName { get; set; } = null!;

    public string? Controller { get; set; }

    public int? OrderId { get; set; }

    public string? Action { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<RolewiseFormPermission> RolewiseFormPermissions { get; set; } = new List<RolewiseFormPermission>();
}
