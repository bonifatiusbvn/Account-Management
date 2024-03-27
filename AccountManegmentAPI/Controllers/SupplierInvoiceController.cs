﻿using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.Repository.Interface.Services.InvoiceMaster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierInvoiceController : ControllerBase
    {
        public SupplierInvoiceController(ISupplierInvoiceService supplierInvoice)
        {
            SupplierInvoice = supplierInvoice;
        }

        public ISupplierInvoiceService SupplierInvoice { get; }


        [HttpPost]
        [Route("GetSupplierInvoiceList")]
        public async Task<IActionResult> GetSupplierInvoiceList(string? searchText, string? searchBy, string? sortBy)
        {
            IEnumerable<SupplierInvoiceModel> supplierList = await SupplierInvoice.GetSupplierInvoiceList(searchText, searchBy, sortBy);
            return Ok(new { code = 200, data = supplierList.ToList() });
        }

        [HttpGet]
        [Route("GetSupplierInvoiceById")]
        public async Task<IActionResult> GetSupplierInvoiceById(Guid InvoiceId)
        {
            var supplierDetails = await SupplierInvoice.GetSupplierInvoiceById(InvoiceId);
            return Ok(new { code = 200, data = supplierDetails });
        }

        [HttpPost]
        [Route("AddSupplierInvoice")]
        public async Task<IActionResult> AddSupplierInvoice(SupplierInvoiceModel supplierInvoiceDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var SupplierDetails = await SupplierInvoice.AddSupplierInvoice(supplierInvoiceDetails);
            if (SupplierDetails.code == 200)
            {
                response.code = SupplierDetails.code;
                response.message = SupplierDetails.message;
            }
            return StatusCode(response.code, response);
        }

        [HttpPost]
        [Route("UpdateSupplierInvoice")]
        public async Task<IActionResult> UpdateSupplierInvoice(SupplierInvoiceModel supplierInvoice)
        {
            ApiResponseModel response = new ApiResponseModel();
            var SupplierDetails = await SupplierInvoice.UpdateSupplierInvoice(supplierInvoice);
            if (SupplierDetails.code == 200)
            {
                response.code = SupplierDetails.code;
                response.message = SupplierDetails.message;
            }
            return StatusCode(response.code, response);
        }

        [HttpPost]
        [Route("DeleteSupplierInvoice")]
        public async Task<IActionResult> DeleteSupplierInvoice(Guid InvoiceId)
        {
            ApiResponseModel responseModel = new ApiResponseModel();

            var invoiceId = await SupplierInvoice.DeleteSupplierInvoice(InvoiceId);
            try
            {

                if (invoiceId != null)
                {

                    responseModel.code = (int)HttpStatusCode.OK;
                    responseModel.message = invoiceId.message;
                }
                else
                {
                    responseModel.message = invoiceId.message;
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