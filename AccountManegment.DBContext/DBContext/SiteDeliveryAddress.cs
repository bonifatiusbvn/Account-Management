using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class SiteDeliveryAddress
{
    public int Id { get; set; }

    public Guid SiteId { get; set; }

    public string Address { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public virtual Site Site { get; set; } = null!;
}
