using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.CompanyModels;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Services.CompanyService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService companyService;

        public CompanyController(ICompanyService companyService)
        {
            this.companyService = companyService;
        }
        [HttpPost]
        [Route("AddCompany")]
        public async Task<IActionResult> AddCompany(CompanyModel AddCompany)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var result = companyService.AddCompany(AddCompany);
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
        [HttpGet]
        [Route("GetAllCompany")]
        public async Task<IActionResult> GetAllCompany()
        {
            IEnumerable<CompanyModel> getExpense = await companyService.GetAllCompany();
            return Ok(new { code = 200, data = getExpense.ToList() });
        }
        [HttpGet]
        [Route("GetCompnaytById")]
        public async Task<IActionResult> GetCompnaytById(Guid Id)
        {
            var company = await companyService.GetCompnaytById(Id);
            return Ok(new { code = 200, data = company });
        }
        [HttpPost]
        [Route("UpdateCompany")]
        public async Task<IActionResult> UpdateCompany(CompanyModel UpdateCompany)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var result = companyService.UpdateCompany(UpdateCompany);
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
    }
}
