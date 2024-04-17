using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Services.InvoiceMaster
{
    public interface ISupplierInvoiceService
    {
        Task<IEnumerable<SupplierInvoiceModel>> GetSupplierInvoiceList(string? searchText, string? searchBy, string? sortBy);
        Task<ApiResponseModel> AddSupplierInvoice(SupplierInvoiceModel SupplierInvoiceDetails);
        Task<SupplierInvoiceMasterView> GetSupplierInvoiceById(Guid Id);
        Task<ApiResponseModel> UpdateSupplierInvoice(SupplierInvoiceMasterView SupplierInvoiceDetails);
        Task<ApiResponseModel> DeleteSupplierInvoice(Guid InvoiceId);
        Task<InvoiceTotalAmount> GetInvoiceDetailsById(Guid CompanyId, Guid SupplierId);
        Task<ApiResponseModel> InsertMultipleSupplierItemDetails(SupplierInvoiceMasterView SupplierItemDetails);
        string CheckSupplierInvoiceNo();
        Task<IEnumerable<SupplierInvoiceModel>> GetSupplierInvoiceDetailsById(Guid SupplierId);

    }
}
