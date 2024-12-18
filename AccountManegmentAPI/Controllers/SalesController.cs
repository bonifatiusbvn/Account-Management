using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SalesMaster;
using AccountManagement.Repository.Interface.Services.SalesIInvoiceService;
using AccountManagement.DBContext.Models.ViewModels.PurchaseRequest;
using AccountManagement.DBContext.Models.ViewModels.SalesMaster;
using AccountManagement.Repository.Interface.Services.SalesIInvoiceService;
using AccountManagement.Repository.Services.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesController : ControllerBase
    {

        public SalesController(ISalesInvoiceService saleInvoice)
        {
            SaleInvoice = saleInvoice;

        }

        public ISalesInvoiceService SaleInvoice { get; }

        [HttpGet]
        [Route("CheckSalesInvoiceNo")]
        public IActionResult CheckSalesInvoiceNo(Guid? CompanyId)
        {
            var checkInvoiceNo = SaleInvoice.CheckSalesInvoiceNo(CompanyId);
            return Ok(new { code = 200, data = checkInvoiceNo });
        }

        [HttpGet]
        [Route("GetSalestInvoiceList")]
        public async Task<IActionResult> GetSalestInvoiceList(string? searchText, string? searchBy, string? sortBy)
        {
            var checkInvoiceNo = await SaleInvoice.GetSalesList(searchText, searchBy, sortBy);
            return Ok(new { code = 200, data = checkInvoiceNo });
        }


        [HttpPost]
        [Route("InsertSalesInvoiceDetails")]
        public async Task<IActionResult> InsertSalesInvoiceDetails(SalesInvoiceMasterModel SalesInvoiceDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var SalesInvoicemaster = await SaleInvoice.InsertSalesInvoiceDetails(SalesInvoiceDetails);
            if (SalesInvoicemaster.code == 200)
            {
                response.code = SalesInvoicemaster.code;
                response.message = SalesInvoicemaster.message;
                response.data = SalesInvoicemaster.data;
            }
            else
            {
                response.code = (int)HttpStatusCode.NotFound;
                response.message = "There Is Some Problem In Your Request!";
            }
            return StatusCode(response.code, response);
        }

    }
}
