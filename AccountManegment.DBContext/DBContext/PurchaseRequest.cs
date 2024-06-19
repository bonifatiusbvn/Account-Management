using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class PurchaseRequest
{
    public Guid Pid { get; set; }

    public Guid SiteId { get; set; }

    public Guid? ItemId { get; set; }

    public string? ItemName { get; set; }

    public int UnitTypeId { get; set; }

    public decimal Quantity { get; set; }

    public string PrNo { get; set; } = null!;

    public int? SiteAddressId { get; set; }

    public string? SiteAddress { get; set; }

    public bool? IsApproved { get; set; }

    public bool? IsDeleted { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual ItemMaster? Item { get; set; }

    public virtual Site Site { get; set; } = null!;

    public virtual UnitMaster UnitType { get; set; } = null!;
}
