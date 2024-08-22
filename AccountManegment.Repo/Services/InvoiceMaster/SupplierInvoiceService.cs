using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.Repository.Interface.Repository.InvoiceMaster;
using AccountManagement.Repository.Interface.Services.InvoiceMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Services.InvoiceMaster
{
    public class SupplierInvoiceService : ISupplierInvoiceService
    {
        public SupplierInvoiceService(ISupplierInvoice supplierInvoice)
        {
            SupplierInvoice = supplierInvoice;
        }
        public ISupplierInvoice SupplierInvoice { get; }

        public async Task<ApiResponseModel> AddSupplierInvoice(List<SupplierInvoiceModel> supplierInvoiceDetails)
        {
            return await SupplierInvoice.AddSupplierInvoice(supplierInvoiceDetails);
        }

        public async Task<ApiResponseModel> DeleteSupplierInvoice(Guid Id)
        {
            return await SupplierInvoice.DeleteSupplierInvoice(Id);
        }

        public async Task<InvoiceTotalAmount> GetInvoiceDetailsById(Guid CompanyId, Guid SupplierId)
        {
            return await SupplierInvoice.GetInvoiceDetailsById(CompanyId, SupplierId);
        }

        public async Task<SupplierInvoiceMasterView> GetSupplierInvoiceById(Guid Id)
        {
            return await SupplierInvoice.GetSupplierInvoiceById(Id);
        }

        public async Task<IEnumerable<SupplierInvoiceModel>> GetSupplierInvoiceList(string? searchText, string? searchBy, string? sortBy)
        {
            return await SupplierInvoice.GetSupplierInvoiceList(searchText, searchBy, sortBy);
        }

        public async Task<ApiResponseModel> InsertMultipleSupplierItemDetails(SupplierInvoiceMasterView SupplierItemDetails)
        {
            return await SupplierInvoice.InsertMultipleSupplierItemDetails(SupplierItemDetails);
        }

        public async Task<ApiResponseModel> UpdateSupplierInvoice(SupplierInvoiceMasterView SupplierInvoiceDetails)
        {
            return await SupplierInvoice.UpdateSupplierInvoice(SupplierInvoiceDetails);
        }


        public string CheckSuppliersInvoiceNo(Guid? CompanyId)
        {
            return SupplierInvoice.CheckSuppliersInvoiceNo(CompanyId);
        }

        public async Task<IEnumerable<SupplierInvoiceModel>> GetSupplierInvoiceDetailsById(Guid SupplierId)
        {
            return await SupplierInvoice.GetSupplierInvoiceDetailsById(SupplierId);
        }

        public async Task<IEnumerable<SupplierInvoiceModel>> GetSupplierInvoiceDetailsReport(InvoiceReportModel invoiceReport)
        {
            return await SupplierInvoice.GetSupplierInvoiceDetailsReport(invoiceReport);
        }

        public async Task<ApiResponseModel> DeletePayoutDetails(Guid InvoiceId)
        {
            return await SupplierInvoice.DeletePayoutDetails(InvoiceId);
        }

        public async Task<SupplierInvoiceModel> GetPayoutDetailsbyId(Guid InvoiceId)
        {
            return await SupplierInvoice.GetPayoutDetailsbyId(InvoiceId);
        }
        public async Task<ApiResponseModel> UpdatePayoutDetails(SupplierInvoiceModel updatepayoutDetails)
        {
            return await SupplierInvoice.UpdatePayoutDetails(updatepayoutDetails);
        }

        public async Task<ApiResponseModel> InvoiceIsApproved(InvoiceIsApprovedMasterModel InvoiceIdList)
        {
            return await SupplierInvoice.InvoiceIsApproved(InvoiceIdList);
        }
    }
}
