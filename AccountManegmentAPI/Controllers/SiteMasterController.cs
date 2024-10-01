using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Interfaces.Authentication;
using AccountManagement.Repository.Interface.Services.SiteMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.RegularExpressions;

namespace AccountManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class SiteMasterController : ControllerBase
    {
        public SiteMasterController(ISiteMasterServices siteMaster)
        {
            SiteMaster = siteMaster;
        }
        public ISiteMasterServices SiteMaster { get; }

        [HttpPost]
        [Route("GetSiteList")]
        public async Task<IActionResult> GetSiteList(string? searchText, string? searchBy, string? sortBy)
        {
            IEnumerable<SiteMasterModel> SiteList = await SiteMaster.GetSiteList(searchText, searchBy, sortBy);
            return Ok(new { code = 200, data = SiteList.ToList() });
        }

        [HttpGet]
        [Route("GetSiteDetailsById")]
        public async Task<IActionResult> GetSiteDetailsById(Guid SiteId)
        {
            var SiteDetails = await SiteMaster.GetSiteDetailsById(SiteId);
            return Ok(new { code = 200, data = SiteDetails });
        }

        [HttpPost]
        [Route("AddSiteDetails")]
        public async Task<IActionResult> AddSiteDetails(SiteMasterModel SiteDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var sitemaster = await SiteMaster.AddSiteDetails(SiteDetails);
            if (sitemaster.code == 200)
            {
                response.code = sitemaster.code;
                response.message = sitemaster.message;
            }
            else
            {
                response.code = sitemaster.code;
                response.message = sitemaster.message;
            }
            return StatusCode(response.code, response);
        }
        [HttpPost]
        [Route("UpdateSiteDetails")]
        public async Task<IActionResult> UpdateSiteDetails(SiteMasterModel SiteDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var sitemaster = await SiteMaster.UpdateSiteDetails(SiteDetails);
            if (sitemaster.code == 200)
            {
                response.code = sitemaster.code;
                response.message = sitemaster.message;
            }
            else
            {
                response.code = (int)HttpStatusCode.BadRequest;
            }
            return StatusCode(response.code, response);
        }

        [HttpPost]
        [Route("ActiveDeactiveSite")]
        public async Task<IActionResult> ActiveDeactiveSite(Guid SiteId)
        {
            ApiResponseModel responseModel = new ApiResponseModel();

            var siteName = await SiteMaster.ActiveDeactiveSite(SiteId);
            try
            {

                if (responseModel.code == 200)
                {

                    responseModel.code = siteName.code;
                    responseModel.message = siteName.message;
                }
                else
                {
                    responseModel.message = siteName.message;
                    responseModel.code = siteName.code;
                }
            }
            catch (Exception ex)
            {
                responseModel.code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(responseModel.code, responseModel);
        }

        [HttpPost]
        [Route("DeleteSite")]
        public async Task<IActionResult> DeleteSite(Guid SiteId)
        {
            ApiResponseModel responseModel = new ApiResponseModel();

            var siteId = await SiteMaster.DeleteSite(SiteId);
            try
            {

                if (responseModel.code == 200)
                {
                    responseModel.code = siteId.code;
                    responseModel.message = siteId.message;
                }
                else
                {
                    responseModel.message = siteId.message;
                    responseModel.code = siteId.code;
                }
            }
            catch (Exception ex)
            {
                responseModel.code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(responseModel.code, responseModel);
        }
        [HttpGet]
        [Route("GetSiteNameList")]
        public async Task<IActionResult> GetSiteNameList()
        {
            IEnumerable<SiteMasterModel> SiteName = await SiteMaster.GetSiteNameList();
            return Ok(new { code = 200, data = SiteName.ToList() });
        }

        [HttpGet]
        [Route("GetSiteAddressList")]
        public async Task<IActionResult> GetSiteAddressList(Guid SiteId)
        {
            IEnumerable<SiteAddressModel> SiteName = await SiteMaster.GetSiteAddressList(SiteId);
            return Ok(new { code = 200, data = SiteName.ToList() });
        }

        [HttpPost]
        [Route("AddSiteGroupDetails")]
        public async Task<IActionResult> AddSiteGroupDetails(SiteGroupModel GroupDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var GroupDetail = await SiteMaster.AddSiteGroupDetails(GroupDetails);
            if (GroupDetail.code == 200)
            {
                response.code = GroupDetail.code;
                response.message = GroupDetail.message;
            }
            else
            {
                response.code = GroupDetail.code;
                response.message = GroupDetail.message;
            }
            return StatusCode(response.code, response);
        }
        [HttpGet]
        [Route("GetGroupNameListBySiteId")]
        public async Task<IActionResult> GetGroupNameListBySiteId(Guid SiteId)
        {
            IEnumerable<GroupMasterModel> GroupList = await SiteMaster.GetGroupNameListBySiteId(SiteId);
            return Ok(new { code = 200, data = GroupList.ToList() });
        }

        [HttpPost]
        [Route("GetGroupNameList")]
        public async Task<IActionResult> GetGroupNameList()
        {
            IEnumerable<SiteGroupModel> GroupList = await SiteMaster.GetGroupNameList();
            return Ok(new { code = 200, data = GroupList.ToList() });
        }

        [HttpPost]
        [Route("DeleteSiteGroupDetails")]
        public async Task<IActionResult> DeleteSiteGroupDetails(Guid GroupId)
        {
            ApiResponseModel responseModel = new ApiResponseModel();

            var sitegroupName = await SiteMaster.DeleteSiteGroupDetails(GroupId);
            try
            {

                if (responseModel.code == 200)
                {
                    responseModel.code = sitegroupName.code;
                    responseModel.message = sitegroupName.message;
                }
                else
                {
                    responseModel.message = sitegroupName.message;
                    responseModel.code = sitegroupName.code;
                }
            }
            catch (Exception ex)
            {
                responseModel.code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(responseModel.code, responseModel);
        }

        [HttpGet]
        [Route("GetGroupDetailsByGroupId")]
        public async Task<IActionResult> GetGroupDetailsByGroupId(Guid GroupId)
        {
            var SiteGroupDetails = await SiteMaster.GetGroupDetailsByGroupId(GroupId);
            return Ok(new { code = 200, data = SiteGroupDetails });
        }

        [HttpPost]
        [Route("UpdateSiteGroupMaster")]
        public async Task<IActionResult> UpdateSiteGroupMaster(SiteGroupModel groupDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var sitemaster = await SiteMaster.UpdateSiteGroupMaster(groupDetails);
                if (sitemaster.code == 200)
                {
                    response.code = sitemaster.code;
                    response.message = sitemaster.message;
                }
                else
                {
                    response.code = (int)HttpStatusCode.BadRequest;
                    response.message = sitemaster.message;
                }

            }
            catch (Exception ex)
            {
                response.code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(response.code, response);
        }

        [HttpGet]
        [Route("GetGroupDetailsByGroupName")]
        public async Task<IActionResult> GetGroupDetailsByGroupName(string GroupName)
        {
            var SiteGroupDetails = await SiteMaster.GetGroupDetailsByGroupName(GroupName);
            if (SiteGroupDetails != null)
            {
                return Ok(new { code = 200, data = SiteGroupDetails });
            }
            else
            {
                return BadRequest(new { code = 400, message = "Group not found" });
            }
        }
    }
}
