using AccountManagement.DBContext.Models.DataTableParameters;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Services.AuthenticationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public AuthenticationController(IAuthenticationService authentication)
        {
            Authentication = authentication;
        }

        public IAuthenticationService Authentication { get; }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest login)
        {
            LoginResponseModel loginresponsemodel = new LoginResponseModel();
            try
            {

                var result = await Authentication.LoginUser(login);

                if (result != null && result.Data != null)
                {
                    loginresponsemodel.Code = (int)HttpStatusCode.OK;
                    loginresponsemodel.Data = result.Data;
                    loginresponsemodel.Message = result.Message;
                }
                else
                {
                    loginresponsemodel.Message = result.Message;
                    loginresponsemodel.Code = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                loginresponsemodel.Code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(loginresponsemodel.Code, loginresponsemodel);
        }
        [HttpPost]
        [Route("GetAllUserList")]
        public async Task<IActionResult> GetAllUserList(string? searchText, string? searchBy, string? sortBy)
        {
            IEnumerable<LoginView> userList = await Authentication.GetUsersList(searchText, searchBy, sortBy);
            return Ok(new { code = 200, data = userList.ToList() });
        }

        [HttpGet]
        [Route("GetUserById")]
        public async Task<IActionResult> GetEmployeeById(Guid UserId)
        {
            var userProfile = await Authentication.GetUserById(UserId);
            return Ok(new { code = 200, data = userProfile });
        }

        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> CreateUser(UserViewModel UpdateUser)
        {
            UserResponceModel response = new UserResponceModel();
            var updateUser = await Authentication.CreateUser(UpdateUser);
            if (updateUser.Code == 200)
            {
                response.Code = (int)HttpStatusCode.OK;
                response.Message = updateUser.Message;
                response.Icone = updateUser.Icone;
            }
            return StatusCode(response.Code, response);
        }
        [HttpPost]
        [Route("UpdateUserDetails")]
        public async Task<IActionResult> UpdateUserDetails(UserViewModel UpdateUser)
        {
            UserResponceModel response = new UserResponceModel();
            var updateUser = await Authentication.UpdateUserDetails(UpdateUser);
            if (updateUser.Code == 200)
            {
                response.Code = (int)HttpStatusCode.OK;
                response.Message = updateUser.Message;
                response.Icone = updateUser.Icone;
            }
            return StatusCode(response.Code, response);
        }
    }
}
