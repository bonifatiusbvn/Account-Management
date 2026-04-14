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
using AccountManagement.DBContext.Models.DataTableParameters;
using Aspose.Pdf.Text;
using Aspose.Pdf;
using ClosedXML.Excel;
using System.Globalization;

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


        public async Task<IActionResult> SalesInvoiceListAction(string? searchText, string? searchBy, string? sortBy, Guid? CompanyId, Guid? SupplierId)
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
                    if (CompanyId != null)
                    {
                        GetInvoiceList.SalesInvoiceList = GetInvoiceList.SalesInvoiceList
                            .Where(invoice => invoice.CompanyId == CompanyId.Value)
                            .ToList();
                    }
                    if (SupplierId != null)
                    {
                        GetInvoiceList.SalesInvoiceList = GetInvoiceList.SalesInvoiceList
                            .Where(invoice => invoice.SupplierId == SupplierId.Value)
                            .ToList();
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
        [FormPermissionAttribute("Inventory Inward-Add")]
        [HttpPost]
        public async Task<IActionResult> InsertInventoryDetails(InventoryInwardView InventoryDetails)
        {

            try
            {
                ApiResponseModel postUser = await APIServices.PostAsync(InventoryDetails, "Sales/InsertInventoryDetails");
                if (postUser.code == 200)
                {
                    return Ok(new { Message = postUser.message, Code = postUser.code });
                }
                else
                {
                    return Ok(new { Message = string.Format(postUser.message), Code = postUser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [FormPermissionAttribute("Inventory Inward-Edit")]
        public async Task<IActionResult> EditInventoryDetails(Guid InventoryId)
        {
            var Inventory = new InventoryInwardView();
            ApiResponseModel response = await APIServices.GetAsync("", "Sales/EditInventoryDetails?Id=" + InventoryId);

            if (response.code == 200)
            {
                Inventory = JsonConvert.DeserializeObject<InventoryInwardView>(response.data.ToString());
                return Ok(Inventory);
            }
            else
            {
                return Ok(new { Code = response.code, Message = response.message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateInventoryDetails(InventoryInwardView InventoryDetails)
        {

            try
            {
                ApiResponseModel postUser = await APIServices.PostAsync(InventoryDetails, "Sales/UpdateInventoryDetails");
                if (postUser.code == 200)
                {
                    return Ok(new { Message = postUser.message, Code = postUser.code });
                }
                else
                {
                    return Ok(new { Message = string.Format(postUser.message), Code = postUser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [FormPermissionAttribute("Inventory Inward-Delete")]
        public async Task<IActionResult> DeleteInventoryDetails(Guid InventoryId)
        {
            ApiResponseModel response = await APIServices.GetAsync("", "Sales/DeleteInventoryDetails?InventoryId=" + InventoryId);

            if (response.code == 200)
            {
                return Ok(new { Code = response.code, Message = response.message });
            }
            else
            {
                return Ok(new { Code = response.code, Message = response.message });
            }
        }
        public async Task<IActionResult> ApproveInventoryDetails(Guid InventoryId)
        {
            ApiResponseModel response = await APIServices.GetAsync("", "Sales/ApproveInventoryDetails?InventoryId=" + InventoryId);

            if (response.code == 200)
            {
                return Ok(new { Code = response.code, Message = response.message });
            }
            else
            {
                return Ok(new { Code = response.code, Message = response.message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SalesInvoiceReport(SalesInvoiceReportModel SalesReport)
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                var sortColumn = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();

                var dataTable = new DataTableRequstModel
                {
                    draw = draw,
                    start = start,
                    pageSize = pageSize,
                    skip = skip,
                    lenght = length,
                    searchValue = searchValue,
                    sortColumn = sortColumn,
                    sortColumnDir = sortColumnDir,
                    filterType = SalesReport.filterType,
                    SupplierId = SalesReport.SupplierId,
                    CompanyId = SalesReport.CompanyId,
                    SelectedYear = SalesReport.SelectedYear,
                    startDate = SalesReport.startDate,
                    endDate = SalesReport.endDate,
                    SupplierName = SalesReport.SupplierName,
                    CompanyName = SalesReport.CompanyName,
                    TillMonth = SalesReport.TillMonth,
                };
                List<SalesInvoiceMasterModel> SalesDetails = new List<SalesInvoiceMasterModel>();
                var jsonData = new jsonData();
                ApiResponseModel response = await APIServices.PostAsync(dataTable, "Sales/SalesInvoiceReport");
                if (response.code == 200)
                {
                    jsonData = JsonConvert.DeserializeObject<jsonData>(response.data.ToString());
                    SalesDetails = JsonConvert.DeserializeObject<List<SalesInvoiceMasterModel>>(jsonData.data.ToString());

                    var result = new
                    {
                        draw = jsonData.draw,
                        recordsFiltered = jsonData.recordsFiltered,
                        recordsTotal = jsonData.recordsTotal,
                        data = SalesDetails,
                        TotalCredit = jsonData.TotalCredit,
                        TotalDebit = jsonData.TotalDebit,
                    };
                    return new JsonResult(result);
                }
                else
                {
                    return BadRequest(new { message = "Failed to retrieve data from the API." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [FormPermissionAttribute("Reports & Payments-Add")]
        [HttpPost]
        public async Task<IActionResult> InsertSalesPayInInvoice()
        {
            try
            {
                var payin = HttpContext.Request.Form["PAYINDETAILS"];
                var payinDetails = JsonConvert.DeserializeObject<List<SalesInvoiceMasterModel>>(payin);

                ApiResponseModel postuser = await APIServices.PostAsync(payinDetails, "Sales/InsertSalesPayInInvoice");
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

        [HttpPost]
        public async Task<IActionResult> SalesInvoicePaymentReport(SalesInvoiceReportModel SalesPaymentReport)
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                var sortColumn = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();
                if (sortColumnIndex == "5")
                {
                    sortColumn = "credit";
                }
                if (sortColumnIndex == "6")
                {
                    sortColumn = "debit";
                }

                var dataTable = new DataTableRequstModel
                {
                    draw = draw,
                    start = start,
                    pageSize = pageSize,
                    skip = skip,
                    lenght = length,
                    searchValue = searchValue,
                    sortColumn = sortColumn,
                    sortColumnDir = sortColumnDir,
                    filterType = SalesPaymentReport.filterType,
                    SupplierId = SalesPaymentReport.SupplierId,
                    CompanyId = SalesPaymentReport.CompanyId,
                    SelectedYear = SalesPaymentReport.SelectedYear,
                    startDate = SalesPaymentReport.startDate,
                    endDate = SalesPaymentReport.endDate,
                    SupplierName = SalesPaymentReport.SupplierName,
                    CompanyName = SalesPaymentReport.CompanyName,
                };
                List<SalesInvoiceMasterModel> SalesInvoiceDetails = new List<SalesInvoiceMasterModel>();
                var jsonData = new jsonData();
                ApiResponseModel response = await APIServices.PostAsync(dataTable, "Sales/SalesInvoicePaymentReport");
                if (response.code == 200)
                {
                    jsonData = JsonConvert.DeserializeObject<jsonData>(response.data.ToString());
                    SalesInvoiceDetails = JsonConvert.DeserializeObject<List<SalesInvoiceMasterModel>>(jsonData.data.ToString());

                    var result = new
                    {
                        TotalCredit = jsonData.TotalCredit,
                        TotalDebit = jsonData.TotalDebit,
                        draw = jsonData.draw,
                        recordsFiltered = jsonData.recordsFiltered,
                        recordsTotal = jsonData.recordsTotal,
                        data = SalesInvoiceDetails,
                    };

                    return new JsonResult(result);
                }
                else
                {
                    return BadRequest(new { message = "Failed to retrieve data from the API." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> DisplayPayInSalesInvoice()
        {
            try
            {
                List<SalesInvoiceMasterModel> payininvoice = new List<SalesInvoiceMasterModel>
                {
                    new SalesInvoiceMasterModel()
                };
                return PartialView("~/Views/Sales/_PayInSalesInvoicePartial.cshtml", payininvoice);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<JsonResult> EditSalesPayInInvoice(Guid SalesId)
        {
            try
            {
                SupplierInvoiceModel SalesInvoice = new SupplierInvoiceModel();
                ApiResponseModel response = await APIServices.GetAsync("", "Sales/EditSalesPayInInvoice?SalesId=" + SalesId);
                if (response.code == 200)
                {
                    SalesInvoice = JsonConvert.DeserializeObject<SupplierInvoiceModel>(response.data.ToString());
                }
                return new JsonResult(SalesInvoice);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSalesPayInInvoice(SalesInvoiceMasterModel slaesPayInDetails)
        {
            try
            {
                ApiResponseModel SalesInvoice = await APIServices.PostAsync(slaesPayInDetails, "Sales/UpdateSalesPayInInvoice");
                if (SalesInvoice.code == 200)
                {
                    return Ok(new { Message = string.Format(SalesInvoice.message), Code = SalesInvoice.code });
                }
                else
                {
                    return Ok(new { Message = string.Format(SalesInvoice.message), Code = SalesInvoice.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSalesPayInInvoice(Guid SalesId)
        {
            try
            {
                ApiResponseModel SalesInvoice = await APIServices.PostAsync("", "Sales/DeleteSalesPayInInvoice?SalesId=" + SalesId);
                if (SalesInvoice.code == 200)
                {
                    return Ok(new { Message = string.Format(SalesInvoice.message), Code = SalesInvoice.code });
                }
                else
                {
                    return Ok(new { Message = string.Format(SalesInvoice.message), Code = SalesInvoice.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<IActionResult> ExportSalesPaymentInvoiceToPdf(InvoiceReportModel invoiceReport)
        {
            try
            {
                ApiResponseModel response = await APIServices.PostAsync(invoiceReport, "Sales/SalesInvoicePaymentPdfReport");
                if (response.code == 200)
                {
                    var SalesDetails = JsonConvert.DeserializeObject<SalesInvoiceTotalAmount>(response.data.ToString());

                    string FormatIndianCurrency(decimal amount)
                    {
                        var cultureInfo = new CultureInfo("en-IN");
                        var numberFormat = cultureInfo.NumberFormat;
                        numberFormat.CurrencySymbol = "₹";
                        numberFormat.CurrencyGroupSizes = new[] { 3, 2 };
                        numberFormat.CurrencyDecimalDigits = 2;
                        numberFormat.CurrencyGroupSeparator = ",";
                        numberFormat.CurrencyDecimalSeparator = ".";
                        return amount.ToString("C", numberFormat);
                    }

                    var document = new Aspose.Pdf.Document
                    {
                        PageInfo = new PageInfo { Margin = new MarginInfo(20, 20, 20, 20) }
                    };

                    var pdfPage = document.Pages.Add();


                    Aspose.Pdf.Table secondTable = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "50% 50%",
                        DefaultCellPadding = new MarginInfo(2, 2, 2, 2),
                        Border = new BorderInfo(BorderSide.All, 1f),
                        DefaultCellBorder = new BorderInfo(BorderSide.None),
                    };

                    var secondTableHeaderRow = secondTable.Rows.Add();
                    secondTableHeaderRow.Cells.Add("Supplier");
                    secondTableHeaderRow.Cells.Add("Company");

                    foreach (var cell in secondTableHeaderRow.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                    }

                    for (int i = 1; i < secondTableHeaderRow.Cells.Count; i++)
                    {
                        secondTableHeaderRow.Cells[i].Border = new BorderInfo(BorderSide.Left, 1f);
                    }

                    var secondTableRow1 = secondTable.Rows.Add();
                    secondTableRow1.Cells.Add(invoiceReport.SupplierName ?? string.Empty);
                    secondTableRow1.Cells.Add(invoiceReport.CompanyName ?? string.Empty);

                    foreach (var cell in secondTableRow1.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                    }

                    for (int i = 1; i < secondTableRow1.Cells.Count; i++)
                    {
                        secondTableRow1.Cells[i].Border = new BorderInfo(BorderSide.Left, 1f);
                    }

                    pdfPage.Paragraphs.Add(secondTable);

                    pdfPage.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment("\n\n"));


                    // Table 2
                    Aspose.Pdf.Table newTable = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "33% 33% 34%",
                        DefaultCellPadding = new MarginInfo(2, 2, 2, 2),
                        Border = new BorderInfo(BorderSide.All, 1f),
                        DefaultCellBorder = new BorderInfo(BorderSide.None),
                    };

                    var newTableHeaderRow = newTable.Rows.Add();
                    newTableHeaderRow.Cells.Add("Credit");
                    newTableHeaderRow.Cells.Add("Debit");
                    newTableHeaderRow.Cells.Add("Net Balance");

                    foreach (var cell in newTableHeaderRow.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                    }

                    for (int i = 1; i < newTableHeaderRow.Cells.Count; i++)
                    {
                        newTableHeaderRow.Cells[i].Border = new BorderInfo(BorderSide.Left, 1f);
                    }

                    var newTableRow1 = newTable.Rows.Add();
                    var creditCell = newTableRow1.Cells.Add(FormatIndianCurrency(SalesDetails.TotalCreadit));
                    var debitCell = newTableRow1.Cells.Add(FormatIndianCurrency(SalesDetails.TotalPurchase));

                    creditCell.DefaultCellTextState = new TextState { FontStyle = FontStyles.Bold };
                    debitCell.DefaultCellTextState = new TextState { FontStyle = FontStyles.Bold };

                    var netBalanceCell = newTableRow1.Cells.Add(FormatIndianCurrency(SalesDetails.TotalPending));
                    netBalanceCell.Alignment = HorizontalAlignment.Right;
                    netBalanceCell.DefaultCellTextState = new TextState
                    {
                        ForegroundColor = Aspose.Pdf.Color.FromRgb(System.Drawing.Color.Green),
                        FontStyle = FontStyles.Bold
                    };


                    for (int i = 1; i < newTableRow1.Cells.Count; i++)
                    {
                        newTableRow1.Cells[i].Border = new BorderInfo(BorderSide.Left, 1f);
                    }

                    foreach (var cell in newTableRow1.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                    }

                    pdfPage.Paragraphs.Add(newTable);

                    pdfPage.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment("\n\n"));

                    // Table 3

                    Aspose.Pdf.Table table = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "30% 15% 25% 15% 15%",
                        DefaultCellPadding = new MarginInfo(2, 2, 2, 2),
                        Border = new BorderInfo(BorderSide.None),
                        DefaultCellBorder = new BorderInfo(BorderSide.None),
                    };

                    var headerRow = table.Rows.Add();
                    headerRow.Cells.Add("Invoice No");
                    headerRow.Cells.Add("Date");
                    headerRow.Cells.Add("Supplier");
                    headerRow.Cells.Add("Credit");
                    headerRow.Cells.Add("Debit");

                    foreach (var cell in headerRow.Cells)
                    {
                        cell.BackgroundColor = Aspose.Pdf.Color.Black;
                        var fragment = cell.Paragraphs[0] as TextFragment;
                        if (fragment != null)
                        {
                            fragment.TextState.ForegroundColor = Aspose.Pdf.Color.White;
                        }
                    }

                    foreach (var item in SalesDetails.SalesInvoiceList)
                    {
                        var row = table.Rows.Add();
                        string cellValue;
                        if (item.SalesInvoiceNo == "PayIn")
                        {
                            var description = (string.IsNullOrEmpty(item.Description) ? "" : " (" + item.Description + ")");

                            cellValue = item.SalesInvoiceNo + description;
                        }
                        else
                        {
                            var invoiceType = (string.IsNullOrEmpty(item.InvoiceType) ? "" : " (" + item.InvoiceType + ")");
                            cellValue = item.SalesInvoiceNo + invoiceType;
                        }
                        row.Cells.Add(cellValue != "" ? cellValue : "");
                        row.Cells.Add(item.Date?.ToString("dd-MM-yyyy"));
                        row.Cells.Add(item.SupplierName);
                        if (item.SalesInvoiceNo == "PayIn" || item.InvoiceType == "Sales Return" || item.InvoiceType == "Credit Note")
                        {
                            row.Cells.Add("");
                            row.Cells.Add(FormatIndianCurrency(item.TotalAmount));
                        }
                        else
                        {
                            row.Cells.Add(FormatIndianCurrency(item.TotalAmount));
                            row.Cells.Add("");
                        }
                        var backgroundColor = table.Rows.Count % 2 == 0 ? Aspose.Pdf.Color.LightGray : Aspose.Pdf.Color.White;
                        foreach (var cell in row.Cells)
                        {
                            cell.BackgroundColor = backgroundColor;
                        }
                    }
                    var footerRow = table.Rows.Add();
                    footerRow.Cells.Add("Total");
                    footerRow.Cells.Add("");
                    footerRow.Cells.Add("");
                    footerRow.Cells.Add(FormatIndianCurrency(SalesDetails.TotalCreadit));
                    footerRow.Cells.Add(FormatIndianCurrency(SalesDetails.TotalPurchase));

                    TextState boldTextState = new TextState
                    {
                        FontStyle = FontStyles.Bold
                    };

                    foreach (var cell in footerRow.Cells)
                    {
                        cell.DefaultCellTextState = boldTextState;
                    }

                    pdfPage.Paragraphs.Add(table);

                    using (var streamout = new MemoryStream())
                    {
                        document.Save(streamout);
                        return new FileContentResult(streamout.ToArray(), "application/pdf")
                        {
                            FileDownloadName = Guid.NewGuid() + "_SalesReportList.pdf",
                        };
                    }
                }
                return RedirectToAction("ReportDetails");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public async Task<IActionResult> ExportSalesPaymentInvoiceToExcel(InvoiceReportModel invoiceReport)
        {
            try
            {
                ApiResponseModel response = await APIServices.PostAsync(invoiceReport, "Sales/SalesInvoicePaymentPdfReport");

                if (response.code == 200)
                {
                    var SalesDetails = JsonConvert.DeserializeObject<SalesInvoiceTotalAmount>(response.data.ToString());

                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Report");

                        var row = 1;

                        // Header Row 1
                        ws.Cell(row, 1).Value = "Supplier";
                        ws.Cell(row, 2).Value = "Company";

                        var headerRange1 = ws.Range(row, 1, row, 2);
                        headerRange1.Style.Font.Bold = true;
                        headerRange1.Style.Fill.BackgroundColor = XLColor.Black;
                        headerRange1.Style.Font.FontColor = XLColor.White;
                        headerRange1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        headerRange1.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        headerRange1.Style.Border.BottomBorderColor = XLColor.Black;
                        headerRange1.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        headerRange1.Style.Border.RightBorderColor = XLColor.Black;

                        row++;
                        ws.Cell(row, 1).Value = invoiceReport.SupplierName ?? string.Empty;
                        ws.Cell(row, 2).Value = invoiceReport.CompanyName ?? string.Empty;

                        var dataRange1 = ws.Range(row, 1, row, 2);
                        dataRange1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        dataRange1.Style.Font.Bold = true;
                        dataRange1.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        dataRange1.Style.Border.BottomBorderColor = XLColor.Black;
                        dataRange1.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        dataRange1.Style.Border.LeftBorderColor = XLColor.Black;
                        dataRange1.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        dataRange1.Style.Border.RightBorderColor = XLColor.Black;

                        row += 2;

                        // Table-2

                        ws.Cell(row, 1).Value = "Credit";
                        ws.Cell(row, 2).Value = "Debit";
                        ws.Cell(row, 3).Value = "Net Balance";

                        var headerRange = ws.Range(row, 1, row, 3);
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.BackgroundColor = XLColor.Gray;
                        headerRange.Style.Font.FontColor = XLColor.White;
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        headerRange.Style.Border.BottomBorderColor = XLColor.Black;
                        headerRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        headerRange.Style.Border.RightBorderColor = XLColor.Black;

                        row++;
                        ws.Cell(row, 1).Value = SalesDetails.TotalCreadit;
                        ws.Cell(row, 2).Value = SalesDetails.TotalPurchase;
                        ws.Cell(row, 3).Value = SalesDetails.TotalPending;

                        var dataRange = ws.Range(row, 1, row, 3);
                        dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(row, 1).Style.Font.Bold = true;
                        ws.Cell(row, 2).Style.Font.Bold = true;
                        ws.Cell(row, 3).Style.Font.Bold = true;
                        ws.Cell(row, 3).Style.Font.FontColor = XLColor.Green;
                        dataRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        dataRange.Style.Border.BottomBorderColor = XLColor.Black;
                        dataRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        dataRange.Style.Border.LeftBorderColor = XLColor.Black;
                        dataRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        dataRange.Style.Border.RightBorderColor = XLColor.Black;

                        // Header Row 3
                        row += 2;

                        ws.Cell(row, 1).Value = "SalesInvoice No";
                        ws.Cell(row, 2).Value = "Date";
                        ws.Cell(row, 3).Value = "Supplier";
                        ws.Cell(row, 4).Value = "Credit";
                        ws.Cell(row, 5).Value = "Debit";

                        var headerRange2 = ws.Range(row, 1, row, 5);
                        headerRange2.Style.Font.Bold = true;
                        headerRange2.Style.Fill.BackgroundColor = XLColor.Black;
                        headerRange2.Style.Font.FontColor = XLColor.White;
                        headerRange2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        row++;


                        foreach (var item in SalesDetails.SalesInvoiceList)
                        {
                            string cellValue;

                            if (item.SalesInvoiceNo == "PayIn")
                            {
                                var description = (string.IsNullOrEmpty(item.Description) ? "" : " (" + item.Description + ")");
                                cellValue = item.SalesInvoiceNo + description;
                            }
                            else
                            {
                                var invoiceType = (string.IsNullOrEmpty(item.InvoiceType) ? "" : " (" + item.InvoiceType + ")");
                                cellValue = item.SalesInvoiceNo + invoiceType;
                            }
                            ws.Cell(row, 1).Value = cellValue;
                            ws.Cell(row, 2).Value = item.Date?.ToString("dd-MM-yyyy") ?? string.Empty;
                            ws.Cell(row, 3).Value = item.SupplierName ?? string.Empty;
                            if (item.SalesInvoiceNo == "PayIn" || item.InvoiceType == "Sales Return" || item.InvoiceType == "Credit Note")
                            {
                                ws.Cell(row, 4).Value = "";
                                ws.Cell(row, 5).Value = item.TotalAmount;
                            }
                            else
                            {

                                ws.Cell(row, 4).Value = item.TotalAmount;
                                ws.Cell(row, 5).Value = "";
                            }
                            row++;
                        }

                        ws.Cell(row, 1).Value = "Total";
                        ws.Cell(row, 2).Value = string.Empty;
                        ws.Cell(row, 3).Value = string.Empty;
                        ws.Cell(row, 4).Value = SalesDetails.TotalCreadit;
                        ws.Cell(row, 5).Value = SalesDetails.TotalPurchase;

                        var footerRange2 = ws.Range(row, 1, row, 5);
                        footerRange2.Style.Font.Bold = true;
                        footerRange2.Style.Fill.BackgroundColor = XLColor.Black;
                        footerRange2.Style.Font.FontColor = XLColor.White;

                        using (var stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            var fileName = $"{Guid.NewGuid()}_SalesReportList.xlsx";
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                        }
                    }
                }
                return RedirectToAction("ReportDetails");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ExportSalesInvoiceNetReportToPDF(InvoiceReportModel SalesPaymentReport)
        {
            try
            {
                ApiResponseModel response = await APIServices.PostAsync(SalesPaymentReport, "Sales/SalesInvoicePdfReport");

                if (response.code == 200)
                {
                    string FormatIndianCurrency(decimal amount)
                    {
                        var cultureInfo = new CultureInfo("en-IN");
                        var numberFormat = cultureInfo.NumberFormat;
                        numberFormat.CurrencySymbol = "₹";
                        numberFormat.CurrencyGroupSizes = new[] { 3, 2 };
                        numberFormat.CurrencyDecimalDigits = 2;
                        numberFormat.CurrencyGroupSeparator = ",";
                        numberFormat.CurrencyDecimalSeparator = ".";
                        return amount.ToString("C", numberFormat);
                    }

                    var SalesInvoiceDetails = JsonConvert.DeserializeObject<SalesInvoiceTotalAmount>(response.data.ToString());

                    var document = new Aspose.Pdf.Document
                    {
                        PageInfo = new PageInfo { Margin = new MarginInfo(20, 20, 20, 20) }
                    };

                    var pdfPage = document.Pages.Add();

                    // Table 1
                    Aspose.Pdf.Table secondTable = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "50% 50%",
                        DefaultCellPadding = new MarginInfo(2, 2, 2, 2),
                        Border = new BorderInfo(BorderSide.All, 1f),
                        DefaultCellBorder = new BorderInfo(BorderSide.None),
                    };

                    var secondTableHeaderRow = secondTable.Rows.Add();
                    secondTableHeaderRow.Cells.Add("Supplier");
                    secondTableHeaderRow.Cells.Add("Company");

                    foreach (var cell in secondTableHeaderRow.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                    }

                    for (int i = 1; i < secondTableHeaderRow.Cells.Count; i++)
                    {
                        secondTableHeaderRow.Cells[i].Border = new BorderInfo(BorderSide.Left, 1f);
                    }

                    var secondTableRow1 = secondTable.Rows.Add();
                    secondTableRow1.Cells.Add(SalesPaymentReport.SupplierName ?? string.Empty);
                    secondTableRow1.Cells.Add(SalesPaymentReport.CompanyName ?? string.Empty);

                    foreach (var cell in secondTableRow1.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                    }

                    for (int i = 1; i < secondTableRow1.Cells.Count; i++)
                    {
                        secondTableRow1.Cells[i].Border = new BorderInfo(BorderSide.Left, 1f);
                    }

                    pdfPage.Paragraphs.Add(secondTable);

                    pdfPage.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment("\n\n"));

                    // Table 2
                    Aspose.Pdf.Table newTable = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "33% 33% 34%",
                        DefaultCellPadding = new MarginInfo(2, 2, 2, 2),
                        Border = new BorderInfo(BorderSide.All, 1f),
                        DefaultCellBorder = new BorderInfo(BorderSide.None),
                    };

                    var newTableHeaderRow = newTable.Rows.Add();
                    newTableHeaderRow.Cells.Add("Net Balance");

                    foreach (var cell in newTableHeaderRow.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                    }

                    for (int i = 1; i < newTableHeaderRow.Cells.Count; i++)
                    {
                        newTableHeaderRow.Cells[i].Border = new BorderInfo(BorderSide.Left, 1f);
                    }

                    var newTableRow1 = newTable.Rows.Add();

                    var netBalanceCell = newTableRow1.Cells.Add(FormatIndianCurrency(SalesInvoiceDetails.TotalPending));
                    netBalanceCell.Alignment = HorizontalAlignment.Right;
                    netBalanceCell.DefaultCellTextState = new TextState
                    {
                        ForegroundColor = Aspose.Pdf.Color.FromRgb(System.Drawing.Color.Green),
                        FontStyle = FontStyles.Bold
                    };


                    for (int i = 1; i < newTableRow1.Cells.Count; i++)
                    {
                        newTableRow1.Cells[i].Border = new BorderInfo(BorderSide.Left, 1f);
                    }

                    foreach (var cell in newTableRow1.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                    }

                    pdfPage.Paragraphs.Add(newTable);

                    pdfPage.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment("\n\n"));

                    // Table 3
                    var table = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "70% 30%",
                        DefaultCellPadding = new MarginInfo(2, 2, 2, 2),
                        Border = new BorderInfo(BorderSide.None),
                        DefaultCellBorder = new BorderInfo(BorderSide.None),
                    };

                    var headerRow = table.Rows.Add();
                    headerRow.Cells.Add("Supplier");
                    headerRow.Cells.Add("Net Total");

                    foreach (var cell in headerRow.Cells)
                    {
                        cell.BackgroundColor = Aspose.Pdf.Color.Black;
                        var fragment = cell.Paragraphs[0] as TextFragment;
                        if (fragment != null)
                        {
                            fragment.TextState.ForegroundColor = Aspose.Pdf.Color.White;
                        }
                    }

                    decimal yougavetotal = 0;
                    decimal yougettotal = 0;
                    decimal nettotal = 0;
                    decimal netbalance = 0;

                    foreach (var item in SalesInvoiceDetails.SalesInvoiceList)
                    {
                        var row = table.Rows.Add();
                        row.Cells.Add(item.SupplierName);

                        yougettotal += item.NonPayOutTotalAmount;
                        yougavetotal += item.PayOutTotalAmount;
                        netbalance = item.NonPayOutTotalAmount - item.PayOutTotalAmount;
                        row.Cells.Add(FormatIndianCurrency(netbalance));

                        var backgroundColor = table.Rows.Count % 2 == 0 ? Aspose.Pdf.Color.LightGray : Aspose.Pdf.Color.White;
                        foreach (var cell in row.Cells)
                        {
                            cell.BackgroundColor = backgroundColor;
                        }
                    }
                    var footerRow = table.Rows.Add();
                    footerRow.Cells.Add("Total");
                    footerRow.Cells.Add(FormatIndianCurrency(SalesInvoiceDetails.TotalPending));

                    TextState boldTextState = new TextState
                    {
                        FontStyle = FontStyles.Bold
                    };

                    foreach (var cell in footerRow.Cells)
                    {
                        cell.DefaultCellTextState = boldTextState;
                    }
                    pdfPage.Paragraphs.Add(table);

                    using (var streamout = new MemoryStream())
                    {
                        document.Save(streamout);
                        return new FileContentResult(streamout.ToArray(), "application/pdf")
                        {
                            FileDownloadName = Guid.NewGuid() + "_SalesInvoiceNetReport.pdf",
                        };
                    }
                }
                return RedirectToAction("ReportDetails");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public async Task<IActionResult> ExportSalesInvoiceNetReportToExcel(InvoiceReportModel SalesPaymentReport)
        {
            try
            {
                ApiResponseModel response = await APIServices.PostAsync(SalesPaymentReport, "Sales/SalesInvoicePdfReport");

                if (response.code == 200)
                {
                    var SalesInvoiceDetails = JsonConvert.DeserializeObject<SalesInvoiceTotalAmount>(response.data.ToString());

                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Report");

                        int row = 1;

                        // Table-1
                        ws.Cell(row, 1).Value = "Supplier";
                        ws.Cell(row, 2).Value = "Company";

                        var headerRangeNewTable = ws.Range(row, 1, row, 2);
                        headerRangeNewTable.Style.Font.Bold = true;
                        headerRangeNewTable.Style.Fill.BackgroundColor = XLColor.Gray;
                        headerRangeNewTable.Style.Font.FontColor = XLColor.White;
                        headerRangeNewTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        headerRangeNewTable.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        headerRangeNewTable.Style.Border.BottomBorderColor = XLColor.Black;
                        headerRangeNewTable.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        headerRangeNewTable.Style.Border.RightBorderColor = XLColor.Black;

                        row++;
                        ws.Cell(row, 1).Value = SalesPaymentReport.SupplierName ?? string.Empty;
                        ws.Cell(row, 2).Value = SalesPaymentReport.CompanyName ?? string.Empty;

                        var dataRangeNewTable = ws.Range(row, 1, row, 2);
                        dataRangeNewTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(row, 1).Style.Font.Bold = true;
                        ws.Cell(row, 2).Style.Font.Bold = true;
                        dataRangeNewTable.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        dataRangeNewTable.Style.Border.BottomBorderColor = XLColor.Black;
                        dataRangeNewTable.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        dataRangeNewTable.Style.Border.LeftBorderColor = XLColor.Black;
                        dataRangeNewTable.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        dataRangeNewTable.Style.Border.RightBorderColor = XLColor.Black;


                        row += 2;

                        // Table-2
                        ws.Cell(row, 1).Value = "Net Balance";

                        var headerRange1 = ws.Range(row, 1, row, 1);
                        headerRange1.Style.Font.Bold = true;
                        headerRange1.Style.Fill.BackgroundColor = XLColor.Gray;
                        headerRange1.Style.Font.FontColor = XLColor.White;
                        headerRange1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        headerRange1.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        headerRange1.Style.Border.BottomBorderColor = XLColor.Black;
                        headerRange1.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        headerRange1.Style.Border.RightBorderColor = XLColor.Black;

                        row++;
                        ws.Cell(row, 1).Value = SalesInvoiceDetails.TotalPending;

                        var dataRange1 = ws.Range(row, 1, row, 1);
                        dataRange1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(row, 1).Style.Font.Bold = true;
                        ws.Cell(row, 1).Style.Font.FontColor = XLColor.Green;
                        dataRange1.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        dataRange1.Style.Border.BottomBorderColor = XLColor.Black;
                        dataRange1.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        dataRange1.Style.Border.LeftBorderColor = XLColor.Black;
                        dataRange1.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        dataRange1.Style.Border.RightBorderColor = XLColor.Black;

                        // Table-3
                        row += 2;

                        ws.Cell(row, 1).Value = "Supplier";
                        ws.Cell(row, 2).Value = "Net Total";

                        var headerRange3 = ws.Range(row, 1, row, 2);
                        headerRange3.Style.Font.Bold = true;
                        headerRange3.Style.Fill.BackgroundColor = XLColor.Gray;
                        headerRange3.Style.Font.FontColor = XLColor.White;
                        headerRange3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        headerRange3.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        headerRange3.Style.Border.BottomBorderColor = XLColor.Black;
                        headerRange3.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        headerRange3.Style.Border.RightBorderColor = XLColor.Black;

                        decimal yougavetotal = 0;
                        decimal yougettotal = 0;
                        decimal nettotal = 0;
                        decimal netbalance = 0;

                        row++;
                        foreach (var item in SalesInvoiceDetails.SalesInvoiceList)
                        {
                            ws.Cell(row, 1).Value = item.SupplierName;
                            yougavetotal += item.PayOutTotalAmount;
                            yougettotal += item.NonPayOutTotalAmount;
                            netbalance = item.NonPayOutTotalAmount - item.PayOutTotalAmount;
                            ws.Cell(row, 2).Value = netbalance;
                            row++;
                        }

                        nettotal = yougettotal - yougavetotal;

                        ws.Cell(row, 1).Value = "Total";
                        ws.Cell(row, 2).Value = nettotal;

                        var footerRange3 = ws.Range(row, 1, row, 2);
                        footerRange3.Style.Fill.BackgroundColor = XLColor.Gray;
                        footerRange3.Style.Font.FontColor = XLColor.White;

                        ws.Columns().AdjustToContents();

                        using (var stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            string fileName = Guid.NewGuid() + "_SalesInvoiceNetReport.xlsx";
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                        }
                    }
                }
                return RedirectToAction("ReportDetails");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
