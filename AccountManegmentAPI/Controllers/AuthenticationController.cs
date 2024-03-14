using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Services.AuthenticationService;
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

        [HttpPost]
        [Route("AddUser")]
        public async Task<IActionResult> AddUser(UserModel AddEmployee)
        {
            UserResponceModel response = new UserResponceModel();
            try
            {
                var addEmployee = Authentication.UserSingUp(AddEmployee);
                if (addEmployee.Result.Code == 200)
                {
                    response.Code = (int)HttpStatusCode.OK;

                }
                else
                {
                    response.Message = addEmployee.Result.Message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return StatusCode(response.Code, response);
        }
    }
}
