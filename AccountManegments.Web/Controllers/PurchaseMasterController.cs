using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.DBContext.Models.ViewModels.PurchaseRequest;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using Newtonsoft.Json;
using System.Reflection;
using Aspose.Pdf.Operators;
using System.Text;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using AccountManagement.API;
using System.Security.Cryptography;

namespace AccountManegments.Web.Controllers
{
    [Authorize]
    public class PurchaseMasterController : Controller
    {
        public PurchaseMasterController(WebAPI webAPI, APIServices aPIServices, IWebHostEnvironment environment, UserSession userSession)
        {
            WebAPI = webAPI;
            APIServices = aPIServices;
            Environment = environment;
            UserSession = userSession;
        }

        public WebAPI WebAPI { get; }
        public APIServices APIServices { get; }
        public IWebHostEnvironment Environment { get; }
        public UserSession UserSession { get; }


        [FormPermissionAttribute("Purchase Request-View")]
        public async Task<IActionResult> PurchaseRequestListView()
        {

            return View();

        }

        [HttpGet]
        public async Task<JsonResult> CheckPRNo()
        {
            try
            {
                ApiResponseModel Response = await APIServices.GetAsync("", "PurchaseRequest/CheckPRNo");
                string PRNo = null;

                if (Response.code == 200)
                {
                    PRNo = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(Response.data));
                }

                return new JsonResult(PRNo);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        [FormPermissionAttribute("Purchase Request-View")]
        public async Task<IActionResult> PurchaseRequestListAction(string searchText, string searchBy, string sortBy, Guid? SiteId)
        {
            try
            {
                if (SiteId != null)
                {
                    UserSession.SiteId = SiteId.ToString();
                }
                Guid? siteId = string.IsNullOrEmpty(UserSession.SiteId) ? null : new Guid(UserSession.SiteId);
                string apiUrl = $"PurchaseRequest/GetPurchaseRequestList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}&&siteId={siteId}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    List<PurchaseRequestModel> GetSiteList = JsonConvert.DeserializeObject<List<PurchaseRequestModel>>(res.data.ToString());

                    return PartialView("~/Views/PurchaseMaster/_PurchaseRequestListPartial.cshtml", GetSiteList);
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
        [FormPermissionAttribute("Purchase Request-View")]
        public async Task<JsonResult> DisplayPurchaseRequestDetails(Guid PurchaseId)
        {
            try
            {
                PurchaseRequestModel SiteDetails = new PurchaseRequestModel();
                ApiResponseModel res = await APIServices.GetAsync("", "PurchaseRequest/GetPurchaseRequestDetailsById?purchaseId=" + PurchaseId);
                if (res.code == 200)
                {
                    SiteDetails = JsonConvert.DeserializeObject<PurchaseRequestModel>(res.data.ToString());
                }
                return new JsonResult(SiteDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [FormPermissionAttribute("Purchase Request-Add")]
        [HttpPost]
        public async Task<IActionResult> CreatePurchaseRequest(PurchaseRequestModel PurchaseRequestDetails)
        {

            try
            {
                bool isApproved = UserSession.FormPermisionData.Any(a => a.FormName == "Purchase Request" && (a.IsApproved == true));

                var PurchaseRequest = new PurchaseRequestModel()
                {
                    Pid = Guid.NewGuid(),
                    ItemId = PurchaseRequestDetails.ItemId,
                    ItemDescription = PurchaseRequestDetails.ItemDescription,
                    UnitTypeId = PurchaseRequestDetails.UnitTypeId,
                    SiteId = PurchaseRequestDetails.SiteId,
                    Quantity = PurchaseRequestDetails.Quantity,
                    CreatedBy = UserSession.UserId,
                    PrNo = PurchaseRequestDetails.PrNo,
                    IsApproved = isApproved,
                    SiteAddress = PurchaseRequestDetails.SiteAddress == "--Select Site Address--" ? null : PurchaseRequestDetails.SiteAddress,
                    SiteAddressId = PurchaseRequestDetails.SiteAddressId,
                    ItemName = PurchaseRequestDetails.ItemName,
                };

                ApiResponseModel postUser = await APIServices.PostAsync(PurchaseRequest, "PurchaseRequest/AddPurchaseRequestDetails");
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

        [HttpPost]
        public async Task<IActionResult> UpdatePurchaseRequestDetails(PurchaseRequestModel updatePurchaseRequest)
        {
            try
            {
                ApiResponseModel postUser = await APIServices.PostAsync(updatePurchaseRequest, "PurchaseRequest/UpdatePurchaseRequestDetails");
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

        [HttpPost]
        public async Task<IActionResult> DeletePurchaseRequest(Guid PurchaseId)
        {
            try
            {
                ApiResponseModel postuser = await APIServices.PostAsync("", "PurchaseRequest/DeletePurchaseRequest?PurchaseId=" + PurchaseId);
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

        [FormPermissionAttribute("Purchase Request-Edit")]
        [HttpPost]
        public async Task<IActionResult> PurchaseRequestIsApproved(Guid PurchaseId)
        {
            try
            {

                ApiResponseModel postuser = await APIServices.PostAsync("", "PurchaseRequest/PurchaseRequestIsApproved?PurchaseId=" + PurchaseId);
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

        public async Task<IActionResult> CreatePurchaseOrder(Guid? id)
        {
            try
            {
                PurchaseOrderMasterView response = new PurchaseOrderMasterView();
                if (id != null)
                {
                    ApiResponseModel res = await APIServices.GetAsync("", "PurchaseOrder/GetPurchaseOrderDetailsById?POId=" + id);
                    if (res.code == 200)
                    {
                        response = JsonConvert.DeserializeObject<PurchaseOrderMasterView>(res.data.ToString());
                        int rowNumber = 0;
                        foreach (var item in response.ItemList)
                        {
                            item.RowNumber = rowNumber++;
                        }
                    }
                }
                return View(response);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        [FormPermissionAttribute("Purchase Orders-Add")]
        [HttpPost]
        public async Task<IActionResult> InsertMultiplePurchaseOrderDetails()
        {
            try
            {
                bool isApproved = UserSession.FormPermisionData.Any(a => a.FormName == "Purchase Orders" && (a.IsApproved == true));
                var OrderDetails = HttpContext.Request.Form["PODETAILS"];
                var PurchaseOrderDetails = JsonConvert.DeserializeObject<PurchaseOrderMasterView>(OrderDetails.ToString());
                var podetails = new PurchaseOrderMasterView
                {
                    Poid = PurchaseOrderDetails.Poid,
                    SiteId = PurchaseOrderDetails.SiteId,
                    Date = PurchaseOrderDetails.Date,
                    FromSupplierId = PurchaseOrderDetails.FromSupplierId,
                    ToCompanyId = PurchaseOrderDetails.ToCompanyId,
                    TotalAmount = PurchaseOrderDetails.TotalAmount,
                    Description = PurchaseOrderDetails.Description,
                    DeliveryShedule = PurchaseOrderDetails.DeliveryShedule,
                    TotalDiscount = PurchaseOrderDetails.TotalDiscount,
                    TotalGstamount = PurchaseOrderDetails.TotalGstamount,
                    BillingAddress = PurchaseOrderDetails.BillingAddress,
                    ContactName = PurchaseOrderDetails.ContactName,
                    ContactNumber = PurchaseOrderDetails.ContactNumber,
                    IsDeleted = false,
                    IsApproved = isApproved,
                    Terms = PurchaseOrderDetails.Terms,
                    DispatchBy = PurchaseOrderDetails.DispatchBy,
                    PaymentTerms = PurchaseOrderDetails.PaymentTerms,
                    PaymentTermsId = PurchaseOrderDetails.PaymentTermsId,
                    BuyersPurchaseNo = PurchaseOrderDetails.BuyersPurchaseNo,
                    CreatedBy = PurchaseOrderDetails.CreatedBy,
                    ItemOrderlist =  PurchaseOrderDetails.ItemOrderlist,
                    ShippingAddressList = PurchaseOrderDetails.ShippingAddressList,
                };
                ApiResponseModel postuser = await APIServices.PostAsync(podetails, "PurchaseOrder/InsertMultiplePurchaseOrderDetails");
                if (postuser.code == 200)
                {
                    return Ok(new { postuser.message, postuser.data, postuser.code });
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

        [FormPermissionAttribute("Purchase Orders-View")]
        public IActionResult PurchaseOrderList()
        {
            return View();
        }
        public async Task<IActionResult> PurchaseOrderListView(string searchText, string searchBy, string sortBy)
        {
            try
            {

                string apiUrl = $"PurchaseOrder/GetPurchaseOrderList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    List<PurchaseOrderView> GetPOList = JsonConvert.DeserializeObject<List<PurchaseOrderView>>(res.data.ToString());

                    return PartialView("~/Views/PurchaseMaster/_PurchaseOrderListPartial.cshtml", GetPOList);
                }
                else
                {
                    return BadRequest(new { Message = "Failed to retrieve user list." });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new { Message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DisplayPurchaseOrderDetails(string Id)
        {
            try
            {

                return RedirectToAction("CreatePurchaseOrder", new { id = Id });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMultiplePurchaseOrderDetails()
        {
            try
            {
                var OrderDetails = HttpContext.Request.Form["PODETAILS"];
                var UpdateDetails = JsonConvert.DeserializeObject<PurchaseOrderMasterView>(OrderDetails.ToString());
                ApiResponseModel postuser = await APIServices.PostAsync(UpdateDetails, "PurchaseOrder/UpdateMultiplePurchaseOrderDetails");
                if (postuser.code == 200)
                {
                    return Ok(new { postuser.message, postuser.data, postuser.code });
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
        public async Task<IActionResult> DeletePurchaseOrderDetails(Guid POId)
        {
            try
            {
                ApiResponseModel postuser = await APIServices.PostAsync("", "PurchaseOrder/DeletePurchaseOrderDetails?POId=" + POId);
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

        [FormPermissionAttribute("Purchase Orders-View")]
        public async Task<IActionResult> POListView()
        {
            return View();
        }

        public async Task<IActionResult> POListAction(string searchText, string searchBy, string sortBy)
        {
            try
            {
                string siteIdString = UserSession.SiteId;
                Guid? SiteId = !string.IsNullOrEmpty(siteIdString) ? Guid.Parse(siteIdString) : (Guid?)null;
                string apiUrl = $"PurchaseOrder/GetPurchaseOrderList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    List<PurchaseOrderView> GetPOList = JsonConvert.DeserializeObject<List<PurchaseOrderView>>(res.data.ToString());
                    if (SiteId != null)
                    {
                        GetPOList = GetPOList.Where(a => a.SiteId == SiteId).ToList();
                    }
                    return PartialView("~/Views/PurchaseMaster/_POListPartial.cshtml", GetPOList);
                }
                else
                {
                    return BadRequest(new { Message = "Failed to retrieve Purchase Order list" });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new { Message = $"An error occurred: {ex.Message}" });
            }
        }

        public async Task<IActionResult> DisplayPODetails(Guid POId)
        {
            try
            {
                PurchaseOrderMasterView order = new PurchaseOrderMasterView();
                ApiResponseModel response = await APIServices.GetAsync("", "PurchaseOrder/GetPurchaseOrderDetailsById?POId=" + POId);
                if (response.code == 200)
                {
                    order = JsonConvert.DeserializeObject<PurchaseOrderMasterView>(response.data.ToString());
                    var number = order.TotalAmount;
                    var totalAmountInWords = NumberToWords(number);
                    ViewData["TotalAmountInWords"] = totalAmountInWords + " " + "Only";
                }
                return View(order);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string NumberToWords(decimal inputNumber)
        {
            int inputNo = (int)Math.Floor(inputNumber);
            decimal fraction = inputNumber - inputNo;

            if (inputNo == 0)
                return "Zero";

            int[] numbers = new int[4];
            int first = 0;
            int u, h, t;
            StringBuilder sb = new StringBuilder();

            if (inputNo < 0)
            {
                sb.Append("Minus ");
                inputNo = -inputNo;
            }

            string[] words0 = {"", "One ", "Two ", "Three ", "Four ",
        "Five ", "Six ", "Seven ", "Eight ", "Nine "};
            string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ",
        "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen "};
            string[] words2 = {"Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ",
        "Seventy ", "Eighty ", "Ninety "};
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };

            numbers[0] = inputNo % 1000;
            numbers[1] = inputNo / 1000;
            numbers[2] = inputNo / 100000;
            numbers[1] = numbers[1] - 100 * numbers[2];
            numbers[3] = inputNo / 10000000;
            numbers[2] = numbers[2] - 100 * numbers[3];
            for (int i = 3; i > 0; i--)
            {
                if (numbers[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (numbers[i] == 0) continue;
                u = numbers[i] % 10;
                t = numbers[i] / 10;
                h = numbers[i] / 100;
                t = t - 10 * h;
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            if (fraction > 0)
            {
                sb.Append("and ");
                int fractionInt = (int)Math.Round(fraction * 100);
                sb.Append(NumberToWords(fractionInt) + " " + "Paisa ");
            }

            return sb.ToString().TrimEnd();
        }


        public async Task<IActionResult> ExportToPdf(Guid POId)
        {
            try
            {
                PurchaseOrderMasterView orderDetails = new PurchaseOrderMasterView();
                ApiResponseModel response = await APIServices.GetAsync("", "PurchaseOrder/GetPurchaseOrderDetailsById?POId=" + POId);

                if (response.code == 200)
                {
                    orderDetails = JsonConvert.DeserializeObject<PurchaseOrderMasterView>(response.data.ToString());

                    var htmlContent = GetHtmlContentForPdf(orderDetails);

                    var document = new Document
                    {
                        PageInfo = new PageInfo { Margin = new MarginInfo(25, 25, 25, 40) }
                    };

                    var pdfPage = document.Pages.Add();

                    var textBuilder = new TextFragment();
                    textBuilder.Text = htmlContent;

                    pdfPage.Paragraphs.Add(textBuilder);

                    using (var streamout = new MemoryStream())
                    {
                        document.Save(streamout);

                        return new FileContentResult(streamout.ToArray(), "application/pdf")
                        {
                            FileDownloadName = Guid.NewGuid() + "_POInvoice.pdf",
                        };
                    }
                }
                return RedirectToAction("POListView");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string GetHtmlContentForPdf(PurchaseOrderMasterView orderDetails)
        {
            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append($"<div class=\"row justify-content-center\" id=\"displayPODetail\">");
            htmlBuilder.Append($"<div class=\"col-xxl-9\">");
            htmlBuilder.Append($"<div class=\"card border\">");
            htmlBuilder.Append($"<div class=\"row\">");
            htmlBuilder.Append($"<div class=\"card-body\">");
            htmlBuilder.Append($"<div class=\"d-flex\">");
            htmlBuilder.Append($"<div class=\"flex-grow-1 col-5 border\">");
            htmlBuilder.Append($"<p class=\"text-muted text-uppercase\"><b>D. M. Infra</b></p>");
            htmlBuilder.Append($"<p class=\"text-muted\">SF- 203, Peridot Complex, Varacha</p>");
            htmlBuilder.Append($"<p class=\"text-muted\">Area- Surat,Gujarat 390020</p>");
            htmlBuilder.Append($"<p class=\"text-muted\">State- Gujarat,Code:24</p>");
            htmlBuilder.Append($"<p class=\"text-muted\">GST.NO- 24AAKCB0366M1ZJ</p>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"<div class=\"flex-grow-1 col-4\">");
            htmlBuilder.Append($"<div class=\"flex-shrink-0 mt-sm-0 mt-3 border\">");
            htmlBuilder.Append($"<label class=\"form-label\">Purchase Order Id</label>");
            htmlBuilder.Append("<p class=\"text-muted text-uppercase\"><b>" + orderDetails.Poid + "</b></p>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"<div class=\"flex-shrink-0 mt-sm-0 mt-3 border\">");
            htmlBuilder.Append($"<label class=\"form-label\">Mode/Terms of Payment</label>");
            htmlBuilder.Append($"<p><span class=\"text-muted\"><b>100% against Pi before dispatch</b></span></p>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"<div class=\"flex-shrink-0 mt-sm-0 mt-3 border\">");
            htmlBuilder.Append($"<label class=\"form-label\">Buyer's Ref./Order No.</label>");
            htmlBuilder.Append($"<p class=\"text-muted text-uppercase\"><b>" + orderDetails.Poid + "</b></p>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"<div class=\"flex-grow-1 col-3\">");
            htmlBuilder.Append($"<!-- Seller Invoice Dates -->");
            htmlBuilder.Append($"<div class=\"flex-shrink-0 mt-sm-0 mt-3 border\">");
            htmlBuilder.Append($"<label class=\"form-label\">Dated</label>");
            htmlBuilder.Append($"<p><span class=\"text-muted fw-normal\">" + orderDetails.Date + "</span></p>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"<div class=\"flex-shrink-0 mt-sm-0 mt-3 border\">");
            htmlBuilder.Append($"<label class=\"form-label\">Mode/Terms of Payment</label>");
            htmlBuilder.Append($"<p><span class=\"text-muted\"><b>100% against Pi before dispatch</b></span></p>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"<div class=\"flex-shrink-0 mt-sm-0 mt-3 border\">");
            htmlBuilder.Append($"<label class=\"form-label\">References No.</label>");
            htmlBuilder.Append($"<p><span class=\"text-muted fw-normal\">FM/BRD/SSD/180324/ROO</span></p>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"<div class=\"card-body\" style=\"margin-top:-33px;\">");
            htmlBuilder.Append($"<div class=\"d-flex\">");
            htmlBuilder.Append($"<div class=\"flex-grow-1 col-5 border\">");
            htmlBuilder.Append($"<div class=\"flex-shrink-0 mt-sm-0 mt-3 border\">");
            htmlBuilder.Append($"<p class=\"text-muted\"><b>Consigner:@firstItem.SupplierName</b></p>");
            htmlBuilder.Append($"<p class=\"text-muted\">" + orderDetails.BillingAddress + "</p>");
            htmlBuilder.Append($"<br />");
            htmlBuilder.Append($"<p class=\"text-muted\">GSTIN:- </p>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"<div class=\"flex-shrink-0 mt-sm-0 mt-3 border\">");
            htmlBuilder.Append($"<p class=\"text-muted\"><b>Consignee (Ship to):</b></p>");
            htmlBuilder.Append($"<p class=\"text-muted text-uppercase\"><b>" + orderDetails.CompanyName + "</b></p>");
            htmlBuilder.Append($"<p class=\"text-muted text-uppercase\">" + orderDetails.ShippingAddress + "</p>");

            int Index = 1;
            foreach (var item in orderDetails.AddressList)
            {
                htmlBuilder.Append($"<p><span>@Index</span><span> </span><span>" + @item.Address + "</span></p>");
                Index++;
            }
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"<div class=\"flex-grow-1 col-7 border\">");
            htmlBuilder.Append($"<div class=\"d-flex\">");
            htmlBuilder.Append($"<div class=\"flex-grow-1 col-4\">");
            htmlBuilder.Append($"<div class=\"flex-shrink-0 mt-sm-0 mt-3 border\">");
            htmlBuilder.Append($"<label class=\"form-label\">Dispatched through</label>");
            htmlBuilder.Append($"<p class=\"text-muted text-uppercase\">" + orderDetails.SupplierName + "<b></b></p>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"<div class=\"flex-grow-1 col-3\">");
            htmlBuilder.Append($"<div class=\"flex-shrink-0 mt-sm-0 mt-3 border\">");
            htmlBuilder.Append($"<label class=\"form-label\">Destination</label>");
            htmlBuilder.Append($"<p><span class=\"text-muted\"><b>Bonifatius Technologies Pvt ltd,</b></span></p>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"<div class=\"flex-shrink-0 mt-sm-0 mt-3 border\">");
            htmlBuilder.Append($"<label class=\"form-label\">Terms of Delivery</label>");
            htmlBuilder.Append($"<p><span class=\"text-muted\"><b>2-3 Weeks</b></span></p>");
            htmlBuilder.Append($"<p><span class=\"text-muted\"><b>P & F-Inclusive</b></span></p>");
            htmlBuilder.Append($"<p><span class=\"text-muted\"><b>Freight-Extra at actual</b></span></p>");
            htmlBuilder.Append($"<p><span class=\"text-muted\"><b>Note : Test Certificate required with material</b></span></p>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"<div class=\"card-body\" style=\"margin-top:-33px;\">");
            htmlBuilder.Append($"<div class=\"table-responsive\">");
            htmlBuilder.Append($"<table class=\"table table-border text-center table-nowrap align-middle mb-0\">");
            htmlBuilder.Append($"<thead>");
            htmlBuilder.Append($"<tr class=\"table-active\">");
            htmlBuilder.Append($"<th class=\"border\" scope=\"col\">SRr.No.</th>");
            htmlBuilder.Append($"<th class=\"border\" scope=\"col\">Product Details</th>");
            htmlBuilder.Append($"<th class=\"border\" scope=\"col\">HSN / SAC</th>");
            htmlBuilder.Append($"<th class=\"border\" scope=\"col\">Quantity</th>");
            htmlBuilder.Append($"<th class=\"border\" scope=\"col\">Rate</th>");
            htmlBuilder.Append($"<th class=\"border\" scope=\"col\">per</th>");
            htmlBuilder.Append($"<th class=\"text-end border\" text-end scope=\"col\">Amount</th>");
            htmlBuilder.Append($"</tr>");
            htmlBuilder.Append($"</thead>");
            htmlBuilder.Append($"<tbody id=\"products-list\">");

            int index = 1;
            foreach (var item in orderDetails.ItemList)
            {
                htmlBuilder.Append($"<tr class=\"product\">");
                htmlBuilder.Append($"<td class=\"border\">" + index + "</td>");
                htmlBuilder.Append($"<td class=\"text-start border\">");
                htmlBuilder.Append($"<span class=\"fw-medium\" id=\"txtproductname\">" + item.ItemName + "</span>");
                htmlBuilder.Append($"<span class=\"fw-medium\" id=\"txtproductid\" hidden>" + item.ItemId + "</span>");
                htmlBuilder.Append($"<p class=\"text-muted mb-0\" id=\"txtdescription\">" + orderDetails.Description + "</p>");
                htmlBuilder.Append($"</td>");
                htmlBuilder.Append($"<td class=\"border\" id=\"txtHSN\"></td>");
                htmlBuilder.Append($"<td class=\"border\" id=\"txtquantity\">" + item.Quantity + "</td>");
                htmlBuilder.Append($"<td class=\"border\" id=\"txtpriceperunit\">₹" + item.PricePerUnit + "</td>");
                htmlBuilder.Append($"<td class=\"border\" id=\"txtper\">No.</td>");
                htmlBuilder.Append($"<td class=\"text-end border\" id=\"txttotal\">₹" + item.ItemAmount + "</td>");
                htmlBuilder.Append($"</tr>");
                index++;
            }

            htmlBuilder.Append($"<tr>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\">Total</td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\">₹</td>");
            htmlBuilder.Append($"</tr>");

            htmlBuilder.Append($"<tr>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\">SGST 9%</td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\">₹</td>");
            htmlBuilder.Append($"</tr>");

            htmlBuilder.Append($"<tr>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\">CGST 9%</td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\">₹</td>");
            htmlBuilder.Append($"</tr>");

            htmlBuilder.Append($"<tr>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\">Round off</td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"</tr>");

            htmlBuilder.Append($"<tr>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\">Total</td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\"></td>");
            htmlBuilder.Append($"<td class=\"border\">₹" + orderDetails.TotalAmount + "</td>");
            htmlBuilder.Append($"</tr>");
            htmlBuilder.Append($"</tbody>");
            htmlBuilder.Append($"</table><!--end table-->");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"</div>");
            htmlBuilder.Append($"</div>");
            return htmlBuilder.ToString();
        }


        public async Task<IActionResult> GetAllItemDetailsList(string? searchText)
        {
            try
            {
                string apiUrl = $"ItemMaster/GetAllItemDetailsList?searchText={searchText}";
                ApiResponseModel response = await APIServices.PostAsync("", apiUrl);
                if (response.code == 200)
                {
                    List<ItemMasterModel> Items = JsonConvert.DeserializeObject<List<ItemMasterModel>>(response.data.ToString());
                    return PartialView("~/Views/PurchaseMaster/_showAllItemPartial.cshtml", Items);
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
        public async Task<IActionResult> CheckPurchaseOrderNo(Guid? CompanyId)
        {
            try
            {
                ApiResponseModel response = await APIServices.GetAsync("", "PurchaseOrder/CheckPONo?CompanyId=" + CompanyId);

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

        [HttpPost]
        public ActionResult UploadTermsImage(IFormFile upload)
        {
            try
            {
                if (upload != null)
                {

                    var uploadsFolder = Path.Combine(Environment.WebRootPath, "Uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(upload.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        upload.CopyTo(fileStream);
                    }
                    string url = Url.Content("~/Uploads/" + uniqueFileName);
                    return Json(new { uploaded = true, url });
                }
                else
                {
                    return Json(new { uploaded = false, error = "No file uploaded" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { uploaded = false, error = ex.Message });
            }
        }

        public async Task<IActionResult> POPrintDetails(Guid POId)
        {
            try
            {
                PurchaseOrderMasterView order = new PurchaseOrderMasterView();
                ApiResponseModel response = await APIServices.GetAsync("", $"PurchaseOrder/GetPurchaseOrderDetailsById?POId={POId}");
                if (response.code == 200)
                {
                    order = JsonConvert.DeserializeObject<PurchaseOrderMasterView>(response.data.ToString());
                    var number = order.TotalAmount;
                    var totalAmountInWords = NumberToWords(number);
                    ViewData["TotalAmountInWords"] = totalAmountInWords + " " + "Only";
                }
                return View(order);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> PrintPODetails(Guid POId)
        {
            try
            {
                IActionResult result = await POPrintDetails(POId);

                if (result is ViewResult viewResult)
                {
                    var order = viewResult.Model as PurchaseOrderMasterView;
                    var htmlContent = await RenderViewToStringAsync("POPrintDetails", order, viewResult.ViewData);
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

        public async Task<IActionResult> PurchaseOrderView()
        {

            return View();

        }
        public async Task<IActionResult> PrintJKDetails(Guid POId)
        {
            try
            {
                PurchaseOrderMasterView order = new PurchaseOrderMasterView();
                ApiResponseModel response = await APIServices.GetAsync("", $"PurchaseOrder/GetPurchaseOrderDetailsById?POId={POId}");
                if (response.code == 200)
                {
                    order = JsonConvert.DeserializeObject<PurchaseOrderMasterView>(response.data.ToString());
                    var number = order.TotalAmount;
                    var totalAmountInWords = NumberToWords(number);
                    ViewData["TotalAmountInWords"] = totalAmountInWords + " " + "Only";
                }
                return View(order);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> JKDetails(Guid POId)
        {
            try
            {
                IActionResult result = await PrintJKDetails(POId);

                if (result is ViewResult viewResult)
                {
                    var order = viewResult.Model as PurchaseOrderMasterView;
                    var htmlContent = await RenderViewToStringAsync("PrintJKDetails", order, viewResult.ViewData);
                    return Content(htmlContent, "text/html");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> PurchaseUltraView(Guid POId)
        {
            try
            {
                PurchaseOrderMasterView order = new PurchaseOrderMasterView();
                ApiResponseModel response = await APIServices.GetAsync("", $"PurchaseOrder/GetPurchaseOrderDetailsById?POId={POId}");
                if (response.code == 200)
                {
                    order = JsonConvert.DeserializeObject<PurchaseOrderMasterView>(response.data.ToString());
                    var number = order.TotalAmount;
                    var totalAmountInWords = NumberToWords(number);
                    ViewData["TotalAmountInWords"] = totalAmountInWords + " " + "Only";
                }
                return View(order);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> PurchaseUltraDetails(Guid POId)
        {
            try
            {
                IActionResult result = await PurchaseUltraView(POId);

                if (result is ViewResult viewResult)
                {
                    var order = viewResult.Model as PurchaseOrderMasterView;
                    var htmlContent = await RenderViewToStringAsync("PurchaseUltraView", order, viewResult.ViewData);
                    return Content(htmlContent, "text/html");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

