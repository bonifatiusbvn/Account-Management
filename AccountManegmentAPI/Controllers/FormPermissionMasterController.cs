using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.FormMaster;
using AccountManagement.DBContext.Models.ViewModels.FormPermissionMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Interfaces.Authentication;
using AccountManagement.Repository.Interface.Repository.FormPermissionMaster;
using AccountManagement.Repository.Interface.Repository.MasterList;
using AccountManagement.Repository.Interface.Repository.PurchaseOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormPermissionMasterController : ControllerBase
    {
        public FormPermissionMasterController(IFormPermissionMaster rolewisePermissionMaster, IFormMaster formMaster)
        {
            RolewisePermissionMaster = rolewisePermissionMaster;
            FormMaster = formMaster;
        }

        public IFormPermissionMaster RolewisePermissionMaster { get; }
        public IFormMaster FormMaster { get; }


        [HttpPost]
        [Route("GetFormGroupList")]
        [Authorize]

        public async Task<IActionResult> GetFormGroupList()
        {
            IEnumerable<FormMasterModel> formList = await FormMaster.GetFormGroupList();
            return Ok(new { code = 200, data = formList.ToList() });
        }


        [HttpPost]
        [Route("GetUserwiseFormPermissionById")]

        public async Task<IActionResult> GetUserwiseFormPermissionById(Guid UserId)
        {
            var rolewiseFormPermission = await RolewisePermissionMaster.GetUserwiseFormPermissionById(UserId);
            return Ok(new { code = 200, data = rolewiseFormPermission });
        }

        [HttpPost]
        [Route("UpdateMultipleUserewiseFormPermission")]

        public async Task<IActionResult> UpdateMultipleUserewiseFormPermission(List<UserwiseFormPermissionModel> UserwiseFormPermission)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var rolewiseFormPermission = RolewisePermissionMaster.UpdateMultipleUserwiseFormPermission(UserwiseFormPermission);
                if (rolewiseFormPermission.Result.code == 200)
                {
                    response.code = (int)HttpStatusCode.OK;
                    response.message = rolewiseFormPermission.Result.message;
                }
                else
                {
                    response.message = rolewiseFormPermission.Result.message;
                    response.code = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return StatusCode(response.code, response);
        }
    }
}
