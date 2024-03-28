using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.Repository.Interface.Services.PurchaseOrderService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        [Route("GetPurchaseOrderList")]
        public async Task<IActionResult> GetPurchaseOrderList()
        {
            IEnumerable<PurchaseOrderView> PurchaseOrderList = await PurchaseOrder.GetPurchaseOrderList();
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
    }
}
