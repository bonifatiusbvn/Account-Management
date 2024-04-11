using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.Repository.Interface.Services.PurchaseOrderService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        public PurchaseOrderController(IPurchaseOrderServices purchaseOrder)
        {
            PurchaseOrder = purchaseOrder;
        }
        public IPurchaseOrderServices PurchaseOrder { get; }

        [HttpPost]
        [Route("GetPurchaseOrderList")]
        public async Task<IActionResult> GetPurchaseOrderList(string? searchText, string? searchBy, string? sortBy)
        {
            IEnumerable<PurchaseOrderView> PurchaseOrderList = await PurchaseOrder.GetPurchaseOrderList(searchText,searchBy,sortBy);
            return Ok(new { code = 200, data = PurchaseOrderList.ToList() });
        }

        [HttpGet]
        [Route("GetPurchaseOrderDetailsById")]
        public async Task<IActionResult> GetPurchaseOrderDetailsById(Guid POId)
        {
            var purchaseOrder = await PurchaseOrder.GetPurchaseOrderDetailsById(POId);
            return Ok(new { code = 200, data = purchaseOrder });
        }

        [HttpPost]
        [Route("AddPurchaseOrderDetails")]
        public async Task<IActionResult> AddPurchaseOrderDetails(PurchaseOrderView PurchaseOrderDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var PurchaseOrdermaster = await PurchaseOrder.AddPurchaseOrderDetails(PurchaseOrderDetails);
            if (PurchaseOrdermaster.code == 200)
            {
                response.code = PurchaseOrdermaster.code;
                response.message = PurchaseOrdermaster.message;
            }
            return StatusCode(response.code, response);
        }
        [HttpPost]
        [Route("UpdatePurchaseOrderDetails")]
        public async Task<IActionResult> UpdatePurchaseOrderDetails(PurchaseOrderView PurchaseOrderDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var PurchaseOrdermaster = await PurchaseOrder.UpdatePurchaseOrderDetails(PurchaseOrderDetails);
            if (PurchaseOrdermaster.code == 200)
            {
                response.code = PurchaseOrdermaster.code;
                response.message = PurchaseOrdermaster.message;
            }
            return StatusCode(response.code, response);
        }
        [HttpGet]
        [Route("CheckPONo")]
        public IActionResult CheckPONo()
        {
            var checkPONo = PurchaseOrder.CheckPONo();
            return Ok(new { code = 200, data = checkPONo });
        }

        [HttpPost]
        [Route("InsertMultiplePurchaseOrderDetails")]
        public async Task<IActionResult> InsertMultiplePurchaseOrderDetails(PurchaseOrderMasterView PurchaseOrderDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var PurchaseOrdermaster = await PurchaseOrder.InsertMultiplePurchaseOrderDetails(PurchaseOrderDetails);
            if (PurchaseOrdermaster.code == 200)
            {
                response.code = PurchaseOrdermaster.code;
                response.message = PurchaseOrdermaster.message;
                response.data = PurchaseOrdermaster.data;
            }
            else
            {
                response.code = (int)HttpStatusCode.NotFound;
                response.message = "There Is Some Problem In Your Request!";
            }
            return StatusCode(response.code, response);
        }
        [HttpPost]
        [Route("UpdateMultiplePurchaseOrderDetails")]
        public async Task<IActionResult> UpdateMultiplePurchaseOrderDetails(PurchaseOrderMasterView PurchaseOrderDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var PurchaseOrdermaster = await PurchaseOrder.UpdateMultiplePurchaseOrderDetails(PurchaseOrderDetails);
            if (PurchaseOrdermaster.code == 200)
            {
                response.code = PurchaseOrdermaster.code;
                response.message = PurchaseOrdermaster.message;
            }
            else
            {
                response.code = (int)HttpStatusCode.NotFound;
                response.message = "There Is Some Problem In Your Request!";
            }
            return StatusCode(response.code, response);
        }
        [HttpPost]
        [Route("DeletePurchaseOrderDetails")]
        public async Task<IActionResult> DeletePurchaseOrderDetails(Guid POId)
        {
            ApiResponseModel response = new ApiResponseModel();
            var PO = await PurchaseOrder.DeletePurchaseOrderDetails(POId);
            try
            {
                if (PO != null)
                {
                    response.code = (int)HttpStatusCode.OK;
                    response.message = PO.message;
                }
                else
                {
                    response.message = PO.message;
                    response.code = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                response.code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(response.code, response);
        }    
    }
}
