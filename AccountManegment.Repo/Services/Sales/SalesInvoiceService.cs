using AccountManagement.DBContext.Models.ViewModels.SalesMaster;
using AccountManagement.Repository.Interface.Repository.Sales;
using AccountManagement.Repository.Interface.Services.SalesIInvoiceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Services.Sales
{
    public class SalesInvoiceService : ISalesInvoiceService
    {
        public SalesInvoiceService(ISalesInvoice saleInvoice)
        {
            SaleInvoice = saleInvoice;
        }

        public ISalesInvoice SaleInvoice { get; }

        public async Task<SalesInvoiceListView> GetSalesList(string searchText, string searchBy, string sortBy)
        {
            return await GetSalesList(searchText, searchBy, sortBy);
        }
    }
}
