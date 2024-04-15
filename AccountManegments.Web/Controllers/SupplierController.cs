using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.SupplierMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;

namespace AccountManegments.Web.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        public APIServices APIServices { get; }

        public SupplierController(APIServices aPIServices)
        {
            APIServices = aPIServices;
        }
        [FormPermissionAttribute("Supplier-View")]
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

        [FormPermissionAttribute("Supplier-Edit")]
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

        [FormPermissionAttribute("Supplier-Delete")]
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
        public async Task<IActionResult> ExportSupplierListToExcel()
        {
            try
            {
                List<SupplierModel> getSupplierList = new List<SupplierModel>();
                string apiUrl = $"SupplierMaster/GetAllSupplierList";

                ApiResponseModel response = await APIServices.PostAsync("", apiUrl);
                if (response.data.Count != 0)
                {
                    getSupplierList = JsonConvert.DeserializeObject<List<SupplierModel>>(response.data.ToString());
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(ToConvertDataTable(getSupplierList.ToList()));
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            string FileName = Guid.NewGuid() + "_SupplierList.xlsx";
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocuments.spreadsheetml.sheet", FileName);

                        }
                    }
                }
                return RedirectToAction("SupplierList");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetSupplierNameList()
        {
            try
            {
                List<SupplierModel> SupplierName = new List<SupplierModel>();
                ApiResponseModel res = await APIServices.GetAsync("", "SupplierMaster/GetSupplierNameList");
                if (res.code == 200)
                {
                    SupplierName = JsonConvert.DeserializeObject<List<SupplierModel>>(res.data.ToString());
                }
                return new JsonResult(SupplierName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public System.Data.DataTable ToConvertDataTable<T>(List<T> items)
        {
            System.Data.DataTable dt = new System.Data.DataTable(typeof(T).Name);
            PropertyInfo[] propInfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in propInfo)
            {
                dt.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[propInfo.Length];
                for (int i = 1; i < propInfo.Length; i++)
                {
                    values[i] = propInfo[i].GetValue(item, null);
                }
                dt.Rows.Add(values);
            }
            dt.Columns.Remove("SupplierId");
            dt.Columns.Remove("BuildingName");
            dt.Columns.Remove("Area");
            dt.Columns.Remove("State");
            dt.Columns.Remove("StateName");
            dt.Columns.Remove("City");
            dt.Columns.Remove("Pincode");
            dt.Columns.Remove("BankName");
            dt.Columns.Remove("AccountNo");
            dt.Columns.Remove("Iffccode");
            dt.Columns.Remove("isDelete");
            dt.Columns.Remove("CreatedBy");
            dt.Columns.Remove("CreatedOn");
            dt.Columns.Remove("UpdatedBy");
            dt.Columns.Remove("UpdatedOn");
            return dt;
        }
    }
}
