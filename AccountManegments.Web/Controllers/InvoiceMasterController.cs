using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.SupplierMaster;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Permissions;

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

        [FormPermissionAttribute("Purchase  Invoice-Add")]
        public async Task<IActionResult> CreateInvoice(Guid? Id)
        {
            try
            {
                SupplierInvoiceMasterView invoiceDetails = new SupplierInvoiceMasterView();
                if (Id != null)
                {
                    ApiResponseModel response = await APIServices.GetAsync("", "SupplierInvoice/GetSupplierInvoiceById?Id=" + Id);
                    if (response.code == 200)
                    {
                        invoiceDetails = JsonConvert.DeserializeObject<SupplierInvoiceMasterView>(response.data.ToString());
                        int rowNumber = 0;
                        foreach (var item in invoiceDetails.ItemList)
                        {
                            item.RowNumber = rowNumber++;
                        }
                    }
                    ViewBag.SupplierInvoiceNo = invoiceDetails.InvoiceNo;
                }

                var SiteId = UserSession.SiteId;
                if (SiteId == "")
                {
                    ViewBag.SiteAddress = "";
                }
                else
                {
                    List<SiteAddressModel> SiteName = new List<SiteAddressModel>();
                    ApiResponseModel res = await APIServices.GetAsync("", "SiteMaster/GetSiteAddressList?SiteId=" + SiteId);
                    if (res.code == 200)
                    {
                        SiteName = JsonConvert.DeserializeObject<List<SiteAddressModel>>(res.data.ToString());
                    }

                    if (SiteName.Count == 0)
                    {
                        SiteMasterModel SiteDetails = new SiteMasterModel();
                        ApiResponseModel response = await APIServices.GetAsync("", "SiteMaster/GetSiteDetailsById?SiteId=" + SiteId);
                        if (response.code == 200)
                        {
                            SiteDetails = JsonConvert.DeserializeObject<SiteMasterModel>(response.data.ToString());
                        }
                        ViewBag.SiteAddress = SiteDetails.ShippingAddress + " , " + SiteDetails.ShippingArea + " , " + SiteDetails.ShippingCityName + " , " + SiteDetails.ShippingStateName + " , " + SiteDetails.ShippingCountryName + ",Code : " + SiteDetails.StateCode;
                    }
                    else
                    {
                        ViewBag.SiteAddress = SiteName[0].Address;
                    }
                }

                return View(invoiceDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IActionResult> CheckSuppliersInvoiceNo(Guid? CompanyId)
        {
            try
            {
                ApiResponseModel response = await APIServices.GetAsync("", "SupplierInvoice/CheckSuppliersInvoiceNo?CompanyId=" + CompanyId);

                if (response.code == 200)
                {
                    return Ok(new { Data = response.data, Code = 200 });
                }
                else
                {
                    return Ok(new { Code = 400, Message = "Failed to create invoice", });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"An error occurred: {ex.Message}" });
            }
        }

        [FormPermissionAttribute("Purchase  Invoice-View")]
        public IActionResult SupplierInvoiceListView()
        {
            return View();
        }

        [FormPermissionAttribute("Purchase  Invoice-View")]
        public async Task<IActionResult> SupplierInvoiceListAction(string searchText, string searchBy, string sortBy)
        {
            try
            {
                string siteIdString = UserSession.SiteId;
                Guid? SiteId = !string.IsNullOrEmpty(siteIdString) ? Guid.Parse(siteIdString) : (Guid?)null;

                string apiUrl = $"SupplierInvoice/GetSupplierInvoiceList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    List<SupplierInvoiceModel> GetInvoiceList = JsonConvert.DeserializeObject<List<SupplierInvoiceModel>>(res.data.ToString());
                    if (SiteId != null)
                    {
                        GetInvoiceList = GetInvoiceList.Where(a => a.SiteId == SiteId).ToList();
                    }

                    return PartialView("~/Views/InvoiceMaster/_SupplierInvoiceListPartial.cshtml", GetInvoiceList);
                }
                else
                {
                    return BadRequest(new { Message = "Failed to retrieve Supplier Invoice list." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"An error occurred: {ex.Message}" });
            }
        }


        [FormPermissionAttribute("Purchase  Invoice-Delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteSupplierInvoice(Guid Id)
        {
            try
            {

                ApiResponseModel postuser = await APIServices.PostAsync("", "SupplierInvoice/DeleteSupplierInvoice?Id=" + Id);
                if (postuser.code == 200)
                {
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
                else
                {
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [FormPermissionAttribute("Payment Out-View")]
        public IActionResult PayOutInvoice()
        {
            return View();
        }

        [FormPermissionAttribute("Payment Out-View")]
        [HttpGet]
        public async Task<IActionResult> GetInvoiceDetails(Guid CompanyId, Guid SupplierId)
        {
            try
            {
                ApiResponseModel postuser = await APIServices.PostAsync("", "SupplierInvoice/GetInvoiceDetailsById?CompanyId=" + CompanyId + "&SupplierId=" + SupplierId);
                if (postuser.code == 200)
                {
                    var jsonString = postuser.data.ToString();
                    var tupleResult = JsonConvert.DeserializeObject<InvoiceTotalAmount>(jsonString);

                    if (tupleResult == null)
                    {
                        return BadRequest("Failed to deserialize API response");
                    }
                    return new JsonResult(tupleResult);
                }
                else
                {
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });
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


        [FormPermissionAttribute("Purchase  Invoice-Add")]
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
                    return Ok(new { postuser.message, postuser.code, postuser.data });
                }
                else
                {
                    return Ok(new { postuser.message, postuser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSupplierInvoice()
        {
            try
            {
                var OrderDetails = HttpContext.Request.Form["UpdateSupplierItems"];
                var DateFormat = new JsonSerializerSettings
                {
                    DateFormatString = "dd-MM-yyyy HH:mm:ss",
                    Error = (sender, args) =>
                    {
                        args.ErrorContext.Handled = true;
                    }
                };
                var UpdateDetails = JsonConvert.DeserializeObject<SupplierInvoiceMasterView>(OrderDetails, DateFormat);
                ApiResponseModel postuser = await APIServices.PostAsync(UpdateDetails, "SupplierInvoice/UpdateSupplierInvoice");
                if (postuser.code == 200)
                {
                    return Ok(new { postuser.message, postuser.code });
                }
                else
                {
                    return Ok(new { postuser.message, postuser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [FormPermissionAttribute("Payment Out-Add")]
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
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
                else
                {
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [FormPermissionAttribute("Purchase  Invoice-View")]
        public IActionResult InvoiceListView()
        {
            return View();
        }
        [FormPermissionAttribute("Purchase  Invoice-View")]
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
                    return BadRequest(new { Message = "Failed to retrieve Invoice list." });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new { Message = $"An error occurred: {ex.Message}" });
            }
        }

        [FormPermissionAttribute("Purchase  Invoice-View")]
        public async Task<IActionResult> DisplayInvoiceDetails(Guid Id)
        {
            try
            {
                SupplierInvoiceMasterView order = new SupplierInvoiceMasterView();
                ApiResponseModel response = await APIServices.GetAsync("", "SupplierInvoice/GetSupplierInvoiceById?Id=" + Id);
                if (response.code == 200)
                {
                    order = JsonConvert.DeserializeObject<SupplierInvoiceMasterView>(response.data.ToString());
                    var number = order.TotalAmountInvoice;
                    var totalAmountInWords = NumberToWords((decimal)number);
                    ViewData["TotalAmountInWords"] = totalAmountInWords + " " + "Only";
                    var gstamt = order.TotalGstamount;
                    var totalGstInWords = NumberToWords((decimal)gstamt);
                    ViewData["TotalGstInWords"] = totalGstInWords + " " + "Only";
                }
                return View(order);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string NumberToWords(decimal number)
        {
            int integerPart = (int)Math.Floor(number);
            decimal decimalPart = number - integerPart;

            string words = "";

            if (integerPart == 0)
            {
                words = "zero";
            }
            else if (integerPart < 0)
            {
                words = "minus " + NumberToWords(Math.Abs(integerPart));
            }
            else
            {
                if ((integerPart / 10000000) > 0)
                {
                    words += NumberToWords(integerPart / 10000000) + " Crore ";
                    integerPart %= 10000000;
                }

                if ((integerPart / 100000) > 0)
                {
                    words += NumberToWords(integerPart / 100000) + " Lakh ";
                    integerPart %= 100000;
                }

                if ((integerPart / 1000) > 0)
                {
                    words += NumberToWords(integerPart / 1000) + " Thousand ";
                    integerPart %= 1000;
                }

                if ((integerPart / 100) > 0)
                {
                    words += NumberToWords(integerPart / 100) + " Hundred ";
                    integerPart %= 100;
                }

                if (integerPart > 0)
                {
                    var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                    var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                    if (words != "")
                        words += " ";

                    if (integerPart < 20)
                        words += unitsMap[integerPart];
                    else
                    {
                        words += tensMap[integerPart / 10];
                        if ((integerPart % 10) > 0)
                            words += " " + unitsMap[integerPart % 10];
                    }
                }
            }

            if (decimalPart > 0)
            {
                decimalPart *= 100;
                words += " and " + NumberToWords((int)decimalPart) + " paisa";
            }

            return words;
        }


        [FormPermissionAttribute("Purchase  Invoice-View")]
        [HttpGet]
        public async Task<IActionResult> GetSupplierInvoiceDetailsById(Guid SupplierId)
        {
            try
            {
                List<SupplierInvoiceModel> SupplierDetails = new List<SupplierInvoiceModel>();
                ApiResponseModel response = await APIServices.PostAsync("", "SupplierInvoice/GetSupplierInvoiceDetailsById?SupplierId=" + SupplierId);
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

        public async Task<IActionResult> GetAllItemDetailList(string? searchText)
        {
            try
            {
                string apiUrl = $"ItemMaster/GetAllItemDetailsList?searchText={searchText}";
                ApiResponseModel response = await APIServices.PostAsync("", apiUrl);
                if (response.code == 200)
                {
                    List<ItemMasterModel> Items = JsonConvert.DeserializeObject<List<ItemMasterModel>>(response.data.ToString());
                    return PartialView("~/Views/InvoiceMaster/_DisplayAllItemPartial.cshtml", Items);
                }
                else
                {
                    return BadRequest(new { Message = "Failed to retrieve Purchase Order list" });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditInvoiceDetails(string Id)
        {
            try
            {
                return RedirectToAction("CreateInvoice", new { id = Id });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IActionResult> InvoicePrintDetails(Guid Id)
        {
            try
            {
                SupplierInvoiceMasterView order = new SupplierInvoiceMasterView();
                ApiResponseModel response = await APIServices.GetAsync("", $"SupplierInvoice/GetSupplierInvoiceById?Id={Id}");
                if (response.code == 200)
                {
                    order = JsonConvert.DeserializeObject<SupplierInvoiceMasterView>(response.data.ToString());
                    var number = order.TotalAmountInvoice;
                    var totalAmountInWords = NumberToWords((decimal)number);
                    ViewData["TotalAmountInWords"] = totalAmountInWords + " " + "Only";
                    var gstamt = order.TotalGstamount;
                    var totalGstInWords = NumberToWords((decimal)gstamt);
                    ViewData["TotalGstInWords"] = totalGstInWords + " " + "Only";
                }
                return View(order);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> PrintInvoiceDetails(Guid Id)
        {
            try
            {
                IActionResult result = await InvoicePrintDetails(Id);

                if (result is ViewResult viewResult)
                {
                    var order = viewResult.Model as SupplierInvoiceMasterView;
                    var htmlContent = await RenderViewToStringAsync("InvoicePrintDetails", order, viewResult.ViewData);
                    return Content(htmlContent, "text/html");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<string> RenderViewToStringAsync(string viewName, object model, ViewDataDictionary viewData)
        {
            var viewEngine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
            var tempDataProvider = HttpContext.RequestServices.GetService(typeof(ITempDataProvider)) as ITempDataProvider;
            var tempData = new TempDataDictionary(HttpContext, tempDataProvider);
            var actionContext = new ActionContext(HttpContext, RouteData, new ActionDescriptor());

            using (var stringWriter = new StringWriter())
            {
                var viewResult = viewEngine.FindView(actionContext, viewName, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"View '{viewName}' was not found.");
                }

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewData,
                    tempData,
                    stringWriter,
                    new HtmlHelperOptions()
                );
                await viewResult.View.RenderAsync(viewContext);
                return stringWriter.ToString();
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetSupplierInvoiceDetailsReport(InvoiceReportModel invoiceReport)
        {
            try
            {
                List<SupplierInvoiceModel> SupplierDetails = new List<SupplierInvoiceModel>();
                ApiResponseModel response = await APIServices.PostAsync(invoiceReport, "SupplierInvoice/GetSupplierInvoiceDetailsReport");
                if (response.code == 200)
                {
                    SupplierDetails = JsonConvert.DeserializeObject<List<SupplierInvoiceModel>>(response.data.ToString());
                }
                return PartialView("~/Views/Report/_ReportDetailsPartial.cshtml", SupplierDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
