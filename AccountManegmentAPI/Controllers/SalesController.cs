using AccountManagement.DBContext.Models.ViewModels.PurchaseRequest;
using AccountManagement.DBContext.Models.ViewModels.SalesMaster;
using AccountManagement.Repository.Interface.Services.SalesIInvoiceService;
using AccountManagement.Repository.Services.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesController : ControllerBase
    {
        public SalesController(SalesInvoiceService saleInvoice, ISalesInvoiceService salesInvoice)
        {
            SaleInvoice = saleInvoice;
            SalesInvoice = salesInvoice;
        }

        public SalesInvoiceService SaleInvoice { get; }
        public ISalesInvoiceService SalesInvoice { get; }

        [HttpPost]
        [Route("GetSalestInvoiceList")]
        public async Task<IActionResult> GetSalestInvoiceList(string searchText, string searchBy, string sortBy)
        {
            var SalestInvoiceList = await SalesInvoice.GetSalesList(searchText, searchBy, sortBy);
            return Ok(new { code = 200, data = SalestInvoiceList });
        }

    }
}
