using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.InvoiceMaster
{
    public class SupplierInvoiceMasterView
    {
        public int InvoiceDetailsId { get; set; }

        public Guid? RefInvoiceId { get; set; }

        public string Item { get; set; } = null!;

        public int UnitTypeId { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal? DiscountPer { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal? Gstper { get; set; }

        public decimal? Gst { get; set; }

        public decimal TotalAmount { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string? UnitTypeName { get; set; }

        public Guid Id { get; set; }
        public string? InvoiceId { get; set; }

        public Guid? SiteId { get; set; }

        public Guid SupplierId { get; set; }

        public Guid CompanyId { get; set; }

        public DateTime? Date { get; set; }

        public string? Description { get; set; }

        public decimal TotalGstamount { get; set; }

        public decimal? TotalDiscount { get; set; }

        public decimal? Roundoff { get; set; }

        public string? PaymentStatus { get; set; }

        public bool? IsPayOut { get; set; }

        public string? SiteName { get; set; }

        public string? SupplierName { get; set; }

        public string? CompanyName { get; set; }

        public string? CompanyAddress { get; set; }

        public string? CompanyArea { get; set; }

        public int? CompanyCityId { get; set; }

        public string? CompanyCityName { get; set; }

        public int? CompanyStateId { get; set; }

        public string? CompanyStateName { get; set; }

        public int? CompanyCountryId { get; set; }

        public string? CompanyCountryName { get; set; }

        public string? CompanyGstNo { get; set; }

        public string? CompanyPincode { get; set; }

        public List<POItemDetailsModel>? ItemList { get; set; }
    }
}
