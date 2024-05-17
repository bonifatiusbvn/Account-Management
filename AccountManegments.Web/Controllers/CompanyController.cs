using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.CompanyModels;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccountManegments.Web.Controllers
{
    public class CompanyController : Controller
    {
        public WebAPI WebAPI { get; }
        public APIServices APIServices { get; }
        public IWebHostEnvironment Environment { get; }
        public UserSession _userSession { get; }

        public CompanyController(WebAPI webAPI, APIServices aPIServices, IWebHostEnvironment environment, UserSession userSession)
        {
            WebAPI = webAPI;
            APIServices = aPIServices;
            Environment = environment;
            _userSession = userSession;
        }

        [FormPermissionAttribute("Company-View")]

        public IActionResult CreateCompany()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCompanyDetails(string searchText, string searchBy, string sortBy)
        {
            try
            {
                string apiUrl = $"Company/GetAllCompany?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    List<CompanyModel> GetCompanyList = JsonConvert.DeserializeObject<List<CompanyModel>>(res.data.ToString());

                    return PartialView("~/Views/Company/_CompanyListPartial.cshtml", GetCompanyList);
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

        [FormPermissionAttribute("Company-Add")]
        [HttpPost]
        public async Task<IActionResult> AddCompany(CompanyModel AddCompany)
        {
            try
            {
                var company = new CompanyModel()
                {
                    CompanyId = Guid.NewGuid(),
                    CompanyName = AddCompany.CompanyName,
                    Gstno = AddCompany.Gstno,
                    PanNo = AddCompany.PanNo,
                    Address = AddCompany.Address,
                    Area = AddCompany.Area,
                    CityId = AddCompany.CityId,
                    StateId = AddCompany.StateId,
                    Country = AddCompany.Country,
                    CreatedOn = DateTime.Now,
                    Pincode = AddCompany.Pincode,
                    CreatedBy = _userSession.UserId,
                };

                ApiResponseModel response = await APIServices.PostAsync(company, "Company/AddCompany");
                if (response.code == 200)
                {
                    return Ok(new { Message = response.message, Code = response.code });
                }
                else
                {
                    return BadRequest(new { Message = string.Format(response.message), Code = response.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetCompnaytById(Guid CompanyId)
        {
            try
            {
                CompanyModel company = new CompanyModel();
                ApiResponseModel response = await APIServices.GetAsync("", "Company/GetCompnaytById?Id=" + CompanyId);
                if (response.code == 200)
                {
                    company = JsonConvert.DeserializeObject<CompanyModel>(response.data.ToString());
                }
                return new JsonResult(company);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [FormPermissionAttribute("Company-Edit")]
        [HttpPost]
        public async Task<IActionResult> UpdateCompany(CompanyModel UpdateCompany)
        {
            try
            {
                ApiResponseModel response = await APIServices.PostAsync(UpdateCompany, "Company/UpdateCompany");
                if (response.code == 200)
                {
                    return Ok(new { Message = response.message, Code = response.code });
                }
                else
                {
                    return BadRequest(new { Message = response.message, Code = response.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [FormPermissionAttribute("Company-Delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteCompanyDetails(Guid CompanyId)
        {
            try
            {
                ApiResponseModel company = await APIServices.PostAsync("", "Company/DeleteCompanyDetails?CompanyId=" + CompanyId);
                if (company.code == 200)
                {
                    return Ok(new { Message = string.Format(company.message), Code = company.code });
                }
                else
                {
                    return BadRequest(new { Message = string.Format(company.message), Code = company.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCompanyNameList()
        {
            try
            {
                List<CompanyModel> CompanyName = new List<CompanyModel>();
                ApiResponseModel res = await APIServices.GetAsync("", "Company/GetCompanyNameList");
                if (res.code == 200)
                {
                    CompanyName = JsonConvert.DeserializeObject<List<CompanyModel>>(res.data.ToString());
                }
                return new JsonResult(CompanyName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
