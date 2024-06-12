using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.CompanyModels;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Services.CompanyService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
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

        [Authorize]
        [HttpPost("AddCompany")]
        public async Task<IActionResult> AddCompany(CompanyModel AddCompany)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var result = _companyService.AddCompany(AddCompany);
                if (result.Result.code == 200)
                {
                    response.code = result.Result.code;
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
        [Authorize]
        [HttpPost("GetAllCompany")]
        public async Task<IActionResult> GetAllCompany(string? searchText, string? searchBy, string? sortBy)
        {
            IEnumerable<CompanyModel> company = await _companyService.GetAllCompany(searchText, searchBy, sortBy);
            return Ok(new { code = 200, data = company.ToList() });
        }
        [Authorize]
        [HttpGet("GetCompnaytById")]
        public async Task<IActionResult> GetCompnaytById(Guid Id)
        {
            var company = await _companyService.GetCompnaytById(Id);
            return Ok(new { code = 200, data = company });
        }
        [Authorize]
        [HttpPost("UpdateCompany")]
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
        [Authorize]
        [HttpPost("DeleteCompanyDetails")]
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
        [HttpGet("GetCompanyNameList")]
        [Authorize]
        public async Task<IActionResult> GetCompanyNameList()
        {
            var _bearerToken = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(_bearerToken))
            {
                IEnumerable<CompanyModel> company = await _companyService.GetCompanyNameList();
                return Ok(new { code = 200, data = company.ToList() });
            }
            else
            {
                return BadRequest(new { Code = (int)HttpStatusCode.InternalServerError });
            }
        }
    }
}
