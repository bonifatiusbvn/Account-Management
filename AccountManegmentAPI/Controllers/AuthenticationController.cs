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
        public async Task<IActionResult> GetAllUserList(DataTableRequstModel UsersList)
        {
            var userList = await Authentication.GetUsersList(UsersList);
            return Ok(new { code = 200, data = userList });
        }
    }
}
