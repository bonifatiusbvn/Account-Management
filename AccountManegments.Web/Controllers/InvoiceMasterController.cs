using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.DataTableParameters;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.SupplierMaster;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using Irony.Parsing.Construction;
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
using System.Globalization;
using System.Reflection;
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
        public async Task<IActionResult> CreateInvoice(Guid? Id, Guid? POID)
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
                    ViewBag.EditShippingAddress = invoiceDetails.ShippingAddress;
                    ViewBag.GroupAddress = invoiceDetails.GroupAddress;
                }


                if (POID != null)
                {
                    ApiResponseModel response = await APIServices.GetAsync("", "PurchaseOrder/GetPODetailsInInvoice?POID=" + POID);
                    if (response.code == 200)
                    {
                        invoiceDetails = JsonConvert.DeserializeObject<SupplierInvoiceMasterView>(response.data.ToString());
                        int rowNumber = 0;
                        foreach (var item in invoiceDetails.ItemList)
                        {
                            item.RowNumber = rowNumber++;
                        }
                    }
                    ViewBag.POShippingAddress = invoiceDetails.PODeliveryAddress;
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

                if (Id != null)
                {
                    if (invoiceDetails.SiteGroup != "")
                    {
                        SiteGroupModel SiteGroupDetails = new SiteGroupModel();
                        ApiResponseModel res = await APIServices.GetAsync("", "SiteMaster/GetGroupDetailsByGroupName?GroupName=" + invoiceDetails.SiteGroup);
                        if (res.code == 200)
                        {
                            SiteGroupDetails = JsonConvert.DeserializeObject<SiteGroupModel>(res.data.ToString());
                            ViewBag.GroupAddresses = SiteGroupDetails.GroupAddressList;
                        }
                    }
                }
                return View(invoiceDetails);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, message = "An unexpected error occurred. Please try again." });
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
        public async Task<IActionResult> SupplierInvoiceListAction(string? searchText, string? searchBy, string? sortBy, Guid? CompanyId, Guid? SupplierId)
        {
            try
            {
                string siteIdString = UserSession.SiteId;
                Guid? SiteId = !string.IsNullOrEmpty(siteIdString) ? Guid.Parse(siteIdString) : (Guid?)null;
                List<UserCompanyListModel> CompanyData = UserSession.CompanyData;

                string apiUrl = $"SupplierInvoice/GetSupplierInvoiceList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    SupplierInvoiceList GetInvoiceList = JsonConvert.DeserializeObject<SupplierInvoiceList>(res.data.ToString());

                    if (SiteId.HasValue)
                    {
                        GetInvoiceList.InvoiceList = GetInvoiceList.InvoiceList
                            .Where(invoice => invoice.SiteId == SiteId.Value)
                            .ToList();

                        GetInvoiceList.InvoiceItemList = GetInvoiceList.InvoiceItemList
                            .Where(item => GetInvoiceList.InvoiceList.Any(invoice => invoice.Id == item.Key))
                            .ToDictionary(
                                item => item.Key,
                                item => item.Value
                            );
                    }

                    if (CompanyData != null && CompanyData.Any())
                    {
                        var companyIds = CompanyData.Select(c => c.CompanyId).ToList();

                        GetInvoiceList.InvoiceList = GetInvoiceList.InvoiceList
                            .Where(invoice => companyIds.Contains(invoice.CompanyId))
                            .ToList();

                        GetInvoiceList.InvoiceItemList = GetInvoiceList.InvoiceItemList
                            .Where(item => GetInvoiceList.InvoiceList.Any(invoice => invoice.Id == item.Key))
                            .ToDictionary(
                                item => item.Key,
                                item => item.Value
                            );
                    }
                    if (CompanyId != null)
                    {
                        GetInvoiceList.InvoiceList = GetInvoiceList.InvoiceList
                            .Where(invoice => invoice.CompanyId == CompanyId.Value)
                            .ToList();
                    }
                    if (SupplierId != null)
                    {
                        GetInvoiceList.InvoiceList = GetInvoiceList.InvoiceList
                            .Where(invoice => invoice.SupplierId == SupplierId.Value)
                            .ToList();
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

        [FormPermissionAttribute("Report & Payout-View")]
        public IActionResult PayOutInvoice()
        {
            return View();
        }

        [FormPermissionAttribute("Report & Payout-View")]
        [HttpPost]
        public async Task<IActionResult> GetInvoiceDetails(InvoiceReportModel PayOutReport)
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
                    filterType = PayOutReport.filterType,
                    SiteId = PayOutReport.SiteId,
                    SupplierId = PayOutReport.SupplierId,
                    CompanyId = PayOutReport.CompanyId,
                    GroupName = PayOutReport.GroupName,
                    SelectedYear = PayOutReport.SelectedYear,
                    startDate = PayOutReport.startDate,
                    endDate = PayOutReport.endDate,
                    SupplierName = PayOutReport.SupplierName,
                    CompanyName = PayOutReport.CompanyName,
                    TillMonth = PayOutReport.TillMonth,
                };
                List<SupplierInvoiceModel> supplierDetails = new List<SupplierInvoiceModel>();
                var jsonData = new jsonData();
                ApiResponseModel response = await APIServices.PostAsync(dataTable, "SupplierInvoice/GetInvoiceDetailsById");
                if (response.code == 200)
                {
                    jsonData = JsonConvert.DeserializeObject<jsonData>(response.data.ToString());
                    supplierDetails = JsonConvert.DeserializeObject<List<SupplierInvoiceModel>>(jsonData.data.ToString());

                    var result = new
                    {
                        TotalCredit = jsonData.TotalCredit,
                        TotalDebit = jsonData.TotalDebit,
                        draw = jsonData.draw,
                        recordsFiltered = jsonData.recordsFiltered,
                        recordsTotal = jsonData.recordsTotal,
                        data = supplierDetails
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
                bool isApproved = UserSession.FormPermisionData.Any(a => a.FormName == "Purchase  Invoice" && (a.IsApproved == true));
                var OrderDetails = HttpContext.Request.Form["SupplierItems"];
                var InsertDetails = JsonConvert.DeserializeObject<SupplierInvoiceMasterView>(OrderDetails.ToString());
                var invoicedetails = new SupplierInvoiceMasterView
                {
                    InvoiceNo = InsertDetails.InvoiceNo,
                    InvoiceType = InsertDetails.InvoiceType,
                    SiteId = InsertDetails.SiteId,
                    SupplierId = InsertDetails.SupplierId,
                    CompanyId = InsertDetails.CompanyId,
                    SupplierInvoiceNo = InsertDetails.SupplierInvoiceNo,
                    Description = InsertDetails.Description,
                    TotalDiscount = InsertDetails.TotalDiscount,
                    TotalGstamount = InsertDetails.TotalGstamount,
                    TotalAmountInvoice = InsertDetails.TotalAmountInvoice,
                    PaymentStatus = InsertDetails.PaymentStatus,
                    Tds = InsertDetails.Tds,
                    ChallanNo = InsertDetails.ChallanNo,
                    Lrno = InsertDetails.Lrno,
                    VehicleNo = InsertDetails.VehicleNo,
                    DispatchBy = InsertDetails.DispatchBy,
                    PaymentTerms = InsertDetails.PaymentTerms,
                    ShippingAddress = InsertDetails.ShippingAddress,
                    DiscountRoundoff = InsertDetails.DiscountRoundoff,
                    IsApproved = isApproved,
                    Date = InsertDetails.Date,
                    CreatedBy = InsertDetails.CreatedBy,
                    SiteGroup = InsertDetails.SiteGroup,
                    ItemList = InsertDetails.ItemList,
                    GroupAddress = InsertDetails.GroupAddress,
                    Poid = InsertDetails.Poid,
                };
                ApiResponseModel postuser = await APIServices.PostAsync(invoicedetails, "SupplierInvoice/InsertMultipleSupplierItemDetails");
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



        [FormPermissionAttribute("Report & Payout-Add")]
        [HttpPost]
        public async Task<IActionResult> InsertPayOutDetails()
        {
            try
            {
                var payout = HttpContext.Request.Form["PAYOUTDETAILS"];
                var payoutDetails = JsonConvert.DeserializeObject<List<SupplierInvoiceModel>>(payout);

                ApiResponseModel postuser = await APIServices.PostAsync(payoutDetails, "SupplierInvoice/AddSupplierInvoice");
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
        public async Task<IActionResult> DisplayPayOutInvoicePayOutInvoice()
        {
            try
            {
                List<SupplierInvoiceModel> pauoutinvoice = new List<SupplierInvoiceModel>
                {
                    new SupplierInvoiceModel()
                };
                return PartialView("~/Views/InvoiceMaster/_PayOutInvoicePartial.cshtml", pauoutinvoice);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IActionResult> CheckOpeningBalance(Guid SupplierId, Guid CompanyId)
        {
            try
            {
                string apiUrl = $"SupplierInvoice/CheckOpeningBalance?SupplierId={SupplierId}&CompanyId={CompanyId}";

                ApiResponseModel postuser = await APIServices.GetAsync("", apiUrl);
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
                return StatusCode(500, new { Message = "An error occurred while fetching invoice details.", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckSupplierInvoiceNo(SupplierInvoiceModel InvoiceData)
        {
            try
            {
                ApiResponseModel postuser = await APIServices.PostAsync(InvoiceData, "SupplierInvoice/CheckSupplierInvoiceNo");
                if (postuser.code == 200)
                {
                    return Ok(new { Data = postuser.data, Code = postuser.code });
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
        [HttpGet]
        public async Task<JsonResult> EditGroupNameListBySiteId(Guid? SiteId)
        {
            try
            {
                List<GroupMasterModel> GroupList = new List<GroupMasterModel>();
                ApiResponseModel res = await APIServices.GetAsync("", "SiteMaster/GetGroupNameListBySiteId?SiteId=" + SiteId);
                if (res.code == 200)
                {
                    GroupList = JsonConvert.DeserializeObject<List<GroupMasterModel>>(res.data.ToString());
                }
                return new JsonResult(GroupList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public async Task<IActionResult> InvoiceDetailsExportToPdf(Guid? CompanyId, Guid? SupplierId)
        {
            try
            {
                var SiteId = UserSession.SiteId;
                if (SiteId == "" || SiteId == null)
                {
                    return BadRequest("Please select a Site.");
                }
                ApiResponseModel response = await APIServices.GetAsync("", "SupplierInvoice/InvoiceDetailsExportToPdf?SiteId=" + SiteId);

                if (response.code == 200)
                {
                    string responseData = response.data.ToString();

                    var supplierDetailsList = JsonConvert.DeserializeObject<List<SupplierInvoiceMasterView>>(responseData);

                    if (CompanyId != null)
                    {
                        supplierDetailsList = supplierDetailsList.Where(e => e.CompanyId == CompanyId.Value).ToList();
                    }
                    if (SupplierId != null)
                    {
                        supplierDetailsList = supplierDetailsList.Where(e => e.SupplierId == SupplierId.Value).ToList();
                    }

                    decimal TotalAmount = 0;
                    decimal AllItemTotalAmount = 0;
                    foreach (var supplier in supplierDetailsList)
                    {
                        foreach (var item in supplier.ItemList ?? new List<POItemDetailsModel>())
                        {
                            TotalAmount = item.TotalAmount ?? 0;
                            AllItemTotalAmount += TotalAmount;
                        }
                    }
                    var document = new Aspose.Pdf.Document
                    {
                        PageInfo = new PageInfo { Margin = new MarginInfo(20, 20, 20, 20) }
                    };
                    var pdfPage = document.Pages.Add();

                    Aspose.Pdf.Text.TextFragment lineBreak = new Aspose.Pdf.Text.TextFragment("");

                    Aspose.Pdf.Table InvoiceSite = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "60% 40%",
                        DefaultCellPadding = new MarginInfo(2, 2, 2, 2),
                        Border = new BorderInfo(BorderSide.All, 1f),
                    };

                    var InvoiceSiteTableHeaderRow = InvoiceSite.Rows.Add();
                    InvoiceSiteTableHeaderRow.Cells.Add("Site Name");
                    InvoiceSiteTableHeaderRow.Cells.Add("Total Amount");
                    TextState boldTextState = new TextState
                    {
                        FontStyle = FontStyles.Bold
                    };

                    foreach (var cell in InvoiceSiteTableHeaderRow.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                        cell.DefaultCellTextState = boldTextState;
                    }

                    var InvoiceSiteBodyRow = InvoiceSite.Rows.Add();
                    InvoiceSiteBodyRow.Cells.Add(UserSession.SiteName ?? string.Empty);

                    InvoiceSiteBodyRow.Cells.Add(FormatIndianCurrency(AllItemTotalAmount));
                    foreach (var cell in InvoiceSiteBodyRow.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                    }
                    pdfPage.Paragraphs.Add(InvoiceSite);

                    pdfPage.Paragraphs.Add(lineBreak);
                    pdfPage.Paragraphs.Add(lineBreak);
                    pdfPage.Paragraphs.Add(lineBreak);
                    pdfPage.Paragraphs.Add(lineBreak);

                    foreach (var Supplier in supplierDetailsList)
                    {


                        Aspose.Pdf.Table InvoiceDetial = new Aspose.Pdf.Table
                        {
                            ColumnWidths = "25% 15% 30% 30%",
                            DefaultCellPadding = new MarginInfo(2, 2, 2, 2),
                            Border = new BorderInfo(BorderSide.All, 1f),
                        };

                        var InvoiceDetialTableHeaderRow = InvoiceDetial.Rows.Add();
                        InvoiceDetialTableHeaderRow.Cells.Add("InvoiceNo");
                        InvoiceDetialTableHeaderRow.Cells.Add("Date");
                        InvoiceDetialTableHeaderRow.Cells.Add("Supplier");
                        InvoiceDetialTableHeaderRow.Cells.Add("Company");

                        foreach (var cell in InvoiceDetialTableHeaderRow.Cells)
                        {
                            cell.Alignment = HorizontalAlignment.Center;
                            cell.DefaultCellTextState = boldTextState;
                        }

                        var InvoiceDetialBodyRow = InvoiceDetial.Rows.Add();
                        InvoiceDetialBodyRow.Cells.Add(Supplier.InvoiceNo ?? string.Empty);
                        InvoiceDetialBodyRow.Cells.Add(Supplier.Date?.ToString("dd-MM-yyyy") ?? string.Empty);
                        InvoiceDetialBodyRow.Cells.Add(Supplier.SupplierName ?? string.Empty);
                        InvoiceDetialBodyRow.Cells.Add(Supplier.CompanyName ?? string.Empty);
                        foreach (var cell in InvoiceDetialBodyRow.Cells)
                        {
                            cell.Alignment = HorizontalAlignment.Center;
                        }
                        pdfPage.Paragraphs.Add(InvoiceDetial);

                        Aspose.Pdf.Table ItemTable = new Aspose.Pdf.Table
                        {
                            ColumnWidths = "50% 25% 25% ",
                            DefaultCellPadding = new MarginInfo(2, 2, 2, 2),
                            Border = new BorderInfo(BorderSide.All, 0.5f),
                            DefaultCellBorder = new BorderInfo(BorderSide.None),
                        };

                        var ItemTableheaderRow = ItemTable.Rows.Add();
                        ItemTableheaderRow.Cells.Add("Item Name");
                        ItemTableheaderRow.Cells.Add("Quantity");
                        ItemTableheaderRow.Cells.Add("Amount");
                        foreach (var cell in ItemTableheaderRow.Cells)
                        {

                            cell.DefaultCellTextState = boldTextState;
                        }

                        int rowCount = 0;
                        decimal ItemTotalAmount = 0;
                        foreach (var item in Supplier.ItemList ?? new List<POItemDetailsModel>())
                        {
                            var itemRow = ItemTable.Rows.Add();
                            itemRow.Cells.Add(item.ItemName ?? string.Empty);
                            itemRow.Cells.Add(item.Quantity.ToString() ?? "0");
                            itemRow.Cells.Add(FormatIndianCurrency(item.TotalAmount ?? 0));

                            var backgroundColor = rowCount % 2 == 0 ? Aspose.Pdf.Color.LightGray : Aspose.Pdf.Color.White;
                            foreach (var cell in itemRow.Cells)
                            {
                                cell.BackgroundColor = backgroundColor;
                            }
                            {
                                ItemTotalAmount += item.TotalAmount ?? 0;
                            }
                            rowCount++;
                        }
                        var footerRow = ItemTable.Rows.Add();
                        footerRow.Cells.Add("Total");
                        footerRow.Cells.Add("");
                        footerRow.Cells.Add(FormatIndianCurrency(ItemTotalAmount));
                        foreach (var cell in footerRow.Cells)
                        {
                            cell.DefaultCellTextState = boldTextState;
                        }

                        pdfPage.Paragraphs.Add(ItemTable);

                        pdfPage.Paragraphs.Add(lineBreak);
                        pdfPage.Paragraphs.Add(lineBreak);

                    }

                    using (var streamout = new MemoryStream())
                    {
                        document.Save(streamout);
                        return new FileContentResult(streamout.ToArray(), "application/pdf")
                        {
                            FileDownloadName = Guid.NewGuid() + "_InvoiceDetails.pdf",
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


        private string FormatIndianCurrency(decimal amount)
        {
            var cultureInfo = new CultureInfo("en-IN");
            var numberFormat = cultureInfo.NumberFormat;
            numberFormat.CurrencySymbol = "₹";
            return amount.ToString("C", numberFormat);
        }

    }
}
