using AccountManagement.DBContext.Models.ViewModels.PurchaseRequest;
using AccountManagement.DBContext.Models.ViewModels.SalesMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Repository.Sales
{
    public interface ISalesInvoice
    {
        Task<SalesInvoiceListView> GetSalesList(string searchText, string searchBy, string sortBy);
    }
}
