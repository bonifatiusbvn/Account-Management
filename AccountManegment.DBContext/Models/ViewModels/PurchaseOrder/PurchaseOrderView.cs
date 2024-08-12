using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.PurchaseOrder
{
    public class PurchaseOrderView
    {
        public Guid Id { get; set; }

        public Guid SiteId { get; set; }

        public string? Poid { get; set; }

        public Guid FromSupplierId { get; set; }

        public Guid ToCompanyId { get; set; }

        public decimal TotalAmount { get; set; }

        public string? Terms { get; set; }

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

        public string? SupplierName { get; set; }
        
        public string? CompanyName { get; set; }
        
        public string? SiteName { get; set; }

        public DateTime? Date { get; set; }

        public string? DispatchBy { get; set; }

        public string? BuyersPurchaseNo { get; set; }

        public string? PaymentTerms { get; set; }
    }
}
