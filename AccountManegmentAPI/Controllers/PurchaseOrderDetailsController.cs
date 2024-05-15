using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.Repository.Interface.Services.PurchaseOrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountManagement.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderDetailsController : ControllerBase
    {
        public PurchaseOrderDetailsController(IPurchaseOrderDetailsServices purchaseOrder)
        {
            PurchaseOrder = purchaseOrder;
        }
        public IPurchaseOrderDetailsServices PurchaseOrder { get; }

        [HttpGet]
        [Route("GetPurchaseOrderDetailsList")]
        public async Task<IActionResult> GetPurchaseOrderDetailsList()
        {
            IEnumerable<PurchaseOrderDetailsModel> PurchaseOrderList = await PurchaseOrder.GetPurchaseOrderDetailsList();
            return Ok(new { code = 200, data = PurchaseOrderList.ToList() });
        }

        [HttpGet]
        [Route("GetPurchaseOrderDetailsById")]
        public async Task<IActionResult> GetPurchaseOrderDetailsById(int Id)
        {
            var purchaseOrder = await PurchaseOrder.GetPurchaseOrderDetailsById(Id);
            return Ok(new { code = 200, data = purchaseOrder });
        }

        [HttpPost]
        [Route("AddPurchaseOrderDetails")]
        public async Task<IActionResult> AddPurchaseOrderDetails(PurchaseOrderDetailsModel PurchaseOrderDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var PurchaseOrdermaster = await PurchaseOrder.AddPurchaseOrderDetails(PurchaseOrderDetails);
            if (PurchaseOrdermaster.code == 200)
            {
                response.code = PurchaseOrdermaster.code;
                response.message = PurchaseOrdermaster.message;
            }
            else
            {
                response.code = (int)HttpStatusCode.BadRequest;
            }

            return StatusCode(response.code, response);
        }
        [HttpPost]
        [Route("UpdatePurchaseOrderDetails")]
        public async Task<IActionResult> UpdatePurchaseOrderDetails(PurchaseOrderDetailsModel PurchaseOrderDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var PurchaseOrdermaster = await PurchaseOrder.UpdatePurchaseOrderDetails(PurchaseOrderDetails);
            if (PurchaseOrdermaster.code == 200)
            {
                response.code = PurchaseOrdermaster.code;
                response.message = PurchaseOrdermaster.message;
            }
            else
            {
                response.code = (int)HttpStatusCode.BadRequest;
            }

            return StatusCode(response.code, response);
        }

    }
}
