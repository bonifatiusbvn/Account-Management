using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.DataTableParameters;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        [HttpPost]
        public async Task<IActionResult> GetUserList()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                var dataTable = new DataTableRequstModel
                {
                    draw = draw,
                    start = start,
                    pageSize = pageSize,
                    skip = skip,
                    lenght = length,
                    searchValue = searchValue,
                    sortColumn = sortColumn,
                    sortColumnDir = sortColumnDir
                };
                List<LoginView> GetUserList = new List<LoginView>();
                var data = new jsonData();
                ApiResponseModel res = await APIServices.PostAsync(dataTable, "Authentication/GetAllUserList");
                if (res.code == 200)
                {
                    data = JsonConvert.DeserializeObject<jsonData>(res.data.ToString());
                    GetUserList = JsonConvert.DeserializeObject<List<LoginView>>(data.data.ToString());
                }
                var jsonData = new
                {
                    draw = data.draw,
                    recordsFiltered = data.recordsFiltered,
                    recordsTotal = data.recordsTotal,
                    data = GetUserList,
                };
                return new JsonResult(jsonData);
            }
            catch (Exception ex)
            {
                throw ex;
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
    }
}
