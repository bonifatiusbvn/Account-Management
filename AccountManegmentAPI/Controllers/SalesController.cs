using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.PurchaseRequest;
using AccountManagement.DBContext.Models.ViewModels.SalesMaster;
using AccountManagement.Repository.Interface.Services.SalesIInvoiceService;
using Microsoft.AspNetCore.Authorization;
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
        [HttpGet]
        [Route("EditSalesInvoiceDetails")]
        public async Task<IActionResult> EditSalesInvoiceDetails(Guid Id)
        {
            var SalesDetails = await SaleInvoice.EditSalesInvoiceDetails(Id);
            return Ok(new { code = 200, data = SalesDetails });
        }

        [HttpPost]
        [Route("UpdateSalesInvoiceDetails")]
        public async Task<IActionResult> UpdateSalesInvoiceDetails(SalesInvoiceMasterModel SalesInvoiceDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var SalesInvoicemaster = await SaleInvoice.UpdateSalesInvoiceDetails(SalesInvoiceDetails);
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
        [HttpGet]
        [Route("DeleteSalesInvoiceDetails")]
        public async Task<IActionResult> DeleteSalesInvoiceDetails(Guid Id)
        {
            ApiResponseModel response = new ApiResponseModel();
            var SalesInvoicemaster = await SaleInvoice.DeleteSalesInvoiceDetails(Id);
            if (SalesInvoicemaster.code == 200)
            {
                response.code = SalesInvoicemaster.code;
                response.message = SalesInvoicemaster.message;
            }
            else
            {
                response.code = (int)HttpStatusCode.NotFound;
                response.message = "There Is Some Problem In Your Request!";
            }
            return StatusCode(response.code, response);
        }

        [HttpPost]
        [Route("GetInventoryList")]
        public async Task<IActionResult> GetInventoryList(string? searchText, string? searchBy, string? sortBy)
        {
            IEnumerable<InventoryInwardView> InventoryList = await SaleInvoice.GetInventoryList(searchText, searchBy, sortBy);
            return Ok(new { code = 200, data = InventoryList.ToList() });
        }
    }
}
