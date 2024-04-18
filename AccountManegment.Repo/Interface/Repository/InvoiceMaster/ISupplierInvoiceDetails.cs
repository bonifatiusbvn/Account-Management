using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Repository.InvoiceMaster
{
    public interface ISupplierInvoiceDetails
    {
        Task<IEnumerable<SupplierInvoiceDetailsModel>> GetSupplierInvoiceDetailsList(string? searchText, string? searchBy, string? sortBy);
        Task<ApiResponseModel> AddSupplierInvoiceDetails(SupplierInvoiceDetailsModel SupplierInvoiceDetails);
        Task<SupplierInvoiceDetailsModel> GetSupplierInvoiceDetailsById(int InvoiceDetailsId);
        Task<ApiResponseModel> UpdateSupplierInvoiceDetails(SupplierInvoiceDetailsModel SupplierInvoiceDetails);
        Task<ApiResponseModel> DeleteSupplierInvoiceDetails(int InvoiceDetailsId);
        Task<IEnumerable<SupplierPendingDetailsModel>> GetSupplierPendingDetails(int InvoiceDetailsId);
    }
}
