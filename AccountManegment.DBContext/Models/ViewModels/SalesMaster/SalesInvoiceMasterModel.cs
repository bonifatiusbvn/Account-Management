using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.SalesMaster
{
    public class SalesInvoiceMasterModel
    {
        public Guid Id { get; set; }

        public string? SalesInvoiceNo { get; set; }

        public string? InvoiceType { get; set; }

        public Guid? SiteId { get; set; }
        public string? SiteName { get; set; }

        public Guid SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierFullAddress { get; set; }
        public string? SupplierMobileNo { get; set; }
        public string? SupplierGstNo { get; set; }
        public string? SupplierStateName { get; set; }
        public int? SupplierStateCode { get; set; }

        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyFullAddress { get; set; }
        public string? CompanyGstNo { get; set; }
        public string? CompanyStateName { get; set; }
        public int? CompanyStateCode { get; set; }
        public string? CompanyBankName { get; set; }

        public string? CompanyBankBranch { get; set; }

        public string? CompanyAccountNo { get; set; }

        public string? CompanyIffccode { get; set; }
        public string? SupplierInvoiceNo { get; set; }

        public DateTime? Date { get; set; }

        public string? ChallanNo { get; set; }

        public string? Lrno { get; set; }

        public string? VehicleNo { get; set; }

        public string? DispatchBy { get; set; }

        public string? PaymentTerms { get; set; }

        public string? Description { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TotalGstamount { get; set; }

        public decimal? TotalDiscount { get; set; }

        public decimal? DiscountRoundoff { get; set; }

        public decimal? Tds { get; set; }

        public string? PaymentStatus { get; set; }

        public bool? IsPayOut { get; set; }

        public string? ContactName { get; set; }

        public string? ContactNumber { get; set; }

        public string? ShippingAddress { get; set; }

        public bool? IsApproved { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public decimal? PayOutTotalAmount { get; set; }
        public decimal? NonPayOutTotalAmount { get; set; }
        public decimal? NetAmount { get; set; }
        public List<SalesInvoiceDetailsModel>? SalesInvoiceDetails { get; set; }
        public List<POItemDetailsModel>? SalesItemList { get; set; }
    }

    public class SalesInvoiceDetailsModel
    {
        public int InvoiceDetailsId { get; set; }

        public Guid? RefSalesInvoiceId { get; set; }

        public Guid ItemId { get; set; }

        public string? ItemName { get; set; }

        public string? ItemDescription { get; set; }

        public int UnitTypeId { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal? DiscountPer { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal? Gstper { get; set; }

        public decimal? Gst { get; set; }

        public decimal? TotalAmount { get; set; }

        public DateTime? Date { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
    public class SalesInvoiceListView
    {
        public IEnumerable<SalesInvoiceMasterModel>? SalesInvoiceList { get; set; }
        public IDictionary<Guid, IEnumerable<SalesInvoiceDetailsModel>>? SalesInvoiceItemList { get; set; }
    }
    public class SalesInvoiceReportModel
    {
        public Guid? SupplierId { get; set; }
        public Guid? CompanyId { get; set; }
        public string? filterType { get; set; }
        public DateTime? TillMonth { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string? SelectedYear { get; set; }
        public string? sortBy { get; set; }
        public string? SupplierName { get; set; }
        public string? CompanyName { get; set; }
    }
    public class SalesInvoiceTotalAmount
    {
        public IEnumerable<SalesInvoiceMasterModel> SalesInvoiceList { get; set; }
        public decimal TotalPending { get; set; }
        public decimal TotalCreadit { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal TotalPurchase { get; set; }
    }
}
