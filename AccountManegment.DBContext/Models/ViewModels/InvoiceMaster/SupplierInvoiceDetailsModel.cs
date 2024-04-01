using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.InvoiceMaster
{
    public class SupplierInvoiceDetailsModel
    {
        public int InvoiceDetailsId { get; set; }

        public Guid? RefInvoiceId { get; set; }

        public string Item { get; set; } = null!;

        public int UnitTypeId { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal? Discount { get; set; }
        public decimal? DiscountPer { get; set; }

        public decimal? Gstper { get; set; }

        public decimal? Gstamount { get; set; }

        public string? PaymentStatus { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string? UnitTypeName { get; set; }
        public Guid InvoiceId { get; set; }

        public Guid SiteId { get; set; }

        public Guid FromSupplierId { get; set; }

        public Guid ToCompanyId { get; set; }

        public decimal TotalAmount { get; set; }

        public string? Description { get; set; }

        public string? DeliveryShedule { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal? TotalDiscount { get; set; }

        public decimal TotalGstamount { get; set; }

        public decimal? Roundoff { get; set; }

        public string? SiteName { get; set; }

        public string? FromSupplierName { get; set; }

        public string? ToCompanyName { get; set; }
    }
}
