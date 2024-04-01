using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class PurchaseOrderDetail
{
    public int Id { get; set; }

    public Guid PorefId { get; set; }

    public Guid? ItemId { get; set; }

    public string Item { get; set; } = null!;

    public int UnitTypeId { get; set; }

    public decimal Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal? Discount { get; set; }

    public decimal Gst { get; set; }

    public decimal ItemTotal { get; set; }
    public bool? IsDeleted { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual ItemMaster? ItemNavigation { get; set; }

    public virtual PurchaseOrder Poref { get; set; } = null!;

    public virtual UnitMaster UnitType { get; set; } = null!;
}
