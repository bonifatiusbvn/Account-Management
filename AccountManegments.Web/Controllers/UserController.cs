using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.DataTableParameters;
using AccountManagement.DBContext.Models.ViewModels;
using AccountManagement.DBContext.Models.ViewModels.FormMaster;
using AccountManagement.DBContext.Models.ViewModels.FormPermissionMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;

namespace AccountManegments.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public WebAPI WebAPI { get; }
        public APIServices APIServices { get; }
        public IWebHostEnvironment Environment { get; }
        public UserSession _userSession { get; }

        public UserController(WebAPI webAPI, APIServices aPIServices, IWebHostEnvironment environment, UserSession userSession)
        {
            WebAPI = webAPI;
            APIServices = aPIServices;
            Environment = environment;
            _userSession = userSession;
        }

        [FormPermissionAttribute("User List-View")]
        public IActionResult UserListView()
        {
            return View();
        }

        public async Task<IActionResult> UserListAction(string searchText, string searchBy, string sortBy)
        {
            try
            {
                string apiUrl = $"Authentication/GetAllUserList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    List<LoginView> GetUserList = JsonConvert.DeserializeObject<List<LoginView>>(res.data.ToString());

                    return PartialView("~/Views/User/_UserListPartial.cshtml", GetUserList);
                }
                else
                {
                    return BadRequest(new { Message = "Failed to retrieve user list." });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new { Message = $"An error occurred: {ex.Message}" });
            }
        }

        [FormPermissionAttribute("User List-View")]
        public async Task<JsonResult> DisplayUserDetails(Guid UserId)
        {
            try
            {
                LoginView UserDetails = new LoginView();
                ApiResponseModel res = await APIServices.GetAsync("", "Authentication/GetUserById?UserId=" + UserId);
                if (res.code == 200)
                {
                    UserDetails = JsonConvert.DeserializeObject<LoginView>(res.data.ToString());
                }
                return new JsonResult(UserDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [FormPermissionAttribute("User List-Add")]
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserViewModel CreatUser)
        {

            try
            {
                var user = new UserViewModel()
                {
                    Id = Guid.NewGuid(),
                    FirstName = CreatUser.FirstName,
                    LastName = CreatUser.LastName,
                    UserName = CreatUser.UserName,
                    Email = CreatUser.Email,
                    Role = CreatUser.Role,
                    Password = CreatUser.Password,
                    PhoneNo = CreatUser.PhoneNo,
                    SiteId = CreatUser.SiteId,
                    CreatedBy = _userSession.UserId,
                };

                ApiResponseModel postUser = await APIServices.PostAsync(user, "Authentication/CreateUser");
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

        [FormPermissionAttribute("User List-Edit")]
        [HttpPost]
        public async Task<IActionResult> UpdateUserDetails(UserViewModel UpdateUser)
        {
            try
            {

                ApiResponseModel postUser = await APIServices.PostAsync(UpdateUser, "Authentication/UpdateUserDetails");
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

        [FormPermissionAttribute("User List-Edit")]
        [HttpPost]
        public async Task<IActionResult> UserActiveDecative(Guid UserId)
        {
            try
            {

                ApiResponseModel postuser = await APIServices.PostAsync("", "Authentication/ActiveDeactiveUsers?UserId=" + UserId);
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
        [FormPermissionAttribute("User List-Delete")]
        [HttpGet]
        public async Task<IActionResult> DeleteUserDetails(Guid UserId)
        {
            try
            {
                ApiResponseModel postuser = await APIServices.PostAsync("", "Authentication/DeleteUserDetails?UserId=" + UserId);
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

        public async Task<JsonResult> GetRoles()
        {

            try
            {
                List<UserRoleModel> Userrole = new List<UserRoleModel>();
                ApiResponseModel response = await APIServices.GetAsyncId(null, "MasterList/GetUserRole");
                if (response.code == 200)
                {
                    Userrole = JsonConvert.DeserializeObject<List<UserRoleModel>>(response.data.ToString());
                }
                return new JsonResult(Userrole);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [FormPermissionAttribute("User Permission-View")]
        [HttpGet]
        public IActionResult RolewisePermission()
        {
            return View();
        }

        [FormPermissionAttribute("User Permission-Edit")]
        public async Task<IActionResult> RolewisePermissionListAction()
        {
            try
            {
                ApiResponseModel res = await APIServices.PostAsync("", "MasterList/GetUserRoleList");

                if (res.code == 200)
                {
                    List<UserRoleModel> GetUserRoleList = JsonConvert.DeserializeObject<List<UserRoleModel>>(res.data.ToString());

                    return PartialView("~/Views/User/_RolewisePermissionPartial.cshtml", GetUserRoleList);
                }
                else
                {
                    return BadRequest(new { Message = "Failed to retrieve user role list." });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new { Message = $"An error occurred: {ex.Message}" });
            }
        }

        [FormPermissionAttribute("User Permission-Edit")]
        [HttpPost]
        public async Task<IActionResult> InsertMultipleRolewiseFormPermission()
        {
            try
            {
                var rolewisePermissionDetails = HttpContext.Request.Form["RolewisePermissionDetails"];
                var InsertDetails = JsonConvert.DeserializeObject<List<RolewiseFormPermissionModel>>(rolewisePermissionDetails.ToString());

                ApiResponseModel postuser = await APIServices.PostAsync(InsertDetails, "FormPermissionMaster/InsertMultipleRolewiseFormPermission");
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

        public async Task<IActionResult> GetRolewiseFormListById(int RoleId)
        {
            try
            {
                List<RolewiseFormPermissionModel> RolewiseFormList = new List<RolewiseFormPermissionModel>();
                ApiResponseModel response = await APIServices.PostAsync("", "FormPermissionMaster/GetRolewiseFormListById?RoleId=" + RoleId);
                if (response.code == 200)
                {
                    RolewiseFormList = JsonConvert.DeserializeObject<List<RolewiseFormPermissionModel>>(response.data.ToString());
                    return PartialView("~/Views/User/_editRolewiseFormPartial.cshtml", RolewiseFormList);
                }
                else
                {
                    return BadRequest(new { response.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [FormPermissionAttribute("User Permission-Edit")]
        [HttpPost]
        public async Task<IActionResult> UpdateMultipleRolewiseFormPermission()
        {
            try
            {
                var rolewisePermissionDetails = HttpContext.Request.Form["RolewisePermissionDetails"];
                var UpdateDetails = JsonConvert.DeserializeObject<List<RolewiseFormPermissionModel>>(rolewisePermissionDetails.ToString());

                ApiResponseModel postuser = await APIServices.PostAsync(UpdateDetails, "FormPermissionMaster/UpdateMultipleRolewiseFormPermission");
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

        [FormPermissionAttribute("User Permission-Add")]
        [HttpPost]
        public async Task<IActionResult> CreateUserRole(UserRoleModel roleDetails)
        {
            try
            {
                ApiResponseModel postuser = await APIServices.PostAsync(roleDetails, "FormPermissionMaster/CreateUserRole");
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

        [FormPermissionAttribute("User Permission-Edit")]
        [HttpPost]
        public async Task<IActionResult> RoleActiveDecative(int roleId)
        {
            try
            {

                ApiResponseModel postuser = await APIServices.PostAsync("", "FormPermissionMaster/ActiveDeactiveRole?RoleId=" + roleId);
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

        [FormPermissionAttribute("User List-Delete")]
        [HttpGet]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            try
            {
                ApiResponseModel postuser = await APIServices.PostAsync("", "FormPermissionMaster/DeleteRole?RoleId=" + roleId);
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
    }
}
