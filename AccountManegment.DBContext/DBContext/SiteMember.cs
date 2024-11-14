using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class SiteMember
{
    public Guid Id { get; set; }

    public Guid? SiteId { get; set; }

    public Guid? UserId { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual Site? Site { get; set; }

    public virtual User? User { get; set; }
}
