using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Interfaces.Authentication;
using AccountManagement.Repository.Interface.Services.SiteMaster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SiteMasterController : ControllerBase
    {
        public SiteMasterController(ISiteMasterServices siteMaster)
        {
            SiteMaster = siteMaster;
        }
        public ISiteMasterServices SiteMaster { get; }

        [HttpGet]
        [Route("GetSiteList")]
        public async Task<IActionResult> GetSiteList()
        {
            IEnumerable<SiteMasterModel> SiteList = await SiteMaster.GetSiteList();
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
            return StatusCode(response.code, response);
        }
    }
}
