using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;

namespace AccountManegments.Web.Controllers
{
    public class AuthenticationController : Controller
    {


        public AuthenticationController(WebAPI webAPI, APIServices aPIServices, IWebHostEnvironment environment)
        {
            WebAPI = webAPI;
            APIServices = aPIServices;
            Environment = environment;
        }
        public WebAPI WebAPI { get; }
        public APIServices APIServices { get; }
        public IWebHostEnvironment Environment { get; }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserLogin(LoginRequest login)
        {
            try
            {


                if (ModelState.IsValid)
                {

                    ApiResponseModel responsemodel = await APIServices.PostAsync(login, "Authentication/Login");
                    LoginResponseModel userlogin = new LoginResponseModel();


                    if (responsemodel.code != (int)HttpStatusCode.OK)
                    {
                        if (responsemodel.code == (int)HttpStatusCode.Forbidden)
                        {
                            TempData["ErrorMessage"] = responsemodel.message;
                            return Ok(new { Message = string.Format(responsemodel.message), Code = responsemodel.code });
                        }
                        else
                        {
                            TempData["ErrorMessage"] = responsemodel.message;
                        }
                    }

                    else
                    {
                        var data = JsonConvert.SerializeObject(responsemodel.data);
                        userlogin.Data = JsonConvert.DeserializeObject<LoginView>(data);
                        var claims = new List<Claim>()
                              {
                                new Claim("UserID", userlogin.Data.Id.ToString()),
                                new Claim("FullName", userlogin.Data.FullName),
                                new Claim("UserName", userlogin.Data.UserName),
                              };




                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                        return RedirectToAction("Index", "Home");
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "InternalServer" });
            }
        }
        public IActionResult UserSingUp()
        {
            return View();
        }
    }
}
