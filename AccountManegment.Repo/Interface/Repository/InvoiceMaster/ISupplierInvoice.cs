using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.DataTableParameters;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Repository.InvoiceMaster
{
    public interface ISupplierInvoice
    {
        Task<SupplierInvoiceList> GetSupplierInvoiceList(string? searchText, string? searchBy, string? sortBy);
        Task<ApiResponseModel> AddSupplierInvoice(List<SupplierInvoiceModel> supplierInvoiceDetails);
        Task<SupplierInvoiceMasterView> GetSupplierInvoiceById(Guid Id);
        Task<ApiResponseModel> UpdateSupplierInvoice(SupplierInvoiceMasterView SupplierInvoiceDetails);
        Task<ApiResponseModel> DeleteSupplierInvoice(Guid InvoiceId);
        Task<jsonData> GetInvoiceDetailsById(DataTableRequstModel PayOutReport);
        Task<ApiResponseModel> InsertMultipleSupplierItemDetails(SupplierInvoiceMasterView SupplierItemDetails);
        string CheckSuppliersInvoiceNo(Guid? CompanyId);
        Task<IEnumerable<SupplierInvoiceModel>> GetSupplierInvoiceDetailsById(Guid SupplierId);
        Task<jsonData> GetSupplierInvoiceDetailsReport(DataTableRequstModel invoiceReport);
        Task<ApiResponseModel> DeletePayoutDetails(Guid InvoiceId);
        Task<SupplierInvoiceModel> GetPayoutDetailsbyId(Guid InvoiceId);
        Task<ApiResponseModel> UpdatePayoutDetails(SupplierInvoiceModel updatepayoutDetails);
        Task<ApiResponseModel> InvoiceIsApproved(InvoiceIsApprovedMasterModel InvoiceIdList);
        Task<ApiResponseModel> CheckOpeningBalance(Guid SupplierId, Guid CompanyId);
        Task<InvoiceTotalAmount> GetInvoiceDetailsPdfReport(InvoiceReportModel invoiceReport);
        Task<InvoiceTotalAmount> GetPayoutInvoiceDetailsPdfReport(InvoiceReportModel invoiceReport);
        Task<bool> CheckSupplierInvoiceNo(SupplierInvoiceModel InvoiceData);
    }
}
