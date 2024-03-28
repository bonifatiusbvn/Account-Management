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
using AccountManagement.DBContext.Models.DataTableParameters;
using AccountManagement.DBContext.Models.ViewModels;
using AccountManagement.API;
using AccountManagement.DBContext.Models.ViewModels.FormMaster;
using AccountManagement.DBContext.Models.ViewModels.FormPermissionMaster;

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
        public UserSession UserSession { get; }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<JsonResult> GetCountrys()
        {

            try
            {
                List<CountryView> countries = new List<CountryView>();
                ApiResponseModel response = await APIServices.GetAsyncId(null, "MasterList/GetCountries");
                if (response.code == 200)
                {
                    countries = JsonConvert.DeserializeObject<List<CountryView>>(response.data.ToString());
                }
                return new JsonResult(countries);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> GetState(int StateId)
        {

            try
            {
                List<StateView> states = new List<StateView>();
                ApiResponseModel response = await APIServices.GetAsyncId(null, "MasterList/GetState?StateId=" + StateId);
                if (response.code == 200)
                {
                    states = JsonConvert.DeserializeObject<List<StateView>>(response.data.ToString());
                }
                return new JsonResult(states);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> GetCity(int CityId)
        {

            try
            {
                List<CityView> cities = new List<CityView>();
                ApiResponseModel response = await APIServices.GetAsyncId(null, "MasterList/GetCities?CityId=" + CityId);
                if (response.code == 200)
                {
                    cities = JsonConvert.DeserializeObject<List<CityView>>(response.data.ToString());
                }
                return new JsonResult(cities);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public IActionResult UserLogin()
        {

            if (Request.Cookies["UserName"] != null && Request.Cookies["Password"] != null)
            {
                ViewBag.UserName = (Request.Cookies["UserName"].ToString());
                var pwd = Request.Cookies["Password"].ToString();
                ViewBag.Password = pwd;
                ViewBag.chkRememberMe = true;

            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UserLogin(LoginRequest login)
        {
            try
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
                new Claim("UserId", userlogin.Data.Id.ToString()),
                new Claim("FullName", userlogin.Data.FullName),
                new Claim("UserName", userlogin.Data.UserName),
                new Claim("SiteName", userlogin.Data.SiteName),
                new Claim("UserRole", userlogin.Data.RoleId.ToString()),
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    if (login.RememberMe)
                    {
                        CookieOptions cookie = new CookieOptions();
                        cookie.Expires = DateTime.UtcNow.AddDays(7);
                        Response.Cookies.Append("UserName", (login.UserName), cookie);
                        Response.Cookies.Append("Password", (login.Password), cookie);
                        ViewBag.chkRememberMe = true;


                    }
                    else
                    {
                        Response.Cookies.Delete("UserName");
                        Response.Cookies.Delete("Password");
                        ViewBag.chkRememberMe = false;
                    }

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                    return RedirectToAction("Index", "Home");
                }
                return View();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "InternalServer" });
            }
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                HttpContext.Session.Clear();
                await HttpContext.SignOutAsync();

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return RedirectToAction("UserLogin");
        }
    }
}
