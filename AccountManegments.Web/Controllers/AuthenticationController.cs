using Microsoft.AspNetCore.Mvc;

namespace AccountManegments.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UserLogin()
        {
            return View();
        }

        public IActionResult UserSingUp()
        {
            return View();
        }
    }
}
