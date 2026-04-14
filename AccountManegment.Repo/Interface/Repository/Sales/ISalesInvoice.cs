
using AccountManagement.DBContext.Models.ViewModels.SalesMaster;
using AccountManagement.DBContext.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseRequest;
using AccountManagement.DBContext.Models.DataTableParameters;

namespace AccountManagement.Repository.Interface.Repository.Sales
{
    public interface ISalesInvoice
    {
        string CheckSalesInvoiceNo(Guid? CompanyId);
        Task<ApiResponseModel> InsertSalesInvoiceDetails(SalesInvoiceMasterModel SalesInvoiceDetails);
        Task<SalesInvoiceListView> GetSalesList(string? searchText, string? searchBy, string? sortBy);
        Task<SalesInvoiceMasterModel> EditSalesInvoiceDetails(Guid Id);
        Task<ApiResponseModel> UpdateSalesInvoiceDetails(SalesInvoiceMasterModel SalesInvoiceDetails);
        Task<ApiResponseModel> DeleteSalesInvoiceDetails(Guid Id);
        Task<IEnumerable<InventoryInwardView>> GetInventoryList(string? searchText, string? searchBy, string? sortBy);
        Task<ApiResponseModel> InsertInventoryDetails(InventoryInwardView InventoryDetails);
        Task<InventoryInwardView> EditInventoryDetails(Guid InventoryId);
        Task<ApiResponseModel> UpdateInventoryDetails(InventoryInwardView InventoryDetails);
        Task<ApiResponseModel> DeleteInventoryDetails(Guid InventoryId);
        Task<ApiResponseModel> ApproveInventoryDetails(Guid InventoryId);
        Task<jsonData> SalesInvoiceReport(DataTableRequstModel SalesReport);
        Task<jsonData> SalesInvoicePaymentReport(DataTableRequstModel SalesPaymentReport);
        Task<ApiResponseModel> InsertSalesPayInInvoice(List<SalesInvoiceMasterModel> slaesPayInDetails);
        Task<SalesInvoiceMasterModel> EditSalesPayInInvoice(Guid SalesId);
        Task<ApiResponseModel> UpdateSalesPayInInvoice(SalesInvoiceMasterModel slaesPayInDetails);
        Task<ApiResponseModel> DeleteSalesPayInInvoice(Guid SalesId);
        Task<SalesInvoiceTotalAmount> SalesInvoicePdfReport(InvoiceReportModel SalesReport);
        Task<SalesInvoiceTotalAmount> SalesInvoicePaymentPdfReport(InvoiceReportModel SalesPaymentReport);
    }
}
