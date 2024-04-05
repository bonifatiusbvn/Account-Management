using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Data.OleDb;

namespace AccountManegments.Web.Controllers
{
    [Authorize]
    public class ItemMasterController : Controller
    {

        public WebAPI WebAPI { get; }
        public APIServices APIServices { get; }
        public IWebHostEnvironment Environment { get; }
        public UserSession _userSession { get; }
        public IConfiguration Configuration { get; }

        public ItemMasterController(WebAPI webAPI, APIServices aPIServices, IWebHostEnvironment environment, UserSession userSession, IConfiguration configuration)
        {
            WebAPI = webAPI;
            APIServices = aPIServices;
            Environment = environment;
            _userSession = userSession;
            Configuration = configuration;
        }
        [FormPermissionAttribute("Item-View")]
        public IActionResult ItemListView()
        {
            return View();
        }
        public async Task<IActionResult> ItemListAction(string searchText, string searchBy, string sortBy)
        {
            try
            {

                string apiUrl = $"ItemMaster/GetItemList?searchText={searchText}&searchBy={searchBy}&sortBy={sortBy}";

                ApiResponseModel res = await APIServices.PostAsync("", apiUrl);

                if (res.code == 200)
                {
                    List<ItemMasterModel> GetItemList = JsonConvert.DeserializeObject<List<ItemMasterModel>>(res.data.ToString());

                    return PartialView("~/Views/ItemMaster/_ItemListPartial.cshtml", GetItemList);
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
        public async Task<JsonResult> DisplayItemDetails(Guid ItemId)
        {
            try
            {
                ItemMasterModel ItemDetails = new ItemMasterModel();
                ApiResponseModel res = await APIServices.GetAsync("", "ItemMaster/GetItemDetailsById?ItemId=" + ItemId);
                if (res.code == 200)
                {
                    ItemDetails = JsonConvert.DeserializeObject<ItemMasterModel>(res.data.ToString());
                }
                return new JsonResult(ItemDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [FormPermissionAttribute("Item-Add")]
        [HttpPost]
        public async Task<IActionResult> CreateItem(ItemMasterModel ItemDetails)
        {

            try
            {
                var item = new ItemMasterModel()
                {
                    ItemId = Guid.NewGuid(),
                    ItemName = ItemDetails.ItemName,
                    UnitType = ItemDetails.UnitType,
                    PricePerUnit = ItemDetails.PricePerUnit,
                    IsWithGst = ItemDetails.IsWithGst,
                    Gstamount = ItemDetails.Gstamount,
                    Gstper = ItemDetails.Gstper,
                    Hsncode = ItemDetails.Hsncode,
                    IsDeleted = false,
                    IsApproved = false,
                    CreatedOn = DateTime.Now,
                    CreatedBy = _userSession.UserId,
                };

                ApiResponseModel postUser = await APIServices.PostAsync(item, "ItemMaster/AddItemDetails");
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
        [HttpGet]
        public async Task<JsonResult> GetAllUnitType()
        {
            try
            {
                List<UnitMasterView> UnitType = new List<UnitMasterView>();
                ApiResponseModel res = await APIServices.GetAsync("", "ItemMaster/GetAllUnitType");
                if (res.code == 200)
                {
                    UnitType = JsonConvert.DeserializeObject<List<UnitMasterView>>(res.data.ToString());
                }
                return new JsonResult(UnitType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [FormPermissionAttribute("Item-Edit")]
        [HttpPost]
        public async Task<IActionResult> UpdateItemDetails(ItemMasterModel ItemDetails)
        {
            try
            {
                ApiResponseModel postUser = await APIServices.PostAsync(ItemDetails, "ItemMaster/UpdateItemDetails");
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
        [FormPermissionAttribute("Item-Edit")]
        [HttpPost]
        public async Task<IActionResult> ItemIsApproved(Guid ItemId)
        {
            try
            {

                ApiResponseModel postuser = await APIServices.PostAsync(null, "ItemMaster/ItemIsApproved?ItemId=" + ItemId);
                if (postuser.code == 200)
                {

                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });

                }
                else
                {
                    return new JsonResult(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [FormPermissionAttribute("Item-Delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteItemDetails(Guid ItemId)
        {
            try
            {
                ApiResponseModel postuser = await APIServices.PostAsync(null, "ItemMaster/DeleteItemDetails?ItemId=" + ItemId);
                if (postuser.code == 200)
                {
                    return Ok(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
                else
                {
                    return new JsonResult(new { Message = string.Format(postuser.message), Code = postuser.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetItemNameList()
        {
            try
            {
                List<ItemMasterModel> ItemName = new List<ItemMasterModel>();
                ApiResponseModel res = await APIServices.GetAsync("", "ItemMaster/GetItemNameList");
                if (res.code == 200)
                {
                    ItemName = JsonConvert.DeserializeObject<List<ItemMasterModel>>(res.data.ToString());
                }
                return new JsonResult(ItemName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[FormPermissionAttribute("Item-Add")]
        [HttpPost]
        public IActionResult ImportExcelFile(IFormFile FormFile)
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
                var filename = FormFile.FileName;
                string extension = Path.GetExtension(filename);
                string conString = string.Empty;
                switch (extension)
                {
                    case ".xls":
                        conString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + filepath + ";Extended Properties='Excel 8.0; HDR=Yes'";
                        break;

                    case ".xlsx":
                        conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties='Excel 8.0; HDR = YES'";
                        break;
                }
                DataTable dt = new DataTable();
                conString = string.Format(conString, filepath);
                using (OleDbConnection conExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = conExcel;
                            conExcel.Open();
                            DataTable dtExcelSchema = conExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["Table_Name"].ToString();
                            cmdExcel.CommandText = "SELECT * FROM [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            conExcel.Close();
                        }
                    }
                }
                conString = Configuration.GetConnectionString("ACCDbconn");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(con))
                    {
                        bulkCopy.DestinationTableName = "ItemMaster";
                        bulkCopy.ColumnMappings.Add("ItemId", "ItemId");
                        bulkCopy.ColumnMappings.Add("ItemName", "ItemName");
                        bulkCopy.ColumnMappings.Add("UnitType", "UnitType");
                        bulkCopy.ColumnMappings.Add("PricePerUnit", "PricePerUnit");
                        bulkCopy.ColumnMappings.Add("IsWithGST", "IsWithGST");
                        bulkCopy.ColumnMappings.Add("GSTAmount", "GSTAmount");
                        bulkCopy.ColumnMappings.Add("GSTPer", "GSTPer");
                        bulkCopy.ColumnMappings.Add("HSNCode", "HSNCode");
                        bulkCopy.ColumnMappings.Add("IsApproved", "IsApproved");
                        bulkCopy.ColumnMappings.Add("IsDeleted", "IsDeleted");
                        bulkCopy.ColumnMappings.Add("CreatedBy", "CreatedBy");
                        bulkCopy.ColumnMappings.Add("CreatedOn", "CreatedOn");
                        bulkCopy.ColumnMappings.Add("UpdatedBy", "UpdatedBy");
                        bulkCopy.ColumnMappings.Add("UpdatedOn", "UpdatedOn");
                        con.Open();
                        bulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return RedirectToAction("ItemListView");
        }

        [HttpPost]
        public async Task<IActionResult> DisplayItemDetailsById()
        {
            try
            {
                string ItemId = HttpContext.Request.Form["ITEMID"];
                var GetItem = JsonConvert.DeserializeObject<ItemMasterModel>(ItemId.ToString());
                ItemMasterModel Items = new ItemMasterModel();
                ApiResponseModel response = await APIServices.GetAsync("", "ItemMaster/GetItemDetailsById?ItemId=" + GetItem.ItemId);
                if (response.code == 200)
                {
                    Items = JsonConvert.DeserializeObject<ItemMasterModel>(response.data.ToString());
                    Items.RowNumber = Items.RowNumber;
                }
                return PartialView("~/Views/PurchaseMaster/_GetItemDetailsPartial.cshtml", Items);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
