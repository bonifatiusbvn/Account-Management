using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.PurchaseRequest;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.Repository.Interface.Services.PurchaseRequestService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountManagement.API.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class PurchaseRequestController : ControllerBase
    {
        public PurchaseRequestController(IPurchaseRequestService purchaseRequest)
        {
            PurchaseRequest = purchaseRequest;
        }

        public IPurchaseRequestService PurchaseRequest { get; }

        [HttpPost]
        [Route("GetPurchaseRequestList")]
        public async Task<IActionResult> GetPuchaseRequestList(string? searchText, string? searchBy, string? sortBy, Guid? siteId)
        {
            IEnumerable<PurchaseRequestModel> purchaseRequestList = await PurchaseRequest.GetPurchaseRequestList(searchText, searchBy, sortBy, siteId);
            return Ok(new { code = 200, data = purchaseRequestList.ToList() });
        }

        [HttpGet]
        [Route("GetPurchaseRequestDetailsById")]
        public async Task<IActionResult> GetPurchaseRequestDetailsById(Guid purchaseId)
        {
            var purchaseRequestDetails = await PurchaseRequest.GetPurchaseRequestDetailsById(purchaseId);
            return Ok(new { code = 200, data = purchaseRequestDetails });
        }

        [HttpPost]
        [Route("AddPurchaseRequestDetails")]
        public async Task<IActionResult> AddPurchaseRequestDetails(PurchaseRequestModel PuchaseRequestDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var purchaseRequest = await PurchaseRequest.AddPurchaseRequestDetails(PuchaseRequestDetails);
            if (purchaseRequest.code == 200)
            {
                response.code = purchaseRequest.code;
                response.message = purchaseRequest.message;
            }
            else
            {
                response.code = (int)HttpStatusCode.BadRequest;
            }

            return StatusCode(response.code, response);
        }

        [HttpPost]
        [Route("UpdatePurchaseRequestDetails")]
        public async Task<IActionResult> UpdatePurchaseRequestDetails(PurchaseRequestModel purchaseRequestDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var updatepurchaseRequest = await PurchaseRequest.UpdatePurchaseRequestDetails(purchaseRequestDetails);
            if (updatepurchaseRequest.code == 200)
            {
                response.code = updatepurchaseRequest.code;
                response.message = updatepurchaseRequest.message;
            }
            else
            {
                response.code = (int)HttpStatusCode.BadRequest;
            }

            return StatusCode(response.code, response);
        }

        [HttpPost]
        [Route("DeletePurchaseRequest")]
        public async Task<IActionResult> DeletePurchaseRequest(Guid PurchaseId)
        {
            ApiResponseModel responseModel = new ApiResponseModel();

            var purchaseId = await PurchaseRequest.DeletePurchaseRequest(PurchaseId);
            try
            {

                if (purchaseId.code == 200)
                {

                    responseModel.code = (int)HttpStatusCode.OK;
                    responseModel.message = purchaseId.message;
                }
                else
                {
                    responseModel.message = purchaseId.message;
                    responseModel.code = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                responseModel.code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(responseModel.code, responseModel);
        }

        [HttpPost]
        [Route("PurchaseRequestIsApproved")]
        public async Task<IActionResult> PurchaseRequestIsApproved(Guid purchaseId)
        {
            ApiResponseModel response = new ApiResponseModel();
            var purchaseRequest = await PurchaseRequest.PurchaseRequestIsApproved(purchaseId);
            try
            {
                if (purchaseRequest != null)
                {
                    response.code = (int)HttpStatusCode.OK;
                    response.message = purchaseRequest.message;
                }
                else
                {
                    response.message = purchaseRequest.message;
                    response.code = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                response.code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(response.code, response);
        }

        [HttpGet]
        [Route("CheckPRNo")]
        public IActionResult CheckPRNo()
        {
            var checkPRNo = PurchaseRequest.CheckPRNo();
            return Ok(new { code = 200, data = checkPRNo.ToString() });
        }

        [HttpPost]
        [Route("MultiplePurchaseRequestIsApproved")]
        public async Task<IActionResult> MultiplePurchaseRequestIsApproved(PRIsApprovedMasterModel PRIdList)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var purchaseOrder = await PurchaseRequest.MultiplePurchaseRequestIsApproved(PRIdList);
                if (purchaseOrder.code == 200)
                {
                    response.code = purchaseOrder.code;
                    response.message = purchaseOrder.message;
                }
                else
                {
                    response.message = purchaseOrder.message;
                    response.code = purchaseOrder.code;
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
