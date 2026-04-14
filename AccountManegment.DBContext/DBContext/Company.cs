using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class Company
{
    public Guid CompanyId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? InvoicePef { get; set; }

    public string Gstno { get; set; } = null!;

    public string? PanNo { get; set; }

    public string? Address { get; set; }

    public string? Area { get; set; }

    public int CityId { get; set; }

    public int StateId { get; set; }

    public int Country { get; set; }

    public string Pincode { get; set; } = null!;

    public string? BankName { get; set; }

    public string? BankBranch { get; set; }

    public string? AccountNo { get; set; }

    public string? Iffccode { get; set; }

    public bool? IsDelete { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();
}
