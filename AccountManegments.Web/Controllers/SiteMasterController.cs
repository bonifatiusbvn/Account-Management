using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;

namespace AccountManegments.Web.Controllers
{
    [Authorize]
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

        [FormPermissionAttribute("Site-View")]
        public IActionResult SiteListView()
        {
            return View();
        }

        [FormPermissionAttribute("Site-View")]
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
                    return BadRequest(new { Message = "Failed to retrieve Site list." });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new { Message = $"An error occurred: {ex.Message}" });
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
        [FormPermissionAttribute("Site-Add")]
        [HttpPost]
        public async Task<IActionResult> CreateSite(SiteMasterModel createSite)
        {

            try
            {

                ApiResponseModel postUser = await APIServices.PostAsync(createSite, "SiteMaster/AddSiteDetails");
                if (postUser.code == 200)
                {
                    return Ok(new { Message = postUser.message, Code = postUser.code });
                }
                else
                {
                    return Ok(new { Message = string.Format(postUser.message), Code = postUser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        [FormPermissionAttribute("Site-Edit")]
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
                    return Ok(new { Message = string.Format(postUser.message), Code = postUser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [FormPermissionAttribute("Site-Edit")]
        [HttpPost]
        public async Task<IActionResult> ActiveDeactiveSite(Guid SiteId)
        {
            try
            {

                ApiResponseModel postuser = await APIServices.PostAsync("", "SiteMaster/ActiveDeactiveSite?SiteId=" + SiteId);
                if (postuser.code == 200)
                {
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
                else
                {
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetSiteNameList()
        {
            try
            {
                List<SiteMasterModel> SiteName = new List<SiteMasterModel>();
                ApiResponseModel res = await APIServices.GetAsync("", "SiteMaster/GetSiteNameList");
                if (res.code == 200)
                {
                    SiteName = JsonConvert.DeserializeObject<List<SiteMasterModel>>(res.data.ToString());
                }

                return new JsonResult(SiteName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [FormPermissionAttribute("Site-Delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteSite(Guid SiteId)
        {
            try
            {

                ApiResponseModel postuser = await APIServices.PostAsync("", "SiteMaster/DeleteSite?SiteId=" + SiteId);
                if (postuser.code == 200)
                {
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
                else
                {
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetSiteAddressList(Guid SiteId)
        {
            try
            {
                List<SiteAddressModel> SiteName = new List<SiteAddressModel>();
                ApiResponseModel res = await APIServices.GetAsync("", "SiteMaster/GetSiteAddressList?SiteId=" + SiteId);
                if (res.code == 200)
                {
                    SiteName = JsonConvert.DeserializeObject<List<SiteAddressModel>>(res.data.ToString());
                }

                return new JsonResult(SiteName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        public async Task<IActionResult> DisplaySiteAddressList(Guid SiteId)
        {
            try
            {
                List<SiteAddressModel> SiteName = new List<SiteAddressModel>();
                ApiResponseModel res = await APIServices.GetAsync("", "SiteMaster/GetSiteAddressList?SiteId=" + SiteId);
                if (res.code == 200)
                {
                    SiteName = JsonConvert.DeserializeObject<List<SiteAddressModel>>(res.data.ToString());
                }
                if (SiteName.Count == 0)
                {
                    SiteMasterModel SiteDetails = new SiteMasterModel();
                    ApiResponseModel response = await APIServices.GetAsync("", "SiteMaster/GetSiteDetailsById?SiteId=" + SiteId);
                    if (response.code == 200)
                    {
                        SiteDetails = JsonConvert.DeserializeObject<SiteMasterModel>(response.data.ToString());
                    }
                    return Ok(SiteDetails);
                }
                else
                {
                    return Ok(SiteName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddSiteGroupDetails()
        {
            try
            {
                var GroupList = HttpContext.Request.Form["GroupDetails"];
                var GroupDetails = JsonConvert.DeserializeObject<GroupMasterModel>(GroupList);
                ApiResponseModel postuser = await APIServices.PostAsync(GroupDetails, "SiteMaster/AddSiteGroupDetails");
                if (postuser.code == 200)
                {
                    return Ok(new { postuser.message, postuser.code });
                }
                else
                {
                    return Ok(new { postuser.message, postuser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetGroupNameListBySiteId()
        {
            try
            {
                var SiteId = UserSession.SiteId;
                List<GroupMasterModel> GroupList = new List<GroupMasterModel>();
                ApiResponseModel res = await APIServices.GetAsync("", "SiteMaster/GetGroupNameListBySiteId?SiteId=" + SiteId);
                if (res.code == 200)
                {
                    GroupList = JsonConvert.DeserializeObject<List<GroupMasterModel>>(res.data.ToString());
                }
                return new JsonResult(GroupList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> GetGroupNameList()
        {
            try
            {
                ApiResponseModel res = await APIServices.PostAsync("", "SiteMaster/GetGroupNameList");

                if (res.code == 200)
                {
                    List<SiteGroupModel> GetSiteList = JsonConvert.DeserializeObject<List<SiteGroupModel>>(res.data.ToString());

                    return PartialView("~/Views/SiteMaster/_SiteGroupMasterListPartial.cshtml", GetSiteList);
                }
                else
                {
                    return Ok(new { Message = "Failed to retrieve Site list.", StatusCode = 500 });
                }
            }
            catch (Exception ex)
            {

                return Ok(new { Message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSiteGroupDetails(Guid GroupId)
        {
            try
            {
                ApiResponseModel postuser = await APIServices.PostAsync("", "SiteMaster/DeleteSiteGroupDetails?GroupId=" + GroupId);
                if (postuser.code == 200)
                {
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
                else
                {
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetGroupDetailsByGroupName(Guid GroupId)
        {
            try
            {
                GroupMasterModel SiteDetails = new GroupMasterModel();
                ApiResponseModel res = await APIServices.GetAsync("", "SiteMaster/GetGroupDetailsByGroupName?GroupId=" + GroupId);
                if (res.code == 200)
                {
                    SiteDetails = JsonConvert.DeserializeObject<GroupMasterModel>(res.data.ToString());
                }
                return new JsonResult(SiteDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSiteGroupMaster()
        {
            try
            {
                var GroupList = HttpContext.Request.Form["GroupDetails"];
                var GroupDetails = JsonConvert.DeserializeObject<GroupMasterModel>(GroupList);
                ApiResponseModel postUser = await APIServices.PostAsync(GroupDetails, "SiteMaster/UpdateSiteGroupMaster");
                if (postUser.code == 200)
                {
                    return Ok(new { Message = postUser.message, Code = postUser.code });
                }
                else
                {
                    return Ok(new { Message = string.Format(postUser.message), Code = postUser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [FormPermissionAttribute("Group-View")]
        public IActionResult CreateGroup()
        {
            return View();
        }
    }
}
