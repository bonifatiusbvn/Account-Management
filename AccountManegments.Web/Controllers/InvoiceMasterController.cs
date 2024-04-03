using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.DBContext.Models.ViewModels.SupplierMaster;
using AccountManegments.Web.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccountManegments.Web.Controllers
{
    [Authorize]
    public class InvoiceMasterController : Controller
    {
        public APIServices APIServices { get; }

        public InvoiceMasterController(APIServices aPIServices)
        {
            APIServices = aPIServices;
        }

        [FormPermissionAttribute("Invoice-Add")]
        public async Task<IActionResult> CreateInvoice()
        {
            try
            {
                ApiResponseModel Response = await APIServices.GetAsync("", "SupplierInvoice/CheckSupplierInvoiceNo");

                if (Response.code == 200)
                {
                    ViewData["SupplierInvoiceNo"] = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(Response.data));
                }


            }
            catch (Exception ex)
            {
                throw ex;

            }
            return View();
        }

        [FormPermissionAttribute("Invoice-View")]
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


        [FormPermissionAttribute("Invoice-Delete")]
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

        [FormPermissionAttribute("Invoice-View")]
        public IActionResult PayOutInvoice()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetInvoiceDetails(Guid CompanyId, Guid SupplierId)
        {
            try
            {
                ApiResponseModel postuser = await APIServices.PostAsync(null, "SupplierInvoice/GetInvoiceDetailsById?CompanyId=" + CompanyId + "&SupplierId=" + SupplierId);
                if (postuser.code == 200)
                {
                    List<SupplierInvoiceModel> GetInvoiceList = JsonConvert.DeserializeObject<List<SupplierInvoiceModel>>(postuser.data.ToString());
                    if(GetInvoiceList.Count == 0)
                    {
                        return new JsonResult("There is no data for selected Supplier!");
                    }
                    else
                    {
                        return PartialView("~/Views/InvoiceMaster/_GetInvoiceDetailsPartial.cshtml", GetInvoiceList);
                    }
                }
                else
                {
                    return BadRequest(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching invoice details.", Error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> DisplayItemDetailById()
        {
            try
            {
                string ItemId = HttpContext.Request.Form["ITEMID"];
                var GetItem = JsonConvert.DeserializeObject<ItemMasterModel>(ItemId.ToString());
                ItemMasterModel Items = new ItemMasterModel();
                ApiResponseModel response = await APIServices.GetAsync("", "ItemMaster/GetItemDetailsById?ItemId=" + GetItem.ItemId);
                if (response.code == 200)
                {
                    Items = JsonConvert.DeserializeObject<ItemMasterModel>(response.data.ToString());
                    Items.RowNumber = Items.RowNumber;
                }
                return PartialView("~/Views/InvoiceMaster/_GetItemsDetailsListPartial.cshtml", Items);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [FormPermissionAttribute("Invoice-Add")]

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


        [HttpPost]
        public async Task<JsonResult> GetPayOutDetailsForTotalAmount()
        {
            try
            {
                var TotalAmount = HttpContext.Request.Form["TOTALAMOUNT"];
                var Details = JsonConvert.DeserializeObject<SupplierInvoiceModel>(TotalAmount);
                List<SupplierInvoiceModel> GetInvoiceList = new List<SupplierInvoiceModel>();
                ApiResponseModel postuser = await APIServices.PostAsync(null, "SupplierInvoice/GetPayOutDetailsForTotalAmount?CompanyId=" + Details.CompanyId + "&SupplierId=" + Details.SupplierId);
                if (postuser.code == 200)
                {
                    GetInvoiceList = JsonConvert.DeserializeObject<List<SupplierInvoiceModel>>(postuser.data.ToString());
                }
                return new JsonResult(GetInvoiceList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public async Task<IActionResult> InsertPayOutDetails()
        {
            try
            {
                var payout = HttpContext.Request.Form["PAYOUTDETAILS"];
                var InsertDetails = JsonConvert.DeserializeObject<SupplierInvoiceModel>(payout);

                ApiResponseModel postuser = await APIServices.PostAsync(InsertDetails, "SupplierInvoice/AddSupplierInvoice");
                if (postuser.code == 200)
                {
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser });
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
    }
}
