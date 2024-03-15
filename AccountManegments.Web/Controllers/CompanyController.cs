using Microsoft.AspNetCore.Mvc;

namespace AccountManegments.Web.Controllers
{
    public class CompanyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateCompany()
        {
            return View();
        }
    }
}
