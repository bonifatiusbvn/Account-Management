using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SupplierMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public async Task<IActionResult> SupplierListAction(string searchText, string searchBy, string sortBy)
        {
            try
            {

                string apiUrl = $"SupplierMaster/GetAllSupplierList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    List<SupplierModel> GetUserList = JsonConvert.DeserializeObject<List<SupplierModel>>(res.data.ToString());

                    return PartialView("~/Views/Supplier/_SupplierListPartial.cshtml", GetUserList);
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

        public async Task<JsonResult> DisplaySupplier(Guid SupplierId)
        {
            try
            {
                SupplierModel UserDetails = new SupplierModel();
                ApiResponseModel res = await APIServices.GetAsync("", "SupplierMaster/GetSupplierById?SupplierId=" + SupplierId);
                if (res.code == 200)
                {
                    UserDetails = JsonConvert.DeserializeObject<SupplierModel>(res.data.ToString());
                }
                return new JsonResult(UserDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateSupplierDetails(SupplierModel UpdateSupplier)
        {
            try
            {

                ApiResponseModel postUser = await APIServices.PostAsync(UpdateSupplier, "SupplierMaster/UpdateSupplierDetails");
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
        public async Task<IActionResult> DeleteSupplierDetails(Guid SupplierId)
        {
            try
            {
                ApiResponseModel postUser = await APIServices.PostAsync(null, "SupplierMaster/DeleteSupplierDetails?SupplierId=" + SupplierId);
                if (postUser.code == 200)
                {
                    return Ok(new { Message = string.Format(postUser.message), Code = postUser.code });
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
