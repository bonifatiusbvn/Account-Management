using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.DataTableParameters;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.Repository.Interface.Services.InvoiceMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Net;

namespace AccountManagement.API.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

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
            var  supplierList = await SupplierInvoice.GetSupplierInvoiceList(searchText, searchBy, sortBy);
            return Ok(new { code = 200, data = supplierList});
        }

        [HttpGet]
        [Route("GetSupplierInvoiceById")]
        public async Task<IActionResult> GetSupplierInvoiceById(Guid Id)
        {
            var supplierDetails = await SupplierInvoice.GetSupplierInvoiceById(Id);
            return Ok(new { code = 200, data = supplierDetails });
        }

        [HttpPost]
        [Route("AddSupplierInvoice")]
        public async Task<IActionResult> AddSupplierInvoice(List<SupplierInvoiceModel> supplierInvoiceDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var SupplierDetails = await SupplierInvoice.AddSupplierInvoice(supplierInvoiceDetails);
            if (SupplierDetails.code == 200)
            {
                response.code = SupplierDetails.code;
                response.message = SupplierDetails.message;
            }
            else
            {
                response.code = (int)HttpStatusCode.BadRequest;
            }
            return StatusCode(response.code, response);
        }

        [HttpPost]
        [Route("UpdateSupplierInvoice")]
        public async Task<IActionResult> UpdateSupplierInvoice(SupplierInvoiceMasterView supplierInvoice)
        {
            ApiResponseModel response = new ApiResponseModel();
            var SupplierDetails = await SupplierInvoice.UpdateSupplierInvoice(supplierInvoice);
            if (SupplierDetails.code == 200)
            {
                response.code = SupplierDetails.code;
                response.message = SupplierDetails.message;
            }
            else
            {
                response.code = (int)HttpStatusCode.NotFound;
                response.message = "There Is Some Problem In Your Request!";
            }
            return StatusCode(response.code, response);
        }

        [HttpPost]
        [Route("DeleteSupplierInvoice")]
        public async Task<IActionResult> DeleteSupplierInvoice(Guid Id)
        {
            ApiResponseModel responseModel = new ApiResponseModel();

            var invoiceId = await SupplierInvoice.DeleteSupplierInvoice(Id);
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

        [HttpPost]
        [Route("GetInvoiceDetailsById")]
        public async Task<IActionResult> GetInvoiceDetailsById(InvoiceReportModel PayOutReport)
        {
            var tupleResult = await SupplierInvoice.GetInvoiceDetailsById(PayOutReport);
            return Ok(new { code = 200, data = tupleResult });
        }


        [HttpPost]
        [Route("InsertMultipleSupplierItemDetails")]
        public async Task<IActionResult> InsertMultipleSupplierItemDetails(SupplierInvoiceMasterView SupplierItemDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var SupplierInvoicemaster = await SupplierInvoice.InsertMultipleSupplierItemDetails(SupplierItemDetails);
            if (SupplierInvoicemaster.code == 200)
            {
                response.code = SupplierInvoicemaster.code;
                response.message = SupplierInvoicemaster.message;
                response.data = SupplierInvoicemaster.data;
            }
            else
            {
                response.code = (int)HttpStatusCode.NotFound;
                response.message = "There Is Some Problem In Your Request!";
            }
            return StatusCode(response.code, response);
        }

        [HttpGet]
        [Route("CheckSuppliersInvoiceNo")]
        public IActionResult CheckSuppliersInvoiceNo(Guid? CompanyId)
        {

            var checkInvoiceNo = SupplierInvoice.CheckSuppliersInvoiceNo(CompanyId);
            return Ok(new { code = 200, data = checkInvoiceNo });
        }

        [HttpPost]
        [Route("GetSupplierInvoiceDetailsById")]
        public async Task<IActionResult> GetSupplierInvoiceDetailsById(Guid SupplierId)
        {
            IEnumerable<SupplierInvoiceModel> supplierList = await SupplierInvoice.GetSupplierInvoiceDetailsById(SupplierId);
            return Ok(new { code = 200, data = supplierList.ToList() });
        }

        [HttpPost]
        [Route("GetSupplierInvoiceDetailsReport")]
        public async Task<IActionResult> GetSupplierInvoiceDetailsReport(DataTableRequstModel invoiceReport)
        {
            var supplierList = await SupplierInvoice.GetSupplierInvoiceDetailsReport(invoiceReport);
            return Ok(new { code = 200, data = supplierList });
        }


        [HttpPost]
        [Route("DeletePayoutDetails")]
        public async Task<IActionResult> DeletePayoutDetails(Guid InvoiceId)
        {
            ApiResponseModel responseModel = new ApiResponseModel();
            var payout = await SupplierInvoice.DeletePayoutDetails(InvoiceId);
            try
            {
                if (payout.code == 200)
                {
                    responseModel.code = (int)HttpStatusCode.OK;
                    responseModel.message = payout.message;
                }
                else
                {
                    responseModel.message = payout.message;
                    responseModel.code = payout.code;
                }
            }
            catch (Exception ex)
            {
                responseModel.code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(responseModel.code, responseModel);
        }

        [HttpGet]
        [Route("GetPayoutDetailsbyId")]
        public async Task<IActionResult> GetPayoutDetailsbyId(Guid InvoiceId)
        {
            var payout = await SupplierInvoice.GetPayoutDetailsbyId(InvoiceId);
            return Ok(new { code = 200, data = payout });
        }

        [HttpPost]
        [Route("UpdatePayoutDetails")]
        public async Task<IActionResult> UpdatePayoutDetails(SupplierInvoiceModel updatepayoutDetails)
        {
            ApiResponseModel responseModel = new ApiResponseModel();
            var payout = await SupplierInvoice.UpdatePayoutDetails(updatepayoutDetails);
            try
            {
                if (payout.code == 200)
                {
                    responseModel.code = (int)HttpStatusCode.OK;
                    responseModel.message = payout.message;
                }
                else
                {
                    responseModel.message = payout.message;
                    responseModel.code = payout.code;
                }
            }
            catch (Exception ex)
            {
                responseModel.code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(responseModel.code, responseModel);
        }

        [HttpPost]
        [Route("InvoiceIsApproved")]
        public async Task<IActionResult> InvoiceIsApproved(InvoiceIsApprovedMasterModel InvoiceIdList)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var Invoice = await SupplierInvoice.InvoiceIsApproved(InvoiceIdList);
                if (Invoice.code == 200)
                {
                    response.code = Invoice.code;
                    response.message = Invoice.message;
                }
                else
                {
                    response.message = Invoice.message;
                    response.code = Invoice.code;
                }
            }
            catch (Exception ex)
            {
                response.code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(response.code, response);
        }

        [HttpGet]
        [Route("CheckOpeningBalance")]
        public async Task<IActionResult> CheckOpeningBalance(Guid SupplierId, Guid CompanyId)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var OBDetails = await SupplierInvoice.CheckOpeningBalance(SupplierId, CompanyId);
                if (OBDetails.code == 200)
                {
                    response.code = OBDetails.code;
                    response.message = OBDetails.message;
                }
                else
                {
                    response.message = OBDetails.message;
                    response.code = OBDetails.code;
                }
            }
            catch (Exception ex)
            {
                response.code = (int)HttpStatusCode.InternalServerError;
            }
            return StatusCode(response.code, response);
        }

        [HttpPost]
        [Route("GetInvoiceDetailsPdfReport")]
        public async Task<IActionResult> GetInvoiceDetailsReportpdf(InvoiceReportModel invoiceReport)
        {
            var supplierList = await SupplierInvoice.GetInvoiceDetailsPdfReport(invoiceReport);
            return Ok(new { code = 200, data = supplierList});
        }
    }
}
