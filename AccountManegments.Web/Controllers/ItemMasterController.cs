using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Aspose.Pdf.Text;
using Aspose.Pdf;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Newtonsoft.Json;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using ClosedXML.Excel;

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
        [FormPermissionAttribute("PurchaseMaster-View")]
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
                    return BadRequest(new { Message = "Failed to retrieve user list." });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new { Message = $"An error occurred: {ex.Message}" });
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
                bool isApproved = UserSession.FormPermisionData.Any(a => a.FormName == "Item" && (a.IsApproved == true));

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
                    IsApproved = isApproved,
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
                    return Ok(new { Message = string.Format(postUser.message), Code = postUser.code });
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
                    return Ok(new { Message = string.Format(postUser.message), Code = postUser.code });
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

                ApiResponseModel postuser = await APIServices.PostAsync("", "ItemMaster/ItemIsApproved?ItemId=" + ItemId);
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

        [FormPermissionAttribute("Item-Delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteItemDetails(Guid ItemId)
        {
            try
            {
                ApiResponseModel postuser = await APIServices.PostAsync("", "ItemMaster/DeleteItemDetails?ItemId=" + ItemId);
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

        [FormPermissionAttribute("Item-Add")]
        [HttpPost]
        public async Task<IActionResult> ImportExcelFile(IFormFile FormFile)
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
                var filename = Guid.NewGuid().ToString() + "_" + FormFile.FileName; // Fixing incorrect usage of Guid
                string extension = Path.GetExtension(filename);
                string excelConString = string.Empty;
                switch (extension)
                {
                    case ".xls":
                        excelConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties='Excel 8.0; HDR=Yes'";
                        break;
                    case ".xlsx":
                        excelConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties='Excel 8.0; HDR=Yes'";
                        break;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("ItemName", typeof(string));
                dt.Columns.Add("UnitType", typeof(string));
                dt.Columns.Add("PricePerUnit", typeof(decimal));
                dt.Columns.Add("GSTPer", typeof(decimal));
                dt.Columns.Add("HSNCode", typeof(string));

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
                                if (tableName != "DD")
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

                var items = new List<ItemMasterModel>();
                foreach (DataRow row in dt.Rows)
                {
                    try
                    {
                        decimal pricePerUnit = Convert.ToDecimal(row["PricePerUnit"]);
                        decimal gstper = Convert.ToDecimal(row["GSTPer"]);
                        decimal gstamount = pricePerUnit / 100 * gstper;

                        var item = new ItemMasterModel
                        {
                            ItemName = row["ItemName"].ToString(),
                            UnitTypeName = row["UnitType"].ToString(),
                            PricePerUnit = pricePerUnit,
                            Gstper = gstper,
                            Gstamount = gstamount,
                            Hsncode = row["HSNCode"].ToString(),
                            CreatedBy = _userSession.UserId,
                            CreatedOn = DateTime.Now,
                            IsWithGst = false,
                        };

                        items.Add(item);
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

                ApiResponseModel postuser = await APIServices.PostAsync(items, "ItemMaster/InsertItemDetailsFromExcel");
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

        [FormPermissionAttribute("PurchaseMaster-View")]
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
        [FormPermissionAttribute("PurchaseMaster-View")]
        [HttpPost]
        public async Task<IActionResult> DisplayItemDetailsListById()
        {
            try
            {
                string ItemId = HttpContext.Request.Form["ITEMID"];
                var GetItem = JsonConvert.DeserializeObject<POItemDetailsModel>(ItemId.ToString());
                List<POItemDetailsModel> Items = new List<POItemDetailsModel>();
                ApiResponseModel response = await APIServices.GetAsync("", "ItemMaster/GetItemDetailsListById?ItemId=" + GetItem.ItemId);
                if (response.code == 200)
                {
                    Items = JsonConvert.DeserializeObject<List<POItemDetailsModel>>(response.data.ToString());
                    foreach (var item in Items)
                    {
                        item.Gstamount = (item.PricePerUnit * GetItem.Quantity * item.GstPercentage) / 100;
                        item.TotalAmount = (GetItem.Quantity * item.PricePerUnit) + item.Gstamount;
                        item.Quantity = GetItem.Quantity;
                    }
                }
                return PartialView("~/Views/PurchaseMaster/_GetItemDetailsPartial.cshtml", Items);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> DisplayItemInInvoiceById()
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
                return PartialView("~/Views/InvoiceMaster/_DisplayInvoiceItemDetailsPartial.cshtml", Items);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> DisplayItemListInInvoiceById()
        {
            try
            {
                string ItemId = HttpContext.Request.Form["ITEMID"];
                var GetItem = JsonConvert.DeserializeObject<POItemDetailsModel>(ItemId.ToString());
                List<POItemDetailsModel> Items = new List<POItemDetailsModel>();
                ApiResponseModel response = await APIServices.GetAsync("", "ItemMaster/GetItemDetailsListById?ItemId=" + GetItem.ItemId);
                if (response.code == 200)
                {
                    Items = JsonConvert.DeserializeObject<List<POItemDetailsModel>>(response.data.ToString());
                    foreach (var item in Items)
                    {
                        item.Gstamount = (item.PricePerUnit * GetItem.Quantity * item.GstPercentage) / 100;
                        item.TotalAmount = (GetItem.Quantity * item.PricePerUnit) + item.Gstamount;
                        item.Quantity = GetItem.Quantity;
                    }
                }
                return PartialView("~/Views/InvoiceMaster/_DisplayInvoiceItemDetailsPartial.cshtml", Items);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetItemHistory(Guid ItemId)
        {
            try
            {
                SupplierInvoiceList Items = new SupplierInvoiceList();
                ApiResponseModel response = await APIServices.PostAsync("", "ItemMaster/GetItemHistory?ItemId=" + ItemId);
                if (response.code == 200)
                {
                    Items = JsonConvert.DeserializeObject<SupplierInvoiceList>(response.data.ToString());
                }
                return PartialView("~/Views/ItemMaster/_ItemHistoryPartial.cshtml", Items);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportItemHistoryToPDF(Guid ItemId)
        {
            try
            {
                SupplierInvoiceList Items = new SupplierInvoiceList();
                ApiResponseModel response = await APIServices.PostAsync("", "ItemMaster/GetItemHistory?ItemId=" + ItemId);
                if (response.code == 200)
                {
                    Items = JsonConvert.DeserializeObject<SupplierInvoiceList>(response.data.ToString());

                    string FormatIndianCurrency(decimal amount)
                    {
                        var cultureInfo = new CultureInfo("en-IN");
                        var numberFormat = cultureInfo.NumberFormat;
                        numberFormat.CurrencySymbol = "₹";
                        numberFormat.CurrencyGroupSizes = new[] { 3, 2 };
                        numberFormat.CurrencyDecimalDigits = 2;
                        numberFormat.CurrencyGroupSeparator = ",";
                        numberFormat.CurrencyDecimalSeparator = ".";
                        return amount.ToString("C", numberFormat);
                    }

                    var document = new Aspose.Pdf.Document
                    {
                        PageInfo = new PageInfo { Margin = new MarginInfo(20, 25, 20, 35) }
                    };

                    var pdfPage = document.Pages.Add();

                    Aspose.Pdf.Table table = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "26% 20% 13% 10% 10% 8% 13%",
                        DefaultCellPadding = new MarginInfo(2, 2, 2, 2),
                        Border = new BorderInfo(BorderSide.None),
                        DefaultCellBorder = new BorderInfo(BorderSide.None),
                    };

                    var headerRow = table.Rows.Add();
                    headerRow.Cells.Add("Item Name");
                    headerRow.Cells.Add("Invoice No");
                    headerRow.Cells.Add("Date");
                    headerRow.Cells.Add("Quantity");
                    headerRow.Cells.Add("Price");
                    headerRow.Cells.Add("GST");
                    headerRow.Cells.Add("PriceWithGST");

                    foreach (var cell in headerRow.Cells)
                    {
                        cell.BackgroundColor = Aspose.Pdf.Color.Black;
                        var fragment = cell.Paragraphs[0] as TextFragment;
                        if (fragment != null)
                        {
                            fragment.TextState.ForegroundColor = Aspose.Pdf.Color.White;
                        }
                    }

                    foreach (var item in Items.InvoiceList)
                    {
                        var row = table.Rows.Add();
                        row.Cells.Add(item.Itemname != null ? item.Itemname : "");
                        row.Cells.Add(item.InvoiceNo != null ? item.InvoiceNo : "");
                        row.Cells.Add(item.Date?.ToString("dd-MM-yyyy"));
                        if (Items.InvoiceItemList.TryGetValue(item.Id, out var itemDetails))
                        {
                            foreach (var itemDetail in itemDetails)
                            {
                                row.Cells.Add(itemDetail.Quantity.ToString() ?? "");
                            }
                        }
                        row.Cells.Add(FormatIndianCurrency(item.ItemPrice ?? 0));
                        row.Cells.Add(item.GSTper.ToString() + "%");
                        row.Cells.Add(FormatIndianCurrency(item.ItemPricewithGST ?? 0));
                        var backgroundColor = table.Rows.Count % 2 == 0 ? Aspose.Pdf.Color.LightGray : Aspose.Pdf.Color.White;
                        foreach (var cell in row.Cells)
                        {
                            cell.BackgroundColor = backgroundColor;
                        }
                    }
                    pdfPage.Paragraphs.Add(table);

                    using (var streamout = new MemoryStream())
                    {
                        document.Save(streamout);
                        return new FileContentResult(streamout.ToArray(), "application/pdf")
                        {
                            FileDownloadName = Guid.NewGuid() + "_ItemHistoryDetails.pdf",
                        };
                    }
                }
                return RedirectToAction("ItemListView");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public async Task<IActionResult> ExportItemHistoryToExcel(Guid ItemId)
        {
            try
            {
                SupplierInvoiceList Items = new SupplierInvoiceList();
                ApiResponseModel response = await APIServices.PostAsync("", "ItemMaster/GetItemHistory?ItemId=" + ItemId);
                if (response.code == 200)
                {
                    Items = JsonConvert.DeserializeObject<SupplierInvoiceList>(response.data.ToString());

                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Item History");

                        var row = 1;

                        ws.Cell(row, 1).Value = "Item Name";
                        ws.Cell(row, 2).Value = "Invoice No";
                        ws.Cell(row, 3).Value = "Date";
                        ws.Cell(row, 4).Value = "Quantity";
                        ws.Cell(row, 5).Value = "Price";
                        ws.Cell(row, 6).Value = "GST";
                        ws.Cell(row, 7).Value = "PriceWithGst";

                        var headerRange2 = ws.Range(row, 1, row, 7);
                        headerRange2.Style.Font.Bold = true;
                        headerRange2.Style.Fill.BackgroundColor = XLColor.Black;
                        headerRange2.Style.Font.FontColor = XLColor.White;
                        headerRange2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        row++;

                        string FormatIndianCurrency(decimal amount)
                        {
                            var cultureInfo = new CultureInfo("en-IN");
                            var numberFormat = cultureInfo.NumberFormat;
                            numberFormat.CurrencySymbol = "₹";
                            numberFormat.CurrencyGroupSizes = new[] { 3, 2 };
                            numberFormat.CurrencyDecimalDigits = 2;
                            numberFormat.CurrencyGroupSeparator = ",";
                            numberFormat.CurrencyDecimalSeparator = ".";
                            return amount.ToString("C", numberFormat);
                        }

                        foreach (var item in Items.InvoiceList)
                        {
                            string cellValue;
                            ws.Cell(row, 1).Value = item.Itemname;
                            ws.Cell(row, 2).Value = item.InvoiceNo ?? string.Empty;
                            ws.Cell(row, 3).Value = item.Date?.ToString("dd-MM-yyyy") ?? string.Empty;
                            if (Items.InvoiceItemList.TryGetValue(item.Id, out var itemDetails))
                            {
                                foreach (var itemDetail in itemDetails)
                                {
                                    ws.Cell(row, 4).Value = (itemDetail.Quantity.ToString() ?? "");
                                }
                            }
                            ws.Cell(row, 5).Value = FormatIndianCurrency(item.ItemPrice ?? 0);
                            ws.Cell(row, 6).Value = item.GSTper.ToString() + "%";
                            ws.Cell(row, 7).Value = FormatIndianCurrency(item.ItemPricewithGST ?? 0);
                            row++;
                        }
                        using (var stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            var fileName = $"{Guid.NewGuid()}_ItemHistoryDetails.xlsx";
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                        }
                    }
                }
                return RedirectToAction("ReportItemListViewDetails");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
