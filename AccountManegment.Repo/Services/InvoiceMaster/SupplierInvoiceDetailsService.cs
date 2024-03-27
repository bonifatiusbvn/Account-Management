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
    public class SupplierInvoiceDetailsService:ISupplierInvoiceDetailsService
    {
        public SupplierInvoiceDetailsService(ISupplierInvoiceDetails supplierinvoiceDetails)
        {
            supplierInvoiceDetails = supplierinvoiceDetails;
        }

        public ISupplierInvoiceDetails supplierInvoiceDetails { get; }

        public async Task<ApiResponseModel> AddSupplierInvoiceDetails(SupplierInvoiceDetailsModel SupplierInvoiceDetails)
        {
            return await supplierInvoiceDetails.AddSupplierInvoiceDetails(SupplierInvoiceDetails);
        }

        public async Task<ApiResponseModel> DeleteSupplierInvoiceDetails(int InvoiceDetailsId)
        {
            return await supplierInvoiceDetails.DeleteSupplierInvoiceDetails(InvoiceDetailsId);
        }

        public async Task<SupplierInvoiceDetailsModel> GetSupplierInvoiceDetailsById(int InvoiceDetailsId)
        {
            return await supplierInvoiceDetails.GetSupplierInvoiceDetailsById(InvoiceDetailsId);
        }

        public async Task<IEnumerable<SupplierInvoiceDetailsModel>> GetSupplierInvoiceDetailsList(string? searchText, string? searchBy, string? sortBy)
        {
            return await supplierInvoiceDetails.GetSupplierInvoiceDetailsList(searchText, searchBy, sortBy);
        }

        public async Task<ApiResponseModel> UpdateSupplierInvoiceDetails(SupplierInvoiceDetailsModel SupplierInvoiceDetails)
        {
            return await supplierInvoiceDetails.UpdateSupplierInvoiceDetails(SupplierInvoiceDetails);
        }
    }
}
