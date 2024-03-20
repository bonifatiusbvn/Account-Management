using AccountManegments.Web.Helper;
using Microsoft.AspNetCore.Mvc;

namespace AccountManegments.Web.Controllers
{
    public class SiteMasterController : Controller
    {
        public SiteMasterController(WebAPI webAPI, APIServices aPIServices, IWebHostEnvironment environment)
        {
            WebAPI = webAPI;
            APIServices = aPIServices;
            Environment = environment;
        }

        public WebAPI WebAPI { get; }
        public APIServices APIServices { get; }
        public IWebHostEnvironment Environment { get; }

        public IActionResult SiteListView()
        {
            return View();
        }
    }
}
