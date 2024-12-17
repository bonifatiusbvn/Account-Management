using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AccountManegments.Web.Controllers
{
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


        public async Task<IActionResult> CreateSalesInvoice()
        {
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
            return View();
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
    }
}
