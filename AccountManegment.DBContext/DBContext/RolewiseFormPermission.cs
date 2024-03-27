using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class RolewiseFormPermission
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public Guid UserId { get; set; }

    public int FormId { get; set; }

    public bool IsViewAllow { get; set; }

    public bool IsEditAllow { get; set; }

    public bool IsDeleteAllow { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual Form Form { get; set; } = null!;
}
