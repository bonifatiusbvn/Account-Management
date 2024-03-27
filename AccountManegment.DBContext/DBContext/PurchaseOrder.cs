﻿using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class PurchaseOrder
{
    public Guid Id { get; set; }

    public Guid SiteId { get; set; }

    public Guid FromSupplierId { get; set; }

    public Guid ToCompanyId { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Description { get; set; }

    public string? DeliveryShedule { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal? TotalDiscount { get; set; }

    public decimal TotalGstamount { get; set; }

    public string? BillingAddress { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual SupplierMaster FromSupplier { get; set; } = null!;

    public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
}
