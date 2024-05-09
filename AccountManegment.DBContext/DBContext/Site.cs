using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class Site
{
    public Guid SiteId { get; set; }

    public string SiteName { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? ContectPersonName { get; set; }

    public string? ContectPersonPhoneNo { get; set; }

    public string Address { get; set; } = null!;

    public string Area { get; set; } = null!;

    public int CityId { get; set; }

    public int StateId { get; set; }

    public int Country { get; set; }

    public string? Pincode { get; set; }

    public string ShippingAddress { get; set; } = null!;

    public string ShippingArea { get; set; } = null!;

    public int ShippingCityId { get; set; }

    public int ShippingStateId { get; set; }

    public int ShippingCountry { get; set; }

    public string? ShippingPincode { get; set; }

    public bool? IsDeleted { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual ICollection<PurchaseRequest> PurchaseRequests { get; set; } = new List<PurchaseRequest>();

    public virtual ICollection<SupplierInvoice> SupplierInvoices { get; set; } = new List<SupplierInvoice>();
}
