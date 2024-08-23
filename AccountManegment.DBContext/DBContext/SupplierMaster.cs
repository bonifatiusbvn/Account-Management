using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class SupplierMaster
{
    public Guid SupplierId { get; set; }

    public string SupplierName { get; set; } = null!;

    public string? Mobile { get; set; }

    public string? Email { get; set; }

    public string? Gstno { get; set; }

    public string? BuildingName { get; set; }

    public string Area { get; set; } = null!;

    public int State { get; set; }

    public int City { get; set; }

    public string? PinCode { get; set; }

    public string? BankName { get; set; }

    public string? BankBranch { get; set; }

    public string? AccountNo { get; set; }

    public string? Iffccode { get; set; }

    public bool IsApproved { get; set; }

    public decimal? OpeningBalance { get; set; }

    public DateTime? OpeningBalanceDate { get; set; }

    public bool? IsDelete { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual City CityNavigation { get; set; } = null!;

    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();

    public virtual State StateNavigation { get; set; } = null!;
}
