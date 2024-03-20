using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class UnitMaster
{
    public int UnitId { get; set; }

    public string UnitName { get; set; } = null!;

    public virtual ICollection<ItemMaster> ItemMasters { get; set; } = new List<ItemMaster>();
}
