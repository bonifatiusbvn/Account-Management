using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.FormPermissionMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Interfaces.Authentication;
using AccountManagement.Repository.Interface.Repository.FormPermissionMaster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormPermissionMasterController : ControllerBase
    {
        public FormPermissionMasterController(IFormPermissionMaster rolewisePermissionMaster)
        {
            RolewisePermissionMaster = rolewisePermissionMaster;
        }

        public IFormPermissionMaster RolewisePermissionMaster { get; }

        [HttpPost]
        [Route("CreateRolewisePermission")]
        public async Task<IActionResult> CreateRolewiseFormPermission(RolewiseFormPermissionModel formPermission)
        {
            ApiResponseModel response = new ApiResponseModel();
            var formpermission = await RolewisePermissionMaster.CreateRolewiseFormPermission(formPermission);
            if (formpermission.code == 200)
            {
                response.code = formpermission.code;
                response.message = formpermission.message;
            }
            return StatusCode(response.code, response);
        }

        [HttpGet]
        [Route("GetRolewiseFormPermissionById")]
        public async Task<IActionResult> GetRolewiseFormPermissionById(int permissionFormId)
        {
            var rolewiseFormPermission = await RolewisePermissionMaster.GetRolewiseFormPermissionById(permissionFormId);
            return Ok(new { code = 200, data = rolewiseFormPermission });
        }

        [HttpGet]
        [Route("GetRolewiseFormPermissionList")]
        public async Task<IActionResult> GetRolewiseFormPermissionList()
        {
            IEnumerable<RolewiseFormPermissionModel> rolewiseFormPermissionList = await RolewisePermissionMaster.GetRolewiseFormPermissionList();
            return Ok(new { code = 200, data = rolewiseFormPermissionList.ToList() });
        }

        [HttpPost]
        [Route("UpdateRolewiseFormPermission")]
        public async Task<IActionResult> UpdateRolewiseFormPermission(RolewiseFormPermissionModel updateRolewiseFormPermission)
        {
            ApiResponseModel response = new ApiResponseModel();
            var updateRolewiseForm = await RolewisePermissionMaster.UpdateRolewiseFormPermission(updateRolewiseFormPermission);
            if (updateRolewiseForm.code == 200)
            {
                response.code = updateRolewiseForm.code;
                response.message = updateRolewiseForm.message;
            }
            return StatusCode(response.code, response);
        }
    }
}
