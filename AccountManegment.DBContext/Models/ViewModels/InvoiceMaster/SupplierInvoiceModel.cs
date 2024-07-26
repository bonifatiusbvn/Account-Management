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

        public string? InvoiceNo { get; set; }

        public Guid? SiteId { get; set; }

        public Guid SupplierId { get; set; }

        public Guid CompanyId { get; set; }

        public string? SupplierInvoiceNo { get; set; }

        public DateTime? Date { get; set; }

        public string? Description { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TotalGstamount { get; set; }

        public decimal? TotalDiscount { get; set; }

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

        public string? SiteName { get; set; }

        public string? SupplierName { get; set; }

        public string? CompanyName { get; set; }
    }
    public class InvoiceTotalAmount
    {
        public IEnumerable<SupplierInvoiceModel> InvoiceList { get; set; }
        public decimal TotalPending { get; set; }
        public decimal TotalCreadit { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal TotalPurchase { get; set; }
    }

    public class InvoiceReportModel
    {
        public Guid? SiteId { get; set; }
        public Guid? SupplierId { get; set; }
        public Guid? CompanyId { get; set; }
        public string? filterType { get; set; }  
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set;}
    }
}
