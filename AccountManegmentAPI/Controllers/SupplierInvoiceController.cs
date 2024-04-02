using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
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
        public async Task<IActionResult> GetSupplierInvoiceById(Guid Id)
        {
            var supplierDetails = await SupplierInvoice.GetSupplierInvoiceById(Id);
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
        public async Task<IActionResult> GetInvoiceDetailsById(Guid CompanyId, Guid SupplierId)
        {
            IEnumerable<SupplierInvoiceModel> supplierDetails = await SupplierInvoice.GetInvoiceDetailsById(CompanyId, SupplierId);
            return Ok(new { code = 200, data = supplierDetails.ToList() });
        }

        [HttpPost]
        [Route("InsertMultipleSupplierItemDetails")]
        public async Task<IActionResult> InsertMultipleSupplierItemDetails(List<SupplierInvoiceMasterView> SupplierItemDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            var SupplierInvoicemaster = await SupplierInvoice.InsertMultipleSupplierItemDetails(SupplierItemDetails);
            if (SupplierInvoicemaster.code == 200)
            {
                response.code = SupplierInvoicemaster.code;
                response.message = SupplierInvoicemaster.message;
            }
            else
            {
                response.code = (int)HttpStatusCode.NotFound;
                response.message = "There Is Some Problem In Your Request!";
            }
            return StatusCode(response.code, response);
        }

        [HttpGet]
        [Route("CheckSupplierInvoiceNo")]
        public IActionResult CheckSupplierInvoiceNo()
        {
            var checkInvoiceNo = SupplierInvoice.CheckSupplierInvoiceNo();
            return Ok(new { code = 200, data = checkInvoiceNo });
        }
        [HttpPost]
        [Route("GetPayOutDetailsByInvoiceNo")]
        public async Task<IActionResult> GetPayOutDetailsByInvoiceNo(string InvoiceNo)
        {
            IEnumerable<SupplierInvoiceModel> supplierDetails = await SupplierInvoice.GetPayOutDetailsByInvoiceNo(InvoiceNo);
            return Ok(new { code = 200, data = supplierDetails.ToList() });
        }
    }
}
