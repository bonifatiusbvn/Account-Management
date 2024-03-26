using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.DataTableParameters;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

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
                    return new JsonResult(new { Message = "Failed to retrieve user list." });
                }
            }
            catch (Exception ex)
            {

                return new JsonResult(new { Message = $"An error occurred: {ex.Message}" });
            }
        }


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
                    return new JsonResult(new { Message = string.Format(postUser.message), Code = postUser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserDetails(UserViewModel UpdateUser)
        {
            try
            {
                var Updateuser = new UserViewModel()
                {
                    Id = UpdateUser.Id,
                    FirstName = UpdateUser.FirstName,
                    LastName = UpdateUser.LastName,
                    UserName = UpdateUser.UserName,
                    Password = UpdateUser.Password,
                    Email = UpdateUser.Email,
                    PhoneNo = UpdateUser.PhoneNo,
                };
                ApiResponseModel postUser = await APIServices.PostAsync(Updateuser, "Authentication/UpdateUserDetails");
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
        public async Task<IActionResult> UserActiveDecative(Guid UserId)
        {
            try
            {

                ApiResponseModel postuser = await APIServices.PostAsync(null, "Authentication/ActiveDeactiveUsers?UserId=" + UserId);
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
        public async Task<IActionResult> DeleteUserDetails(Guid UserId)
        {
            try
            {
                ApiResponseModel postuser = await APIServices.PostAsync(null, "Authentication/DeleteUserDetails?UserId=" + UserId);
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
