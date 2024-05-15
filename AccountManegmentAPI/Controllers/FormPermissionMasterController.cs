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
        [Route("CreateRolewisePermission")]
        [Authorize]

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
        [Authorize]

        public async Task<IActionResult> GetRolewiseFormPermissionById(int permissionFormId)
        {
            var rolewiseFormPermission = await RolewisePermissionMaster.GetRolewiseFormPermissionById(permissionFormId);
            return Ok(new { code = 200, data = rolewiseFormPermission });
        }

        [HttpGet]
        [Route("GetRolewiseFormPermissionList")]
        [Authorize]

        public async Task<IActionResult> GetRolewiseFormPermissionList()
        {
            IEnumerable<RolewiseFormPermissionModel> rolewiseFormPermissionList = await RolewisePermissionMaster.GetRolewiseFormPermissionList();
            return Ok(new { code = 200, data = rolewiseFormPermissionList.ToList() });
        }

        [HttpPost]
        [Route("UpdateRolewiseFormPermission")]
        [Authorize]

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

        [HttpPost]
        [Route("GetFormGroupList")]
        [Authorize]

        public async Task<IActionResult> GetFormGroupList()
        {
            IEnumerable<FormMasterModel> formList = await FormMaster.GetFormGroupList();
            return Ok(new { code = 200, data = formList.ToList() });
        }

        [HttpPost]
        [Route("InsertMultipleRolewiseFormPermission")]
        [Authorize]

        public async Task<IActionResult> InsertMultipleRolewiseFormPermission(List<RolewiseFormPermissionModel> RolewiseFormPermission)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var rolewiseFormPermission = RolewisePermissionMaster.InsertMultipleRolewiseFormPermission(RolewiseFormPermission);
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

        [HttpPost]
        [Route("GetRolewiseFormListById")]
        [Authorize]

        public async Task<IActionResult> GetRolewiseFormListById(int RoleId)
        {
            ApiResponseModel response = new ApiResponseModel();
            List<RolewiseFormPermissionModel> RolewiseFormList = await RolewisePermissionMaster.GetRolewiseFormListById(RoleId);

            if (RolewiseFormList.Count == 0)
            {
                response.code = 400;
            }
            else
            {
                response.code = 200;
                response.data = RolewiseFormList.ToList();
            }
            return StatusCode(response.code, response);
        }


        [HttpPost]
        [Route("UpdateMultipleRolewiseFormPermission")]
        [Authorize]

        public async Task<IActionResult> UpdateMultipleRolewiseFormPermission(List<RolewiseFormPermissionModel> RolewiseFormPermission)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var rolewiseFormPermission = RolewisePermissionMaster.UpdateMultipleRolewiseFormPermission(RolewiseFormPermission);
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
