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

namespace AccountManegments.Web.Controllers
{
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


        [FormPermissionAttribute("PurchaseMaster-View")]
        public async Task<IActionResult> PurchaseRequestListView()
        {
            try
            {
                ApiResponseModel Response = await APIServices.GetAsync("", "PurchaseRequest/CheckPRNo");

                if (Response.code == 200)
                {
                    ViewData["PrNo"] = JsonConvert.DeserializeObject<string>(JsonConvert.SerializeObject(Response.data));
                }

                return View();
            }
            catch (Exception ex)
            {
                throw ex;

            }
            return View();
        }

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
                    return new JsonResult(new { Message = "Failed to retrieve Purchase Request list." });
                }
            }
            catch (Exception ex)
            {

                return new JsonResult(new { Message = $"An error occurred: {ex.Message}" });
            }
        }
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


        [FormPermissionAttribute("PurchaseMaster-Add")]
        [HttpPost]
        public async Task<IActionResult> CreatePurchaseRequest(PurchaseRequestModel PurchaseRequestDetails)
        {

            try
            {
                var PurchaseRequest = new PurchaseRequestModel()
                {
                    Pid = Guid.NewGuid(),
                    Item = PurchaseRequestDetails.Item,
                    ItemId = PurchaseRequestDetails.ItemId,
                    UnitTypeId = PurchaseRequestDetails.UnitTypeId,
                    SiteId = PurchaseRequestDetails.SiteId,
                    Quantity = PurchaseRequestDetails.Quantity,
                    CreatedBy = UserSession.UserId,
                    PrNo = PurchaseRequestDetails.PrNo,
                    IsApproved = false,
                };

                ApiResponseModel postUser = await APIServices.PostAsync(PurchaseRequest, "PurchaseRequest/AddPurchaseRequestDetails");
                if (postUser.code == 200)
                {
                    return Ok(new { Message = postUser.message, Code = postUser.code });
                }
                else
                {
                    return new JsonResult(new { Message = string.Format(postUser.message), Code = postUser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [FormPermissionAttribute("PurchaseMaster-Edit")]
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
                    return new JsonResult(new { Message = string.Format(postUser.message), Code = postUser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [FormPermissionAttribute("PurchaseMaster-Delete")]
        [HttpPost]
        public async Task<IActionResult> DeletePurchaseRequest(Guid PurchaseId)
        {
            try
            {
                ApiResponseModel postuser = await APIServices.PostAsync(null, "PurchaseRequest/DeletePurchaseRequest?PurchaseId=" + PurchaseId);
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

        [FormPermissionAttribute("PurchaseMaster-Edit")]
        [HttpPost]
        public async Task<IActionResult> PurchaseRequestIsApproved(Guid PurchaseId)
        {
            try
            {

                ApiResponseModel postuser = await APIServices.PostAsync(null, "PurchaseRequest/PurchaseRequestIsApproved?PurchaseId=" + PurchaseId);
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
                    }
                    ViewBag.PurchaseOrderNo = response.Poid;
                }
                else
                {

                    ApiResponseModel Response = await APIServices.GetAsync("", "PurchaseOrder/CheckPONo");
                    if (Response.code == 200)
                    {
                        ViewBag.PurchaseOrderNo = Response.data;
                    }
                }
                return View(response);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        [FormPermissionAttribute("PurchaseMaster-Add")]
        [HttpPost]
        public async Task<IActionResult> InsertMultiplePurchaseOrderDetails()
        {
            try
            {

                var OrderDetails = HttpContext.Request.Form["PODETAILS"];
                var InsertDetails = JsonConvert.DeserializeObject<PurchaseOrderMasterView>(OrderDetails.ToString());
                ApiResponseModel postuser = await APIServices.PostAsync(InsertDetails, "PurchaseOrder/InsertMultiplePurchaseOrderDetails");
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

        [FormPermissionAttribute("PurchaseMaster-View")]
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
                    return new JsonResult(new { Message = "Failed to retrieve user list." });
                }
            }
            catch (Exception ex)
            {

                return new JsonResult(new { Message = $"An error occurred: {ex.Message}" });
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

        [FormPermissionAttribute("PurchaseMaster-Edit")]
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

        [FormPermissionAttribute("PurchaseMaster-Delete")]
        [HttpPost]
        public async Task<IActionResult> DeletePurchaseOrderDetails(Guid POId)
        {
            try
            {
                ApiResponseModel postuser = await APIServices.PostAsync(null, "PurchaseOrder/DeletePurchaseOrderDetails?POId=" + POId);
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

        [FormPermissionAttribute("PurchaseMaster-View")]
        public async Task<IActionResult> POListView()
        {
            return View();
        }

        public async Task<IActionResult> POListAction(string searchText, string searchBy, string sortBy)
        {
            try
            {

                string apiUrl = $"PurchaseOrder/GetPurchaseOrderList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    List<PurchaseOrderView> GetPOList = JsonConvert.DeserializeObject<List<PurchaseOrderView>>(res.data.ToString());

                    return PartialView("~/Views/PurchaseMaster/_POListPartial.cshtml", GetPOList);
                }
                else
                {
                    return new JsonResult(new { Message = "Failed to retrieve Purchase Order list" });
                }
            }
            catch (Exception ex)
            {

                return new JsonResult(new { Message = $"An error occurred: {ex.Message}" });
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
                    response.data = order;
                }
                return View(order);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                    return new JsonResult(new { Message = "Failed to retrieve Purchase Order list" });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
