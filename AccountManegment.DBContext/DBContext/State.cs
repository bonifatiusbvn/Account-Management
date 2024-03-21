using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class State
{
    public int StatesId { get; set; }

    public string StatesName { get; set; } = null!;

    public int CountryId { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<SupplierMaster> SupplierMasters { get; set; } = new List<SupplierMaster>();
}
