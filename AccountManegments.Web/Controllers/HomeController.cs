using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.PurchaseRequest;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace AccountManegments.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public WebAPI WebAPI { get; }
        public APIServices APIServices { get; }
        public HomeController(ILogger<HomeController> logger, WebAPI webAPI, APIServices aPIServices)
        {
            _logger = logger;

            WebAPI = webAPI;
            APIServices = aPIServices;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> PurchaseRequestList(string searchText, string searchBy, string sortBy, Guid? SiteId, string SiteName)
        {
            try
            {

                UserSession.SiteId = SiteId.ToString();
                //UserSession.SiteName = SiteId == null ? "All Site" : SiteName.ToString();

                //Guid? siteId = string.IsNullOrEmpty(UserSession.SiteId) ? null : new Guid(UserSession.SiteId);

                string apiUrl = $"PurchaseRequest/GetPurchaseRequestList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}&&siteId={SiteId}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    List<PurchaseRequestModel> GetSiteList = JsonConvert.DeserializeObject<List<PurchaseRequestModel>>(res.data.ToString());

                    return PartialView("~/Views/Home/_DashbordPurchaseRequestList.cshtml", GetSiteList.Where(e => e.IsApproved == false));
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult UnAuthorised()
        {
            return View();
        }
    }
}
