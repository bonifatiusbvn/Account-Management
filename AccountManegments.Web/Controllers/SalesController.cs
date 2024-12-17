using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.SalesMaster;
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


        public IActionResult CreateSalesInvoice()
        {
            return View();
        }

        [FormPermissionAttribute("Sales Invoice-View")]
        public async Task<IActionResult> SalesInvoiceListAction(string searchText, string searchBy, string sortBy)
        {
            try
            {
                string siteIdString = UserSession.SiteId;
                Guid? SiteId = !string.IsNullOrEmpty(siteIdString) ? Guid.Parse(siteIdString) : (Guid?)null;
                List<UserCompanyListModel> CompanyData = UserSession.CompanyData;

                string apiUrl = $"Sales/GetSalestInvoiceList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

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

    }
}
