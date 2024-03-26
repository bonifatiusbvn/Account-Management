﻿using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SupplierMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Interfaces.Authentication;
using AccountManagement.Repository.Interface.Services.SupplierService;
using AccountManagement.Repository.Services.Company;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierMasterController : ControllerBase
    {
        public SupplierMasterController(ISupplierServices supplier)
        {
            _Supplier = supplier;
        }

        public ISupplierServices _Supplier { get; }

        [HttpPost]
        [Route("CreateSupplier")]
        public async Task<IActionResult> CreateUser(SupplierModel Supplier)
        {
            ApiResponseModel response = new ApiResponseModel();
            var CreateSupplier = await _Supplier.CreateSupplier(Supplier);
            if (CreateSupplier.code == 200)
            {
                response.code = (int)HttpStatusCode.OK;
                response.message = CreateSupplier.message;

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
    }
}