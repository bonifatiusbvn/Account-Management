using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.SalesMaster;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.SalesMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Abstractions;
using AccountManagement.DBContext.Models.ViewModels.PurchaseRequest;

namespace AccountManegments.Web.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        public APIServices APIServices { get; }

        public SalesController(APIServices aPIServices)
        {
            APIServices = aPIServices;
        }


        public IActionResult SalesList()
        {
            return View();
        }
        public IActionResult SalesReport()
        {
            return View();
        }

        public IActionResult CreateInventory()
        {
            return View();
        }
        public async Task<IActionResult> CreateSalesInvoice(Guid? Id)
        {
            SalesInvoiceMasterModel SalesDetails = new SalesInvoiceMasterModel();
            if (Id != null)
            {
                ApiResponseModel response = await APIServices.GetAsync("", "Sales/EditSalesInvoiceDetails?Id=" + Id);
                if (response.code == 200)
                {
                    SalesDetails = JsonConvert.DeserializeObject<SalesInvoiceMasterModel>(response.data.ToString());
                    int rowNumber = 0;
                    foreach (var item in SalesDetails.SalesItemList)
                    {
                        item.RowNumber = rowNumber++;
                    }
                }
                ViewBag.EditSalesShippingAddress = SalesDetails.ShippingAddress;
            }
            var SiteId = UserSession.SiteId;
            if (string.IsNullOrEmpty(SiteId))
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
                    ViewBag.SiteAddress = SiteName;
                }
            }
            return View(SalesDetails);
        }
        public async Task<IActionResult> CheckSalesInvoiceNo(Guid? CompanyId)
        {
            try
            {
                ApiResponseModel response = await APIServices.GetAsync("", "Sales/CheckSalesInvoiceNo?CompanyId=" + CompanyId);

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

        public async Task<IActionResult> GetAllSalesItemDetailList(string? searchText)
        {
            try
            {
                string apiUrl = $"ItemMaster/GetAllItemDetailsList?searchText={searchText}";
                ApiResponseModel response = await APIServices.PostAsync("", apiUrl);
                if (response.code == 200)
                {
                    List<ItemMasterModel> Items = JsonConvert.DeserializeObject<List<ItemMasterModel>>(response.data.ToString());
                    return PartialView("~/Views/Sales/_DisplaySalesAllItemPartial.cshtml", Items);
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


        public async Task<IActionResult> SalesInvoiceListAction(string? searchText, string? searchBy, string? sortBy)
        {
            try
            {
                string siteIdString = UserSession.SiteId;
                Guid? SiteId = !string.IsNullOrEmpty(siteIdString) ? Guid.Parse(siteIdString) : (Guid?)null;
                List<UserCompanyListModel> CompanyData = UserSession.CompanyData;

                string apiUrl = $"Sales/GetSalestInvoiceList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}";

                ApiResponseModel res = await APIServices.GetAsync("", apiUrl);

                if (res.code == 200)
                {
                    SalesInvoiceListView GetInvoiceList = JsonConvert.DeserializeObject<SalesInvoiceListView>(res.data.ToString());

                    if (SiteId.HasValue)
                    {
                        GetInvoiceList.SalesInvoiceList = GetInvoiceList.SalesInvoiceList
                            .Where(invoice => invoice.SiteId == SiteId.Value)
                            .ToList();

                        GetInvoiceList.SalesInvoiceItemList = GetInvoiceList.SalesInvoiceItemList
                            .Where(item => GetInvoiceList.SalesInvoiceList.Any(invoice => invoice.Id == item.Key))
                            .ToDictionary(
                                item => item.Key,
                                item => item.Value
                            );
                    }

                    if (CompanyData != null && CompanyData.Any())
                    {
                        var companyIds = CompanyData.Select(c => c.CompanyId).ToList();

                        GetInvoiceList.SalesInvoiceList = GetInvoiceList.SalesInvoiceList
                            .Where(invoice => companyIds.Contains(invoice.CompanyId))
                            .ToList();

                        GetInvoiceList.SalesInvoiceItemList = GetInvoiceList.SalesInvoiceItemList
                            .Where(item => GetInvoiceList.SalesInvoiceList.Any(invoice => invoice.Id == item.Key))
                            .ToDictionary(
                                item => item.Key,
                                item => item.Value
                            );
                    }

                    return PartialView("~/Views/Sales/_SalesInvoiceListPartial.cshtml", GetInvoiceList);
                }
                else
                {
                    return BadRequest(new { Message = "Failed to retrieve Sales Invoice list." });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DisplayItemListInSalesInvoice()
        {
            try
            {
                string ItemId = HttpContext.Request.Form["ITEMID"];
                var GetItem = JsonConvert.DeserializeObject<POItemDetailsModel>(ItemId.ToString());
                List<POItemDetailsModel> Items = new List<POItemDetailsModel>();
                ApiResponseModel response = await APIServices.GetAsync("", "ItemMaster/GetItemDetailsListById?ItemId=" + GetItem.ItemId);
                if (response.code == 200)
                {
                    Items = JsonConvert.DeserializeObject<List<POItemDetailsModel>>(response.data.ToString());
                    foreach (var item in Items)
                    {
                        item.Gstamount = (item.PricePerUnit * GetItem.Quantity * item.GstPercentage) / 100;
                        item.TotalAmount = (GetItem.Quantity * item.PricePerUnit) + item.Gstamount;
                        item.Quantity = GetItem.Quantity;
                    }
                }
                return PartialView("~/Views/Sales/_DisplaySalesItemDetailsPartial.cshtml", Items);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertSalesInvoiceDetails()
        {
            try
            {
                var SalesOrderDetails = HttpContext.Request.Form["SalesInvoiceDetails"];
                var SalesInvoicedetails = JsonConvert.DeserializeObject<SalesInvoiceMasterModel>(SalesOrderDetails.ToString());

                ApiResponseModel postuser = await APIServices.PostAsync(SalesInvoicedetails, "Sales/InsertSalesInvoiceDetails");
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
        public async Task<IActionResult> UpdateSalesInvoiceDetails()
        {
            try
            {
                var SalesOrderDetails = HttpContext.Request.Form["SalesInvoiceDetails"];
                var SalesInvoicedetails = JsonConvert.DeserializeObject<SalesInvoiceMasterModel>(SalesOrderDetails.ToString());

                ApiResponseModel postuser = await APIServices.PostAsync(SalesInvoicedetails, "Sales/UpdateSalesInvoiceDetails");
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
        public async Task<IActionResult> DeleteSalesInvoiceDetails(Guid Id)
        {
            ApiResponseModel response = await APIServices.GetAsync("", "Sales/DeleteSalesInvoiceDetails?Id=" + Id);

            if (response.code == 200)
            {
                return Ok(new { Code = response.code, Message = response.message });
            }
            else
            {
                return Ok(new { Code = response.code, Message = response.message });
            }
        }
        public async Task<IActionResult> DisplaySalesInvoiceDetails(Guid Id)
        {
            try
            {
                SalesInvoiceMasterModel SalesDetails = new SalesInvoiceMasterModel();
                ApiResponseModel response = await APIServices.GetAsync("", "Sales/EditSalesInvoiceDetails?Id=" + Id);
                if (response.code == 200)
                {
                    SalesDetails = JsonConvert.DeserializeObject<SalesInvoiceMasterModel>(response.data.ToString());
                    var number = SalesDetails.TotalAmount;
                    var totalAmountInWords = NumberToWords((decimal)number);
                    ViewData["TotalAmountInWords"] = totalAmountInWords + " " + "Only";
                    var gstamt = SalesDetails.TotalGstamount;
                    var totalGstInWords = NumberToWords((decimal)gstamt);
                    ViewData["TotalGstInWords"] = totalGstInWords + " " + "Only";
                }
                return View(SalesDetails);
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

        public async Task<IActionResult> PrintSalesInvoiceDetails(Guid Id)
        {
            try
            {
                SalesInvoiceMasterModel SalesDetails = new SalesInvoiceMasterModel();
                ApiResponseModel response = await APIServices.GetAsync("", "Sales/EditSalesInvoiceDetails?Id=" + Id);
                if (response.code == 200)
                {
                    SalesDetails = JsonConvert.DeserializeObject<SalesInvoiceMasterModel>(response.data.ToString());
                    var number = SalesDetails.TotalAmount;
                    var totalAmountInWords = NumberToWords((decimal)number);
                    ViewData["TotalAmountInWords"] = totalAmountInWords + " " + "Only";
                    var gstamt = SalesDetails.TotalGstamount;
                    var totalGstInWords = NumberToWords((decimal)gstamt);
                    ViewData["TotalGstInWords"] = totalGstInWords + " " + "Only";
                }
                return View(SalesDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> SalesInvoicePrintDetails(Guid Id)
        {
            try
            {
                IActionResult result = await PrintSalesInvoiceDetails(Id);

                if (result is ViewResult viewResult)
                {
                    var order = viewResult.Model as SalesInvoiceMasterModel;
                    var htmlContent = await RenderViewToStringAsync("PrintSalesInvoiceDetails", order, viewResult.ViewData);
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


        [FormPermissionAttribute("Inventory Inward-View")]
        public async Task<IActionResult> InventoryListAction(string searchText, string searchBy, string sortBy, Guid? SiteId)
        {
            try
            {


                string apiUrl = $"Sales/GetInventoryList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    List<InventoryInwardView> GetInventoryList = JsonConvert.DeserializeObject<List<InventoryInwardView>>(res.data.ToString());

                    return PartialView("~/Views/Sales/_DisplayInventoryPartial.cshtml", GetInventoryList);
                }
                else
                {
                    return BadRequest(new { Message = "Failed to retrieve Purchase Request list." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
