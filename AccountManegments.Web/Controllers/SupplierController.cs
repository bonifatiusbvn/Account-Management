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
using System.Data.OleDb;
using System.Data;
using System.Globalization;
using System.Reflection;
using NuGet.ContentModel;

namespace AccountManegments.Web.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        public WebAPI WebAPI { get; }
        public APIServices APIServices { get; }
        public IWebHostEnvironment Environment { get; }
        public UserSession _userSession { get; }

        public SupplierController(WebAPI webAPI, APIServices aPIServices, IWebHostEnvironment environment, UserSession userSession, IConfiguration configuration)
        {
            WebAPI = webAPI;
            APIServices = aPIServices;
            Environment = environment;
            _userSession = userSession;
        }
        [FormPermissionAttribute("Supplier-View")]
        public IActionResult SupplierList()
        {
            return View();
        }
        [FormPermissionAttribute("Supplier-Add")]
        [HttpPost]
        public async Task<IActionResult> CreateSupplier(SupplierModel Supplier)
        {

            try
            {

                ApiResponseModel postUser = await APIServices.PostAsync(Supplier, "SupplierMaster/CreateSupplier");
                if (postUser.code == 200)
                {
                    return Ok(new { Message = string.Format(postUser.message), Code = postUser.code });
                }
                else
                {
                    return Ok(new { Message = string.Format(postUser.message), Code = postUser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [FormPermissionAttribute("Supplier-View")]
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
                    return BadRequest(new { Message = "Failed to retrieve user list." });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new { Message = $"An error occurred: {ex.Message}" });
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
                    return Ok(new { Message = string.Format(postUser.message), Code = postUser.code });
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
                ApiResponseModel postUser = await APIServices.PostAsync("", "SupplierMaster/DeleteSupplierDetails?SupplierId=" + SupplierId);
                if (postUser.code == 200)
                {
                    return Ok(new { Message = string.Format(postUser.message), Code = postUser.code });
                }
                else
                {
                    return Ok(new { Message = string.Format(postUser.message), Code = postUser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<IActionResult> ActiveDeactiveSupplier(Guid SupplierId)
        {
            try
            {

                ApiResponseModel postuser = await APIServices.PostAsync("", "SupplierMaster/ActiveDeactiveSupplier?SupplierId=" + SupplierId);
                if (postuser.code == 200)
                {

                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });

                }
                else
                {
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });
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

        [HttpPost]
        public async Task<IActionResult> ImportSupplierListFromExcel(IFormFile FormFile)
        {
            try
            {
                var MainPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadExcelFile");
                if (!Directory.Exists(MainPath))
                {
                    Directory.CreateDirectory(MainPath);
                }
                var filepath = Path.Combine(MainPath, FormFile.FileName);
                using (FileStream stream = new FileStream(filepath, FileMode.Create))
                {
                    FormFile.CopyTo(stream);
                }
                var filename = Guid.NewGuid + "_" + FormFile.FileName;
                string extension = Path.GetExtension(filename);
                string excelConString = string.Empty;
                switch (extension)
                {
                    case ".xls":
                        excelConString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + filepath + ";Extended Properties='Excel 8.0; HDR=Yes'";
                        break;

                    case ".xlsx":
                        excelConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties='Excel 8.0; HDR = YES'";
                        break;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("SupplierName", typeof(string));
                dt.Columns.Add("Mobile", typeof(string));
                dt.Columns.Add("Email", typeof(string));
                dt.Columns.Add("GSTNo", typeof(string));
                dt.Columns.Add("BuildingName", typeof(string));
                dt.Columns.Add("Area", typeof(string));
                dt.Columns.Add("State", typeof(string));
                dt.Columns.Add("City", typeof(string));
                dt.Columns.Add("Pincode", typeof(string));
                dt.Columns.Add("BankName", typeof(string));
                dt.Columns.Add("AccountNo", typeof(string));
                dt.Columns.Add("IFFCCode", typeof(string));

                excelConString = string.Format(excelConString, filepath);
                using (OleDbConnection conExcel = new OleDbConnection(excelConString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = conExcel;
                            conExcel.Open();
                            DataTable dtExcelSchema = conExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = "";
                            foreach (DataRow row in dtExcelSchema.Rows)
                            {
                                string tableName = row["TABLE_NAME"].ToString();
                                if (tableName != "StateNames" && tableName != "ANDAMAN___NICOBAR" && tableName != "ARUNACHAL_PRADESH" && tableName != "ASSAM" && tableName != "JHARKHAND" && tableName != "JAMMU___KASHMIR" && tableName != "HIMACHAL_PRADESH"
                                    && tableName != "HARYANA" && tableName != "GUJRAT" && tableName != "GOA" && tableName != "DELHI" && tableName != "DAMAN___DIU" && tableName != "CHATTISGARH" && tableName != "BIHAR" && tableName != "ANDHRA_PRADESH")
                                {
                                    sheetName = tableName;
                                    break;
                                }
                            }
                            if (string.IsNullOrEmpty(sheetName))
                            {
                                throw new Exception("No valid sheet found in the Excel file.");
                            }
                            cmdExcel.CommandText = "SELECT * FROM [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            conExcel.Close();
                        }
                    }
                }

                var supplierList = new List<SupplierModel>();

                foreach (DataRow row in dt.Rows)
                {
                    if (string.IsNullOrWhiteSpace(row[0].ToString()))
                    {
                        break;
                    }
                    try
                    {
                        var supplier = new SupplierModel
                        {
                            SupplierName = row["SupplierName"].ToString(),
                            Mobile = row["Mobile"].ToString(),
                            Email = row["Email"].ToString(),
                            Gstno = row["GSTNo"].ToString(),
                            BuildingName = row["BuildingName"].ToString(),
                            Area = row["Area"].ToString(),
                            StateName = row["State"].ToString(),
                            CityName = row["City"].ToString(),
                            PinCode = row["Pincode"].ToString(),
                            BankName = row["BankName"].ToString(),
                            AccountNo = row["AccountNo"].ToString(),
                            Iffccode = row["IFFCCode"].ToString(),
                            CreatedBy = _userSession.UserId,
                            CreatedOn = DateTime.Now,
                            IsApproved = true,
                        };

                        supplierList.Add(supplier);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing row: {ex.Message}");
                        foreach (DataColumn column in dt.Columns)
                        {
                            Console.WriteLine($"{column.ColumnName}: {row[column]}");
                        }
                    }
                }

                ApiResponseModel postuser = await APIServices.PostAsync(supplierList, "SupplierMaster/ImportSupplierListFromExcel");
                if (postuser.code == 200)
                {
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
                else
                {
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return View("SupplierList");
        }
    }
}
