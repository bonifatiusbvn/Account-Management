using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.Repository.Interface.Repository.InvoiceMaster;
using AccountManagement.Repository.Interface.Services.InvoiceMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Services.InvoiceMaster
{
    public class SupplierInvoiceService:ISupplierInvoiceService
    {
        public SupplierInvoiceService(ISupplierInvoice supplierInvoice)
        {
            SupplierInvoice = supplierInvoice;
        }
        public ISupplierInvoice SupplierInvoice { get; }

        public async Task<ApiResponseModel> AddSupplierInvoice(SupplierInvoiceModel SupplierInvoiceDetails)
        {
            return await SupplierInvoice.AddSupplierInvoice(SupplierInvoiceDetails);
        }

        public async Task<ApiResponseModel> DeleteSupplierInvoice(Guid InvoiceId)
        {
            return await SupplierInvoice.DeleteSupplierInvoice(InvoiceId);
        }

        public async Task<SupplierInvoiceModel> GetSupplierInvoiceById(Guid InvoiceId)
        {
            return await SupplierInvoice.GetSupplierInvoiceById(InvoiceId);
        }

        public async Task<IEnumerable<SupplierInvoiceModel>> GetSupplierInvoiceList(string? searchText, string? searchBy, string? sortBy)
        {
            return await SupplierInvoice.GetSupplierInvoiceList(searchText, searchBy, sortBy);
        }

        public async Task<ApiResponseModel> UpdateSupplierInvoice(SupplierInvoiceModel SupplierInvoiceDetails)
        {
            return await SupplierInvoice.UpdateSupplierInvoice(SupplierInvoiceDetails);
        }
    }
}
