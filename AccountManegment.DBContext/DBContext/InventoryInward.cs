using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class InventoryInward
{
    public Guid Id { get; set; }

    public Guid? SiteId { get; set; }

    public Guid ItemId { get; set; }

    public string Item { get; set; } = null!;

    public int UnitTypeId { get; set; }

    public decimal Quantity { get; set; }

    public DateTime? Date { get; set; }

    public string? Details { get; set; }

    public bool? IsApproved { get; set; }

    public bool? IsDeleted { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual ItemMaster ItemNavigation { get; set; } = null!;

    public virtual UnitMaster UnitType { get; set; } = null!;
}
