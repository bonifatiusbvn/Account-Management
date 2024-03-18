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
