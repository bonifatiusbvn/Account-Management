using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
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
        string CheckSalesInvoiceNo(Guid? CompanyId);
        Task<ApiResponseModel> InsertSalesInvoiceDetails(SalesInvoiceMasterModel SalesInvoiceDetails);
    }
}
