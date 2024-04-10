using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.DBContext.Models.ViewModels.SupplierMaster;
using AccountManegments.Web.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
                    var jsonString = postuser.data.ToString();
                    var tupleResult = JsonConvert.DeserializeObject<InvoiceTotalAmount>(jsonString);

                    if (tupleResult == null)
                    {
                        return new JsonResult("Failed to deserialize API response");
                    }

                    
                    return new JsonResult(tupleResult);

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
                var InsertDetails = JsonConvert.DeserializeObject<SupplierInvoiceMasterView>(OrderDetails.ToString());
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

        public IActionResult InvoiceListView()
        {
            return View();
        }

        public async Task<IActionResult> InvoiceListAction(string searchText, string searchBy, string sortBy)
        {
            try
            {

                string apiUrl = $"SupplierInvoice/GetSupplierInvoiceList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    List<SupplierInvoiceModel> GetInvoiceList = JsonConvert.DeserializeObject<List<SupplierInvoiceModel>>(res.data.ToString());

                    return PartialView("~/Views/InvoiceMaster/_InvoiceListPartial.cshtml", GetInvoiceList);
                }
                else
                {
                    return new JsonResult(new { Message = "Failed to retrieve Invoice list." });
                }
            }
            catch (Exception ex)
            {

                return new JsonResult(new { Message = $"An error occurred: {ex.Message}" });
            }
        }
        public async Task<IActionResult> DisplayInvoiceDetails(Guid Id)
        {
            try
            {
                SupplierInvoiceMasterView order = new SupplierInvoiceMasterView();
                ApiResponseModel response = await APIServices.GetAsync("", "SupplierInvoice/GetSupplierInvoiceById?Id=" + Id);
                if (response.code == 200)
                {
                    order = JsonConvert.DeserializeObject<SupplierInvoiceMasterView>(response.data.ToString());
                    var number = order.TotalAmount;
                    var totalAmountInWords = NumberToWords((int)number);
                    ViewData["TotalAmountInWords"] = totalAmountInWords + " " + "Only";
                    var gstamt = order.TotalGstamount;
                    var totalGstInWords = NumberToWords((int)gstamt);
                    ViewData["TotalGstInWords"] = totalGstInWords + " " + "Only";
                }
                return View(order);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }
            return words;
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplierInvoiceDetailsById(Guid SupplierId)
        {
            try
            {
                List<SupplierInvoiceModel> SupplierDetails = new List<SupplierInvoiceModel>();
                ApiResponseModel response = await APIServices.PostAsync(null, "SupplierInvoice/GetSupplierInvoiceDetailsById?SupplierId=" + SupplierId);
                if (response.code == 200)
                {
                    SupplierDetails = JsonConvert.DeserializeObject<List<SupplierInvoiceModel>>(response.data.ToString());
                }
                return PartialView("~/Views/InvoiceMaster/_GetInvoiceDetailsPartial.cshtml", SupplierDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
