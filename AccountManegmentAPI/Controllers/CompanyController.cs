using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.CompanyModels;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Services.CompanyService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountManagement.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        public ICompanyService _companyService { get; }

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }
        [HttpPost]
        [Route("AddCompany")]
        public async Task<IActionResult> AddCompany(CompanyModel AddCompany)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var result = _companyService.AddCompany(AddCompany);
                if (result.Result.code == 200)
                {
                    response.code = (int)HttpStatusCode.OK;
                    response.message = result.Result.message;
                }
                else
                {
                    response.message = result.Result.message;
                    response.code = result.Result.code;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return StatusCode(response.code, response);
        }
        [HttpPost]
        [Route("GetAllCompany")]
        public async Task<IActionResult> GetAllCompany(string? searchText, string? searchBy, string? sortBy)
        {
            IEnumerable<CompanyModel> company = await _companyService.GetAllCompany(searchText, searchBy, sortBy);
            return Ok(new { code = 200, data = company.ToList() });
        }
        [HttpGet]
        [Route("GetCompnaytById")]
        public async Task<IActionResult> GetCompnaytById(Guid Id)
        {
            var company = await _companyService.GetCompnaytById(Id);
            return Ok(new { code = 200, data = company });
        }
        [HttpPost]
        [Route("UpdateCompany")]
        public async Task<IActionResult> UpdateCompany(CompanyModel UpdateCompany)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var result = _companyService.UpdateCompany(UpdateCompany);
                if (result.Result.code == 200)
                {
                    response.code = (int)HttpStatusCode.OK;
                    response.message = result.Result.message;
                }
                else
                {
                    response.message = result.Result.message;
                    response.code = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return StatusCode(response.code, response);
        }
        [HttpPost]
        [Route("DeleteCompanyDetails")]
        public async Task<IActionResult> DeleteCompanyDetails(Guid CompanyId)
        {
            ApiResponseModel responseModel = new ApiResponseModel();
            var company = await _companyService.DeleteCompanyDetails(CompanyId);
            try
            {
                if (company != null)
                {
                    responseModel.code = (int)HttpStatusCode.OK;
                    responseModel.message = company.message;
                }
                else
                {
                    responseModel.message = company.message;
                    responseModel.code = company.code;
                }
            }
            catch (Exception ex)
            {
                responseModel.code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(responseModel.code, responseModel);
        }
        [HttpGet]
        [Route("GetCompanyNameList")]
        public async Task<IActionResult> GetCompanyNameList()
        {
            IEnumerable<CompanyModel> company = await _companyService.GetCompanyNameList();
            return Ok(new { code = 200, data = company.ToList() });
        }
    }
}
