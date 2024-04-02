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

        [HttpPost]
        public async Task<IActionResult> CreatePurchaseRequest(PurchaseRequestModel PurchaseRequestDetails)
        {

            try
            {
                var PurchaseRequest = new PurchaseRequestModel()
                {
                    Pid = Guid.NewGuid(),
                    Item = PurchaseRequestDetails.Item,
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

        [HttpPost]
        public async Task<IActionResult> InsertMultiplePurchaseOrderDetails()
        {
            try
            {

                var OrderDetails = HttpContext.Request.Form["PODETAILS"];
                var InsertDetails = JsonConvert.DeserializeObject<List<PurchaseOrderMasterView>>(OrderDetails.ToString());
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
        [HttpPost]
        public async Task<IActionResult> UpdateMultiplePurchaseOrderDetails()
        {
            try
            {
                var OrderDetails = HttpContext.Request.Form["PODETAILS"];
                var UpdateDetails = JsonConvert.DeserializeObject<List<PurchaseOrderMasterView>>(OrderDetails.ToString());
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

                    var document = new Document
                    {
                        PageInfo = new PageInfo { Margin = new MarginInfo(25, 25, 25, 40) }
                    };

                    var pdfPage = document.Pages.Add();

                    var textBuilder = new TextFragment();

                    string tableContent = "";
                        int index = 1;
                    foreach (var item in orderDetails.ItemList)
                    {
                        tableContent += $@"
                         <tr class='product'>
                                <td>{index}</td>
                                <td class='text-start'>
                                <span class='fw-medium' id='txtproductname'>{item.ItemName}</span>
                        <span class='fw-medium' id='txtproductid' hidden>{item.ItemId}</span>
                        <p class='text-muted mb-0' id='txtdescription'>{item.GstPercentage}</p>
                        <p class='text-muted mb-0' id='txtdescription'>{item.GstPercentage}</p>
                        </td>
                        <td id='txtperunitprice'>{item.PricePerUnit}</td>
                        <td id='txtquantity'>{item.Quantity}</td>
                        <td id='txtperunitwithgstprice'>{item.Gstamount}</td>
                        <td class='text-end' id='txttotal'>{item.ItemAmount}</td>
                        </tr>";

                        index++;
                    }

                    string htmlContent = $@"<div class='row justify-content-center' id='printableContent'>" +
                        "<div class='col-xxl-9'>" +
                            "<div class='card' id='demo'>" +
                                "<div class='row'>" +
                                    "<div class='col-lg-12'>" +
                                        "<div class='card-header border-bottom-dashed p-4'>" +
                                            "<div class='d-flex'>" +
                                                "<div class='flex-grow-1'>" +
                                                    "<img src='~/UserHome/minimal/assets/images/logo - bonifatius.jpeg' class='card-logo card-logo-dark user-profile-image img-fluid' alt='logo dark' style='width:250px;'>" +
                                                    "<div class='mt-sm-5 mt-4'>" +
                                                        "<h6 class='text-muted text-uppercase fw-semibold'>Address</h6>" +
                                                        "<h6><span class='text-muted fw-normal'>Bonifatius Technologies Pvt. Ltd.</span></h6>" +
                                                        "<h6><span class='text-muted fw-normal'>SF- 203, Peridot Complex, Urmi co-Operative scoiety Akota,</span></h6>" +
                                                        "<h6><span class='text-muted fw-normal'>Vadodara,Gujarat 390020</span></h6>" +
                                                    "</div>" +
                                                "</div>" +
                                                "<div class='flex-shrink-0 mt-sm-0 mt-3'>" +
                                                    "<h6><span class='text-muted fw-normal'>Legal Registration No:</span><span>987654</span></h6>" +
                                                    "<h6><span class='text-muted fw-normal'>Email:</span><span>bonifatius@gmail.com</span></h6>" +
                                                    "<h6 class='mb-0'><span class='text-muted fw-normal'>Contact No: </span><span> +(91) 64 234 6789</span></h6>" +
                                                "</div>" +
                                            "</div>" +
                                        "</div>" +
                                        "<!--end card-header-->" +
                                    "</div><!--end col-->" +
                                    "<div class='col-lg-12'>" +
                                        "<div class='card-body p-4'>" +
                                            "<div class='row g-3'>" +
                                                "@if (firstItem != null)" +
                                                "{" +
                                                    "<div class='col-lg-3 col-6'>" +
                                                        "<p class='text-muted mb-2 text-uppercase fw-semibold'>Invoice No</p>" +
                                                        "<h5 class='fs-14 mb-0' id='txtinvoiceid'>" + orderDetails.Poid + "</h5>" +
                                                    "</div>" +
                                                    "<!--end col-->" +
                                                    "<div class='col-lg-3 col-6'>" +
                                                        "<p class='text-muted mb-2 text-uppercase fw-semibold'>Date</p>" +
                                                        "<h5 class='fs-14 mb-0' id='txtDate'>" + orderDetails.Date + "</h5>" +
                                                    "</div>" +
                                                    "<!--end col-->" +
                                                    "<div class='col-lg-3 col-6'>" +
                                                        "<p class='text-muted mb-2 text-uppercase fw-semibold'>Payment Status</p>" +
                                                        "<h5 class='bg-success-subtle text-success' id='txtDeliveryShedule'>" + orderDetails.DeliveryShedule + "</h5>" +
                                                    "</div>" +
                                                    "<!--end col-->" +
                                                    "<div class='col-lg-3 col-6'>" +
                                                        "<p class='text-muted mb-2 text-uppercase fw-semibold'>Total Amount</p>" +
                                                        "<h5 class='fs-14 mb-0'>₹<span id='txttotalamount'>" + orderDetails.TotalAmount + "</span></h5>" +
                                                    "</div>" +
                                                    "<!--end col-->" +
                                                "}" +
                                            "</div>" +
                                            "<!--end row-->" +
                                        "</div>" +
                                        "<!--end card-body-->" +
                                    "</div><!--end col-->" +
                                    "<div class='col-lg-12'>" +
                                        "<div class='card-body p-4 border-top border-top-dashed'>" +
                                            "@if (firstItem != null)" +
                                            "{" +
                                                "<div class='row g-3'>" +
                                                    "<div class='col-6'>" +
                                                        "<h6 class='text-muted text-uppercase fw-semibold mb-3'>Billing Address</h6>" +
                                                        "<p class='fw-medium mb-2' id='txtcompanyname'>" + orderDetails.CompanyName + "</p>" +
                                                        "<p class='text-muted mb-1' id='txtaddress'>" + orderDetails.BillingAddress + "</p>" +
                                                    "</div>" +
                                                    "<!--end col-->" +
                                                    "<div class='col-6'>" +
                                                        "<h6 class='text-muted text-uppercase fw-semibold mb-3'>Shipping Address</h6>" +
                                                        "<p class='fw-medium mb-2' id='txtshippingcompany'>" + orderDetails.SupplierName + "</p>" +
                                                        "@*<p class='text-muted mb-1' id='txtshippingaddress'>@firstItem.BillingAddress</p>*@" +
                                                    "</div>" +
                                                    "<!--end col-->" +
                                                "</div>" +
                                                "<!--end row-->" +
                                            "}" +
                                        "</div>" +
                                        "<!--end card-body-->" +
                                    "</div><!--end col-->" +
                                    "<div class='col-lg-12'>" +
                                        "<div class='card-body p-4'>" +
                                            "<div class='table-responsive'>" +
                                                "<table class='table table-borderless text-center table-nowrap align-middle mb-0'>" +
                                                    "<thead>" +
                                                        "<tr class='table-active'>" +
                                                            "<th scope='col'>#</th>" +
                                                            "<th scope='col'>Product Details</th>" +
                                                            "<th scope='col'>Rate</th>" +
                                                            "<th scope='col'>Quantity</th>" +
                                                            "<th scope='col'>RateWithGst</th>" +
                                                            "<th scope='col' class='text-end'>Amount</th>" +
                                                        "</tr>" +
                                                    "</thead>" +
                                                    "<tbody id='products-list'>" +
                                                       $"{tableContent}" +
                                                    "</tbody>" +
                                                "</table><!--end table-->" +
                                            "</div>" +
                                            "<div class='border-top border-top-dashed mt-2'>" +
                                                "<table class='table table-borderless table-nowrap align-middle mb-0 ms-auto' style='width:250px'>" +
                                                    "<tbody>" +
                                                        "<tr class='border-top border-top-dashed fs-15'>" +
                                                            "<th scope='row'>Total Amount</th>" +
                                                            "<th class='text-end' id='txtTotalAmount'>₹" + orderDetails.TotalAmount + "</th>" +
                                                        "</tr>" +
                                                    "</tbody>" +
                                                "</table>" +
                                                "<!--end table-->" +
                                            "</div>" +
                                            "<div class='mt-4'>" +
                                                "<div class='alert alert-primary'>" +
                                                    "<p class='mb-0'>" +
                                                        "<span class='fw-semibold'>NOTES:</span>" +
                                                        "<span id='note'>" +
                                                            "All accounts are to be paid within 7 days from receipt of invoice. To be paid by cheque or" +
                                                            "credit card or direct payment online. If account is not paid within 7" +
                                                            "days the credits details supplied as confirmation of work undertaken" +
                                                            "will be charged the agreed quoted fee noted above." +
                                                        "</span>" +
                                                    "</p>" +
                                                "</div>" +
                                            "</div>" +
                                        "</div>" +
                                        "<!--end card-body-->" +
                                    "</div><!--end col-->" +
                                "</div><!--end row-->" +
                            "</div>" +
                            "<!--end card-->" +
                        "</div>" +
                        "<!--end col-->" +
                    "</div>";
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

    }
}
