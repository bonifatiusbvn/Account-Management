using AccountManagement.DBContext.Models.ViewModels.SalesMaster;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SalesMaster;
using AccountManagement.Repository.Interface.Repository.Sales;
using AccountManagement.Repository.Interface.Services.SalesIInvoiceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManagement.API;
using AccountManagement.DBContext.Models.DataTableParameters;

namespace AccountManagement.Repository.Services.Sales
{
    public class SalesInvoiceService : ISalesInvoiceService
    {
        public SalesInvoiceService(ISalesInvoice saleInvoice)
        {
            SaleInvoice = saleInvoice;
        }

        public ISalesInvoice SaleInvoice { get; }

        public string CheckSalesInvoiceNo(Guid? CompanyId)
        {
            return SaleInvoice.CheckSalesInvoiceNo(CompanyId);
        }

        public async Task<ApiResponseModel> InsertSalesInvoiceDetails(SalesInvoiceMasterModel SalesInvoiceDetails)
        {
            return await SaleInvoice.InsertSalesInvoiceDetails(SalesInvoiceDetails);
        }

        public async Task<SalesInvoiceListView> GetSalesList(string? searchText, string? searchBy, string? sortBy)
        {
            return await SaleInvoice.GetSalesList(searchText, searchBy, sortBy);
        }

        public async Task<SalesInvoiceMasterModel> EditSalesInvoiceDetails(Guid Id)
        {
            return await SaleInvoice.EditSalesInvoiceDetails(Id);
        }

        public async Task<ApiResponseModel> UpdateSalesInvoiceDetails(SalesInvoiceMasterModel SalesInvoiceDetails)
        {
            return await SaleInvoice.UpdateSalesInvoiceDetails(SalesInvoiceDetails);
        }

        public async Task<ApiResponseModel> DeleteSalesInvoiceDetails(Guid Id)
        {
            return await SaleInvoice.DeleteSalesInvoiceDetails(Id);
        }

        public async Task<IEnumerable<InventoryInwardView>> GetInventoryList(string? searchText, string? searchBy, string? sortBy)
        {
            return await SaleInvoice.GetInventoryList(searchText, searchBy, sortBy);
        }

        public async Task<ApiResponseModel> InsertInventoryDetails(InventoryInwardView InventoryDetails)
        {
            return await SaleInvoice.InsertInventoryDetails(InventoryDetails);
        }

        public async Task<InventoryInwardView> EditInventoryDetails(Guid InventoryId)
        {
            return await SaleInvoice.EditInventoryDetails(InventoryId);
        }

        public async Task<ApiResponseModel> UpdateInventoryDetails(InventoryInwardView InventoryDetails)
        {
            return await SaleInvoice.UpdateInventoryDetails(InventoryDetails);
        }

        public async Task<ApiResponseModel> DeleteInventoryDetails(Guid InventoryId)
        {
            return await SaleInvoice.DeleteInventoryDetails(InventoryId);
        }

        public async Task<ApiResponseModel> ApproveInventoryDetails(Guid InventoryId)
        {
            return await SaleInvoice.ApproveInventoryDetails(InventoryId);
        }

        public async Task<jsonData> SalesInvoiceReport(DataTableRequstModel SalesReport)
        {
            return await SaleInvoice.SalesInvoiceReport(SalesReport);
        }
    }
}
