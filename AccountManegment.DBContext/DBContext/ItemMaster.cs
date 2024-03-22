using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class ItemMaster
{
    public Guid ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public int UnitType { get; set; }

    public decimal PricePerUnit { get; set; }

    public bool IsWithGst { get; set; }

    public decimal? Gstamount { get; set; }

    public decimal? Gstper { get; set; }

    public string? Hsncode { get; set; }

    public bool? IsApproved { get; set; }

    public bool? IsDeleted { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual UnitMaster UnitTypeNavigation { get; set; } = null!;
}
