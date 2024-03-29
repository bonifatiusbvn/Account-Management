using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.DBContext.Models.ViewModels.PurchaseRequest;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccountManegments.Web.Controllers
{
    public class PurchaseMasterController : Controller
    {
        public PurchaseMasterController(WebAPI webAPI, APIServices aPIServices, IWebHostEnvironment environment, UserSession userSession)
        {
            WebAPI = webAPI;
            APIServices = aPIServices;
            Environment = environment;
            UserSession = userSession;
        }

        public WebAPI WebAPI { get; }
        public APIServices APIServices { get; }
        public IWebHostEnvironment Environment { get; }
        public UserSession UserSession { get; }

        public async Task<IActionResult> PurchaseRequestListView()
        {
            try
            {
                ApiResponseModel Response = await APIServices.GetAsync("", "PurchaseRequest/CheckPRNo");
                if (Response.code == 200)
                {
                    ViewBag.ProjectRequestNo = Response.data;
                }
                return View();
            }
            catch (Exception ex)
            {
                throw ex;

            }
            return View();
        }

        public async Task<IActionResult> PurchaseRequestListAction(string searchText, string searchBy, string sortBy)
        {
            try
            {

                string apiUrl = $"PurchaseRequest/GetPurchaseRequestList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    List<PurchaseRequestModel> GetSiteList = JsonConvert.DeserializeObject<List<PurchaseRequestModel>>(res.data.ToString());

                    return PartialView("~/Views/PurchaseMaster/_PurchaseRequestListPartial.cshtml", GetSiteList);
                }
                else
                {
                    return new JsonResult(new { Message = "Failed to retrieve Purchase Request list." });
                }
            }
            catch (Exception ex)
            {

                return new JsonResult(new { Message = $"An error occurred: {ex.Message}" });
            }
        }
        public async Task<JsonResult> DisplayPurchaseRequestDetails(Guid PurchaseId)
        {
            try
            {
                PurchaseRequestModel SiteDetails = new PurchaseRequestModel();
                ApiResponseModel res = await APIServices.GetAsync("", "PurchaseRequest/GetPurchaseRequestDetailsById?purchaseId=" + PurchaseId);
                if (res.code == 200)
                {
                    SiteDetails = JsonConvert.DeserializeObject<PurchaseRequestModel>(res.data.ToString());
                }
                return new JsonResult(SiteDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurchaseRequest(PurchaseRequestModel PurchaseRequestDetails)
        {

            try
            {
                var PurchaseRequest = new PurchaseRequestModel()
                {
                    Pid = Guid.NewGuid(),
                    Item = PurchaseRequestDetails.Item,
                    UnitTypeId = PurchaseRequestDetails.UnitTypeId,
                    SiteId = PurchaseRequestDetails.SiteId,
                    Quantity = PurchaseRequestDetails.Quantity,
                    CreatedBy = UserSession.UserId,
                    PrNo = PurchaseRequestDetails.PrNo,
                    IsApproved = false,
                };

                ApiResponseModel postUser = await APIServices.PostAsync(PurchaseRequest, "PurchaseRequest/AddPurchaseRequestDetails");
                if (postUser.code == 200)
                {
                    return Ok(new { Message = postUser.message, Code = postUser.code });
                }
                else
                {
                    return new JsonResult(new { Message = string.Format(postUser.message), Code = postUser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePurchaseRequestDetails(PurchaseRequestModel updatePurchaseRequest)
        {
            try
            {
                ApiResponseModel postUser = await APIServices.PostAsync(updatePurchaseRequest, "PurchaseRequest/UpdatePurchaseRequestDetails");
                if (postUser.code == 200)
                {
                    return Ok(new { Message = postUser.message, Code = postUser.code });
                }
                else
                {
                    return new JsonResult(new { Message = string.Format(postUser.message), Code = postUser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePurchaseRequest(Guid PurchaseId)
        {
            try
            {
                ApiResponseModel postuser = await APIServices.PostAsync(null, "PurchaseRequest/DeletePurchaseRequest?PurchaseId=" + PurchaseId);
                if (postuser.code == 200)
                {
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
                else
                {
                    return new JsonResult(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> PurchaseRequestIsApproved(Guid PurchaseId)
        {
            try
            {

                ApiResponseModel postuser = await APIServices.PostAsync(null, "PurchaseRequest/PurchaseRequestIsApproved?PurchaseId=" + PurchaseId);
                if (postuser.code == 200)
                {

                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });

                }
                else
                {
                    return new JsonResult(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> CreatePurchaseOrder()
        {
            try
            {
                ApiResponseModel Response = await APIServices.GetAsync("", "PurchaseOrder/CheckPONo");
                if (Response.code == 200)
                {
                    ViewBag.PurchaseOrderNo = Response.data;
                }
                return View();
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertMultiplePurchaseOrderDetails()
        {
            try
            {
                var OrderDetails = HttpContext.Request.Form["PODETAILS"];
                var InsertDetails = JsonConvert.DeserializeObject<List<PurchaseOrderMasterView>>(OrderDetails.ToString());
                ApiResponseModel postuser = await APIServices.PostAsync(InsertDetails, "PurchaseOrder/InsertMultiplePurchaseOrderDetails");
                if (postuser.code == 200)
                {
                    return Ok(new { postuser.message });
                }
                else
                {
                    return Ok(new { postuser.message });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
