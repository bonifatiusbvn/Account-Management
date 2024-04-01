using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.DBContext.Models.ViewModels.SupplierMaster;
using AccountManegments.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccountManegments.Web.Controllers
{
    public class InvoiceMasterController : Controller
    {
        public APIServices APIServices { get; }

        public InvoiceMasterController(APIServices aPIServices)
        {
            APIServices = aPIServices;
        }
        public IActionResult CreateInvoice()
        {
            return View();
        }

        public IActionResult SupplierInvoiceListView()
        {
            return View();
        }

        public async Task<IActionResult> SupplierInvoiceListAction(string searchText, string searchBy, string sortBy)
        {
            try
            {

                string apiUrl = $"SupplierInvoice/GetSupplierInvoiceList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    List<SupplierInvoiceModel> GetInvoiceList = JsonConvert.DeserializeObject<List<SupplierInvoiceModel>>(res.data.ToString());

                    return PartialView("~/Views/InvoiceMaster/_SupplierInvoiceListPartial.cshtml", GetInvoiceList);
                }
                else
                {
                    return new JsonResult(new { Message = "Failed to retrieve Supplier Invoice list." });
                }
            }
            catch (Exception ex)
            {

                return new JsonResult(new { Message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSupplierInvoice(Guid InvoiceId)
        {
            try
            {

                ApiResponseModel postuser = await APIServices.PostAsync(null, "SupplierInvoice/DeleteSupplierInvoice?InvoiceId=" + InvoiceId);
                if (postuser.code == 200)
                {

                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });

                }
                else
                {
                    return new JsonResult(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public async Task<IActionResult> InsertMultipleSupplierItemDetails()
        {
            try
            {
                var OrderDetails = HttpContext.Request.Form["SupplierItems"];
                var InsertDetails = JsonConvert.DeserializeObject<List<SupplierInvoiceMasterView>>(OrderDetails.ToString());
                ApiResponseModel postuser = await APIServices.PostAsync(InsertDetails, "SupplierInvoice/InsertMultipleSupplierItemDetails");
                if (postuser.code == 200)
                {
                    return Ok(new { postuser.message });
                }
                else
                {
                    return Ok(new { postuser.message });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
