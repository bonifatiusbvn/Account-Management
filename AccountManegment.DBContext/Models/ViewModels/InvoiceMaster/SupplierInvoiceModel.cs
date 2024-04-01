using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.InvoiceMaster
{
    public class SupplierInvoiceModel
    {
        public Guid Id { get; set; }
        public string? InvoiceId { get; set; }

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

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public string? SiteName { get; set; }

        public string? FromSupplierName { get; set; }

        public string? ToCompanyName { get; set; }
    }
}
