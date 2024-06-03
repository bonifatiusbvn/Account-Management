using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.SupplierMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Interfaces.Authentication;
using AccountManagement.Repository.Interface.Services.SupplierService;
using AccountManagement.Repository.Services.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountManagement.API.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class SupplierMasterController : ControllerBase
    {
        public SupplierMasterController(ISupplierServices supplier)
        {
            _Supplier = supplier;
        }

        public ISupplierServices _Supplier { get; }

        [HttpPost]
        [Route("CreateSupplier")]
        public async Task<IActionResult> CreateSupplier(SupplierModel Supplier)
        {
            ApiResponseModel response = new ApiResponseModel();
            var CreateSupplier = await _Supplier.CreateSupplier(Supplier);
            if (CreateSupplier.code == 200)
            {
                response.code = (int)HttpStatusCode.OK;
                response.message = CreateSupplier.message;

            }
            else
            {
                response.code = (int)HttpStatusCode.BadRequest;
            }

            return StatusCode(response.code, response);
        }
        [HttpPost]
        [Route("GetAllSupplierList")]
        public async Task<IActionResult> GetAllUserList(string? searchText, string? searchBy, string? sortBy)
        {
            IEnumerable<SupplierModel> userList = await _Supplier.GetSupplierList(searchText, searchBy, sortBy);
            return Ok(new { code = 200, data = userList.ToList() });
        }

        [HttpGet]
        [Route("GetSupplierById")]
        public async Task<IActionResult> GetEmployeeById(Guid SupplierId)
        {
            var SupplierProfile = await _Supplier.GetSupplierById(SupplierId);
            return Ok(new { code = 200, data = SupplierProfile });
        }

        [HttpPost]
        [Route("UpdateSupplierDetails")]
        public async Task<IActionResult> UpdateUserDetails(SupplierModel UpdateSupplier)
        {
            ApiResponseModel response = new ApiResponseModel();
            var updateUser = await _Supplier.UpdateSupplierDetails(UpdateSupplier);
            if (updateUser.code == 200)
            {
                response.code = (int)HttpStatusCode.OK;
                response.message = updateUser.message;

            }
            else
            {
                response.code = (int)HttpStatusCode.BadRequest;
            }

            return StatusCode(response.code, response);
        }
        [HttpPost]
        [Route("DeleteSupplierDetails")]
        public async Task<IActionResult> DeleteSupplierDetails(Guid SupplierId)
        {
            ApiResponseModel responseModel = new ApiResponseModel();
            var updateUser = await _Supplier.DeleteSupplierDetails(SupplierId);
            try
            {
                if (updateUser != null)
                {
                    responseModel.code = (int)HttpStatusCode.OK;
                    responseModel.message = updateUser.message;
                }
                else
                {
                    responseModel.message = updateUser.message;
                    responseModel.code = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                responseModel.code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(responseModel.code, responseModel);
        }
        [HttpPost]
        [Route("ActiveDeactiveSupplier")]
        public async Task<IActionResult> ActiveDeactiveSupplier(Guid SupplierId)
        {
            ApiResponseModel responseModel = new ApiResponseModel();
            var updateUser = await _Supplier.ActiveDeactiveSupplier(SupplierId);
            try
            {
                if (updateUser != null)
                {
                    responseModel.code = (int)HttpStatusCode.OK;
                    responseModel.message = updateUser.message;
                }
                else
                {
                    responseModel.message = updateUser.message;
                    responseModel.code = (int)HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                responseModel.code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(responseModel.code, responseModel);
        }
        [HttpGet]
        [Route("GetSupplierNameList")]
        public async Task<IActionResult> GetSupplierNameList()
        {
            IEnumerable<SupplierModel> SupplierName = await _Supplier.GetSupplierNameList();
            return Ok(new { code = 200, data = SupplierName.ToList() });
        }

        [HttpPost]
        [Route("ImportSupplierListFromExcel")]
        public async Task<IActionResult> ImportSupplierListFromExcel(List<SupplierModel> supplierList)
        {
            ApiResponseModel response = new ApiResponseModel();
            var addSupplierList = await _Supplier.ImportSupplierListFromExcel(supplierList);
            if (addSupplierList.code == 200)
            {
                response.code = (int)HttpStatusCode.OK;
                response.message = addSupplierList.message;

            }
            else
            {
                response.code = addSupplierList.code;
                response.message = addSupplierList.message;
            }
            return StatusCode(response.code, response);
        }
    }
}
