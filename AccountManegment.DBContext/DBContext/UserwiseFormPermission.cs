using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class UserwiseFormPermission
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public int FormId { get; set; }

    public bool IsAddAllow { get; set; }

    public bool IsViewAllow { get; set; }

    public bool IsEditAllow { get; set; }

    public bool IsDeleteAllow { get; set; }

    public bool? IsApproved { get; set; }

    public Guid Createdby { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual Form Form { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
