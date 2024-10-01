using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.Repository.Interface.Services.InvoiceMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountManagement.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class SupplierInvoiceDetailsController : ControllerBase
    {
        public SupplierInvoiceDetailsController(ISupplierInvoiceDetailsService supplierInvoiceDetails)
        {
            SupplierInvoiceDetails = supplierInvoiceDetails;
        }

        public ISupplierInvoiceDetailsService SupplierInvoiceDetails { get; }

        [HttpPost]
        [Route("GetSupplierInvoiceDetailsList")]
        public async Task<IActionResult> GetSupplierInvoiceDetailsList(string? searchText, string? searchBy, string? sortBy)
        {
            IEnumerable<SupplierInvoiceDetailsModel> supplierDetailsList = await SupplierInvoiceDetails.GetSupplierInvoiceDetailsList(searchText, searchBy, sortBy);
            return Ok(new { code = 200, data = supplierDetailsList.ToList() });
        }

        [HttpGet]
        [Route("GetSupplierInvoiceDetailsById")]
        public async Task<IActionResult> GetSupplierInvoiceDetailsById(int InvoiceId)
        {
            var supplierInvoiceDetails = await SupplierInvoiceDetails.GetSupplierInvoiceDetailsById(InvoiceId);
            return Ok(new { code = 200, data = supplierInvoiceDetails });
        }

        [HttpPost]
        [Route("AddSupplierInvoiceDetails")]
        public async Task<IActionResult> AddSupplierInvoiceDetails(SupplierInvoiceDetailsModel SupplierInvoiceDetail)
        {
            ApiResponseModel response = new ApiResponseModel();
            var supplierInvoiceDetails = await SupplierInvoiceDetails.AddSupplierInvoiceDetails(SupplierInvoiceDetail);
            if (supplierInvoiceDetails.code == 200)
            {
                response.code = supplierInvoiceDetails.code;
                response.message = supplierInvoiceDetails.message;
            }
            else
            {
                response.code = (int)HttpStatusCode.BadRequest;
            }

            return StatusCode(response.code, response);
        }

        [HttpPost]
        [Route("UpdateSupplierInvoiceDetails")]
        public async Task<IActionResult> UpdateSupplierInvoiceDetails(SupplierInvoiceDetailsModel SupplierInvoiceDetail)
        {
            ApiResponseModel response = new ApiResponseModel();
            var supplierInvoiceDetails = await SupplierInvoiceDetails.UpdateSupplierInvoiceDetails(SupplierInvoiceDetail);
            if (supplierInvoiceDetails.code == 200)
            {
                response.code = supplierInvoiceDetails.code;
                response.message = supplierInvoiceDetails.message;
            }
            else
            {
                response.code = (int)HttpStatusCode.BadRequest;
            }

            return StatusCode(response.code, response);
        }

        [HttpPost]
        [Route("DeleteSupplierInvoiceDetails")]
        public async Task<IActionResult> DeleteSupplierInvoiceDetails(int InvoiceId)
        {
            ApiResponseModel responseModel = new ApiResponseModel();

            var invoiceId = await SupplierInvoiceDetails.DeleteSupplierInvoiceDetails(InvoiceId);
            try
            {

                if (invoiceId.code == 200)
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

        [HttpGet]
        [Route("GetSupplierPendingDetailsList")]
        public async Task<IActionResult> GetSupplierPendingDetailsList(Guid CompanyId)
        {
            IEnumerable<SupplierPendingDetailsModel> supplierDetails = await SupplierInvoiceDetails.GetSupplierPendingDetailsList(CompanyId);
            return Ok(new { code = 200, data = supplierDetails.ToList() });
        }
    }
}
