using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SalesMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Services.SalesIInvoiceService
{
    public interface ISalesInvoiceService
    {
        string CheckSalesInvoiceNo(Guid? CompanyId);
        Task<ApiResponseModel> InsertSalesInvoiceDetails(SalesInvoiceMasterModel SalesInvoiceDetails);
        Task<SalesInvoiceListView> GetSalesList(string? searchText, string? searchBy, string? sortBy);
        Task<SalesInvoiceMasterModel> EditSalesInvoiceDetails(Guid Id);
        Task<ApiResponseModel> UpdateSalesInvoiceDetails(SalesInvoiceMasterModel SalesInvoiceDetails);
        Task<ApiResponseModel> DeleteSalesInvoiceDetails(Guid Id);
    }
}
