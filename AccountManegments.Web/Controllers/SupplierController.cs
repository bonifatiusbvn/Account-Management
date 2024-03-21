using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SupplierMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccountManegments.Web.Controllers
{
    public class SupplierController : Controller
    {
        public APIServices APIServices { get; }

        public SupplierController(APIServices aPIServices)
        {
            APIServices = aPIServices;
        }
        public IActionResult SupplierList()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateSupplier(SupplierModel Supplier)
        {

            try
            {

                ApiResponseModel postUser = await APIServices.PostAsync(Supplier, "SupplierMaster/CreateSupplier");
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
    }
}
