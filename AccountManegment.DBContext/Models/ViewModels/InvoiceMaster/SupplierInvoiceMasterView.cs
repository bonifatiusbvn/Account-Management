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
        public decimal? TotalAmount { get; set; }
        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string? UnitTypeName { get; set; }

        public Guid Id { get; set; }

        public string? InvoiceNo { get; set; }

        public Guid? SiteId { get; set; }

        public Guid SupplierId { get; set; }

        public Guid CompanyId { get; set; }

        public DateTime? Date { get; set; }

        public string? Description { get; set; }

        public decimal TotalAmountInvoice { get; set; }

        public decimal TotalGstamount { get; set; }

        public decimal? TotalDiscount { get; set; }

        public decimal? Roundoff { get; set; }

        public string? PaymentStatus { get; set; }

        public bool? IsPayOut { get; set; }

        public string? ContactName { get; set; }

        public string? ContactNumber { get; set; }

        public string? SiteName { get; set; }

        public string? SupplierName { get; set; }

        public string? CompanyName { get; set; }

        public string? SupplierGstNo { get; set; }

        public string? SupplierEmail { get; set; }

        public string? SupplierBuildingName { get; set; }

        public string? SupplierArea { get; set; }

        public string? SupplierState { get; set; }

        public int? SupplierStateId { get; set; }

        public string? SupplierCity { get; set; }

        public int? SupplierCityId { get; set; }

        public string? SupplierPincode { get; set; }

        public string? SupplierBankName { get; set; }

        public string? SupplierAccountNo { get; set; }

        public string? SupplierIFSCCode { get; set; }

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
        public string? CompanyPanNo { get; set; }

        public List<POItemDetailsModel>? ItemList { get; set; }
    }
}
