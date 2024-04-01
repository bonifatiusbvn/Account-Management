using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class PodeliveryAddress
{
    public int Aid { get; set; }

    public Guid Poid { get; set; }

    public int? Quantity { get; set; }

    public int? UnitTypeId { get; set; }

    public string Address { get; set; } = null!;
    public bool? IsDeleted { get; set; }
}
