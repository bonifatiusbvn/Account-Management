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
        [Route("InsertInventoryDetails")]
        public async Task<IActionResult> InsertInventoryDetails(InventoryInwardView InventoryDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var Inventory = await SaleInvoice.InsertInventoryDetails(InventoryDetails);
            if (Inventory.code == 200)
            {
                response.code = Inventory.code;
                response.message = Inventory.message;
            }
            else
            {
                response.code = Inventory.code;
                response.message = Inventory.message;
            }
            return StatusCode(response.code, response);
        }
        [HttpGet]
        [Route("EditInventoryDetails")]
        public async Task<IActionResult> EditInventoryDetails(Guid Id)
        {
            var Inventory = await SaleInvoice.EditInventoryDetails(Id);
            return Ok(new { code = 200, data = Inventory });
        }

        [HttpPost]
        [Route("UpdateInventoryDetails")]
        public async Task<IActionResult> UpdateInventoryDetails(InventoryInwardView InventoryDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var Inventory = await SaleInvoice.UpdateInventoryDetails(InventoryDetails);
            if (Inventory.code == 200)
            {
                response.code = Inventory.code;
                response.message = Inventory.message;
            }
            else
            {
                response.code = Inventory.code;
                response.message = Inventory.message;
            }
            return StatusCode(response.code, response);
        }
        [HttpGet]
        [Route("DeleteInventoryDetails")]
        public async Task<IActionResult> DeleteInventoryDetails(Guid InventoryId)
        {
            ApiResponseModel response = new ApiResponseModel();
            var Inventory = await SaleInvoice.DeleteInventoryDetails(InventoryId);
            if (Inventory.code == 200)
            {
                response.code = Inventory.code;
                response.message = Inventory.message;
            }
            else
            {
                response.code = Inventory.code;
                response.message = Inventory.message;
            }
            return StatusCode(response.code, response);
        }
    }
}
