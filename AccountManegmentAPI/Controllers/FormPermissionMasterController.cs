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

        [HttpPost]
        [Route("CreateUserRole")]
        public async Task<IActionResult> CreateUserRole(UserRoleModel roleDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var RoleData = RolewisePermissionMaster.CreateUserRole(roleDetails);
                if (RoleData.Result.code != (int)HttpStatusCode.NotFound && RoleData.Result.code != (int)HttpStatusCode.InternalServerError)
                {
                    response.code = (int)HttpStatusCode.OK;
                    response.message = RoleData.Result.message;
                }
                else
                {
                    response.message = RoleData.Result.message;
                    response.code = RoleData.Result.code;
                }
            }
            catch (Exception ex)
            {
                response.code = (int)HttpStatusCode.InternalServerError;
                response.message = "An error occurred while processing the request.";
            }
            return StatusCode(response.code, response);
        }

        [HttpPost]
        [Route("ActiveDeactiveRole")]
        [Authorize]
        public async Task<IActionResult> ActiveDeactiveUsers(int roleId)
        {
            UserResponceModel responseModel = new UserResponceModel();

            var userName = await RolewisePermissionMaster.ActiveDeactiveRole(roleId);
            try
            {
                if (responseModel.Code == 200)
                {
                    responseModel.Code = userName.Code;
                    responseModel.Message = userName.Message;
                }
                else
                {
                    responseModel.Message = userName.Message;
                    responseModel.Code = userName.Code;
                }
            }
            catch (Exception ex)
            {
                responseModel.Code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(responseModel.Code, responseModel);
        }
        [HttpPost]
        [Route("DeleteRole")]
        [Authorize]
        public async Task<IActionResult> DeleteUserDetails(int roleId)
        {
            UserResponceModel responseModel = new UserResponceModel();

            var User = await RolewisePermissionMaster.DeleteRole(roleId);
            try
            {
                if (responseModel.Code == 200)
                {
                    responseModel.Code = User.Code;
                    responseModel.Message = User.Message;
                }
                else
                {
                    responseModel.Message = User.Message;
                    responseModel.Code = User.Code;
                }
            }
            catch (Exception ex)
            {
                responseModel.Code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(responseModel.Code, responseModel);
        }
    }
}
