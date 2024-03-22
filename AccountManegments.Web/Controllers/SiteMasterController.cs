using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccountManegments.Web.Controllers
{
    public class SiteMasterController : Controller
    {
        public SiteMasterController(WebAPI webAPI, APIServices aPIServices, IWebHostEnvironment environment, UserSession userSession)
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

        public IActionResult SiteListView()
        {
            return View();
        }

        public async Task<IActionResult> SiteListAction(string searchText, string searchBy, string sortBy)
        {
            try
            {

                string apiUrl = $"SiteMaster/GetSiteList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    List<SiteMasterModel> GetSiteList = JsonConvert.DeserializeObject<List<SiteMasterModel>>(res.data.ToString());

                    return PartialView("~/Views/SiteMaster/_SiteListPartial.cshtml", GetSiteList);
                }
                else
                {
                    return new JsonResult(new { Message = "Failed to retrieve Site list." });
                }
            }
            catch (Exception ex)
            {

                return new JsonResult(new { Message = $"An error occurred: {ex.Message}" });
            }
        }

        public async Task<JsonResult> DisplaySiteDetails(Guid SiteId)
        {
            try
            {
                SiteMasterModel SiteDetails = new SiteMasterModel();
                ApiResponseModel res = await APIServices.GetAsync("", "SiteMaster/GetSiteDetailsById?SiteId=" + SiteId);
                if (res.code == 200)
                {
                    SiteDetails = JsonConvert.DeserializeObject<SiteMasterModel>(res.data.ToString());
                }
                return new JsonResult(SiteDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSite(SiteMasterModel createSite)
        {

            try
            {
                var Site = new SiteMasterModel()
                {
                    SiteId = Guid.NewGuid(),
                    SiteName = createSite.SiteName,
                    ContectPersonName = createSite.ContectPersonName,
                    ContectPersonPhoneNo = createSite.ContectPersonPhoneNo,
                    Country = createSite.Country,
                    CityId = createSite.CityId,
                    StateId = createSite.StateId,
                    Area = createSite.Area,
                    Address = createSite.Address,
                    Pincode = createSite.Pincode,
                    ShippingAddress = createSite.ShippingAddress,
                    ShippingArea = createSite.ShippingArea,
                    ShippingCityId = createSite.ShippingCityId,
                    ShippingStateId = createSite.ShippingStateId,
                    ShippingCountry = createSite.ShippingCountry,
                    ShippingPincode  = createSite.ShippingPincode,
                    CreatedBy = UserSession.UserId,
                };

                ApiResponseModel postUser = await APIServices.PostAsync(Site, "SiteMaster/AddSiteDetails");
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
        public async Task<IActionResult> UpdateSiteDetails(SiteMasterModel updateSite)
        {
            try
            {
                ApiResponseModel postUser = await APIServices.PostAsync(updateSite, "SiteMaster/UpdateSiteDetails");
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
        public async Task<IActionResult> ActiveDeactiveSite(Guid SiteId)
        {
            try
            {

                ApiResponseModel postuser = await APIServices.PostAsync(null, "SiteMaster/ActiveDeactiveSite?SiteId=" + SiteId);
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
        public async Task<IActionResult> DeleteSite(Guid SiteId)
        {
            try
            {

                ApiResponseModel postuser = await APIServices.PostAsync(null, "SiteMaster/DeleteSite?SiteId=" + SiteId);
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
    }
}
