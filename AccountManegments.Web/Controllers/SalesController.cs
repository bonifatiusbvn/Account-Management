using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
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



    }
}
