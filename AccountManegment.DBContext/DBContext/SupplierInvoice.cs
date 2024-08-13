using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class SupplierInvoice
{
    public Guid Id { get; set; }

    public string? InvoiceNo { get; set; }

    public Guid? SiteId { get; set; }

    public Guid SupplierId { get; set; }

    public Guid CompanyId { get; set; }

    public string? SupplierInvoiceNo { get; set; }

    public DateTime? Date { get; set; }

    public string? ChallanNo { get; set; }

    public string? Lrno { get; set; }

    public string? VehicleNo { get; set; }

    public string? DispatchBy { get; set; }

    public string? PaymentTerms { get; set; }

    public string? Description { get; set; }

    public string? SiteGroup { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal TotalGstamount { get; set; }

    public decimal? TotalDiscount { get; set; }

    public decimal? DiscountRoundoff { get; set; }

    public decimal? Roundoff { get; set; }

    public string? PaymentStatus { get; set; }

    public bool? IsPayOut { get; set; }

    public string? ContactName { get; set; }

    public string? ContactNumber { get; set; }

    public string? ShippingAddress { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual Site? Site { get; set; }
}
