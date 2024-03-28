using Microsoft.AspNetCore.Mvc;

namespace AccountManegments.Web.Controllers
{
    public class InvoiceMasterController : Controller
    {
        public IActionResult CreateInvoice()
        {
            return View();
        }

    }
}
