﻿using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.DataTableParameters;
using AccountManagement.DBContext.Models.ViewModels.CompanyModels;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Drawing;
using System.Globalization;
using System.Reflection;

namespace AccountManegments.Web.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        public ReportController(APIServices aPIServices, UserSession userSession)
        {
            APIServices = aPIServices;
            User_Session = aPIServices;
        }

        public APIServices APIServices { get; }
        public APIServices User_Session { get; }

        public IActionResult ReportDetails()
        {
            return View();
        }
        //[HttpPost]
        //public async Task<IActionResult> GetSupplierInvoiceDetailsReport(InvoiceReportModel invoiceReport)
        //{
        //    try
        //    {
        //        List<SupplierInvoiceModel> SupplierDetails = new List<SupplierInvoiceModel>();

        //        ApiResponseModel response = await APIServices.PostAsync(invoiceReport, "SupplierInvoice/GetSupplierInvoiceDetailsReport");
        //        if (response.code == 200)
        //        {
        //            SupplierDetails = JsonConvert.DeserializeObject<List<SupplierInvoiceModel>>(response.data.ToString());
        //        }
        //        return PartialView("~/Views/Report/_ReportDetailsPartial.cshtml", SupplierDetails);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> GetSupplierInvoiceDetailsReport(InvoiceReportModel invoiceReport)
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                var sortColumn = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();
                if (sortColumnIndex == "5")
                {
                    sortColumn = "credit";
                }
                if (sortColumnIndex == "6")
                {
                    sortColumn = "debit";
                }

                var dataTable = new DataTableRequstModel
                {
                    draw = draw,
                    start = start,
                    pageSize = pageSize,
                    skip = skip,
                    lenght = length,
                    searchValue = searchValue,
                    sortColumn = sortColumn,
                    sortColumnDir = sortColumnDir,
                    filterType = invoiceReport.filterType,
                    SiteId = invoiceReport.SiteId,
                    SupplierId = invoiceReport.SupplierId,
                    CompanyId = invoiceReport.CompanyId,
                    GroupName = invoiceReport.GroupName,
                    SelectedYear = invoiceReport.SelectedYear,
                    startDate = invoiceReport.startDate,
                    endDate = invoiceReport.endDate,
                    SupplierName = invoiceReport.SupplierName,
                    CompanyName = invoiceReport.CompanyName,
                };
                List<SupplierInvoiceModel> supplierDetails = new List<SupplierInvoiceModel>();
                var jsonData = new jsonData();
                ApiResponseModel response = await APIServices.PostAsync(dataTable, "SupplierInvoice/GetSupplierInvoiceDetailsReport");
                if (response.code == 200)
                {
                    jsonData = JsonConvert.DeserializeObject<jsonData>(response.data.ToString());
                    supplierDetails = JsonConvert.DeserializeObject<List<SupplierInvoiceModel>>(jsonData.data.ToString());

                    var result = new
                    {
                        TotalCredit = jsonData.TotalCredit,
                        TotalDebit = jsonData.TotalDebit,
                        draw = jsonData.draw,
                        recordsFiltered = jsonData.recordsFiltered,
                        recordsTotal = jsonData.recordsTotal,
                        data = supplierDetails
                    };

                    return new JsonResult(result);
                }
                else
                {
                    return BadRequest(new { message = "Failed to retrieve data from the API." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> ExportToPdf(InvoiceReportModel invoiceReport)
        {
            try
            {
                ApiResponseModel response = await APIServices.PostAsync(invoiceReport, "SupplierInvoice/GetInvoiceDetailsPdfReport");
                if (response.code == 200)
                {
                    var SupplierDetails = JsonConvert.DeserializeObject<InvoiceTotalAmount>(response.data.ToString());

                    var document = new Aspose.Pdf.Document
                    {
                        PageInfo = new PageInfo { Margin = new MarginInfo(20, 20, 20, 20) }
                    };

                    var pdfPage = document.Pages.Add();


                    Aspose.Pdf.Table secondTable = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "33% 33% 34%",
                        DefaultCellPadding = new MarginInfo(2, 2, 2, 2),
                        Border = new BorderInfo(BorderSide.All, 1f),
                        DefaultCellBorder = new BorderInfo(BorderSide.None),
                    };

                    var secondTableHeaderRow = secondTable.Rows.Add();
                    secondTableHeaderRow.Cells.Add("Site");
                    secondTableHeaderRow.Cells.Add("Supplier");
                    secondTableHeaderRow.Cells.Add("Company");

                    foreach (var cell in secondTableHeaderRow.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                    }

                    for (int i = 1; i < secondTableHeaderRow.Cells.Count; i++)
                    {
                        secondTableHeaderRow.Cells[i].Border = new BorderInfo(BorderSide.Left, 1f);
                    }

                    var secondTableRow1 = secondTable.Rows.Add();
                    secondTableRow1.Cells.Add(UserSession.SiteName ?? string.Empty);
                    secondTableRow1.Cells.Add(invoiceReport.SupplierName ?? string.Empty);
                    secondTableRow1.Cells.Add(invoiceReport.CompanyName ?? string.Empty);

                    foreach (var cell in secondTableRow1.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                    }

                    for (int i = 1; i < secondTableRow1.Cells.Count; i++)
                    {
                        secondTableRow1.Cells[i].Border = new BorderInfo(BorderSide.Left, 1f);
                    }

                    pdfPage.Paragraphs.Add(secondTable);

                    pdfPage.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment("\n\n"));


                    // Table 2
                    Aspose.Pdf.Table newTable = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "33% 33% 34%",
                        DefaultCellPadding = new MarginInfo(2, 2, 2, 2),
                        Border = new BorderInfo(BorderSide.All, 1f),
                        DefaultCellBorder = new BorderInfo(BorderSide.None),
                    };

                    var newTableHeaderRow = newTable.Rows.Add();
                    newTableHeaderRow.Cells.Add("Credit");
                    newTableHeaderRow.Cells.Add("Debit");
                    newTableHeaderRow.Cells.Add("Net Balance");

                    foreach (var cell in newTableHeaderRow.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                    }

                    for (int i = 1; i < newTableHeaderRow.Cells.Count; i++)
                    {
                        newTableHeaderRow.Cells[i].Border = new BorderInfo(BorderSide.Left, 1f);
                    }

                    var newTableRow1 = newTable.Rows.Add();
                    var creditCell = newTableRow1.Cells.Add(FormatIndianCurrency(SupplierDetails.TotalCreadit));
                    var debitCell = newTableRow1.Cells.Add(FormatIndianCurrency(SupplierDetails.TotalPurchase));

                    creditCell.DefaultCellTextState = new TextState { FontStyle = FontStyles.Bold };
                    debitCell.DefaultCellTextState = new TextState { FontStyle = FontStyles.Bold };

                    var netBalanceCell = newTableRow1.Cells.Add(FormatIndianCurrency(SupplierDetails.TotalPending));
                    netBalanceCell.Alignment = HorizontalAlignment.Right;
                    netBalanceCell.DefaultCellTextState = new TextState
                    {
                        ForegroundColor = Aspose.Pdf.Color.FromRgb(System.Drawing.Color.Green),
                        FontStyle = FontStyles.Bold
                    };


                    for (int i = 1; i < newTableRow1.Cells.Count; i++)
                    {
                        newTableRow1.Cells[i].Border = new BorderInfo(BorderSide.Left, 1f);
                    }

                    foreach (var cell in newTableRow1.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                    }

                    pdfPage.Paragraphs.Add(newTable);

                    pdfPage.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment("\n\n"));

                    // Table 3

                    Aspose.Pdf.Table table = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "16% 12% 16% 10% 17% 15% 15%",
                        DefaultCellPadding = new MarginInfo(2, 2, 2, 2),
                        Border = new BorderInfo(BorderSide.None),
                        DefaultCellBorder = new BorderInfo(BorderSide.None),
                    };

                    var headerRow = table.Rows.Add();
                    headerRow.Cells.Add("Invoice No");
                    headerRow.Cells.Add("Date");
                    headerRow.Cells.Add("Site");
                    headerRow.Cells.Add("Group");
                    headerRow.Cells.Add("Supplier");
                    headerRow.Cells.Add("Credit");
                    headerRow.Cells.Add("Debit");

                    foreach (var cell in headerRow.Cells)
                    {
                        cell.BackgroundColor = Aspose.Pdf.Color.Black;
                        var fragment = cell.Paragraphs[0] as TextFragment;
                        if (fragment != null)
                        {
                            fragment.TextState.ForegroundColor = Aspose.Pdf.Color.White;
                        }
                    }

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

                    foreach (var item in SupplierDetails.InvoiceList)
                    {
                        var row = table.Rows.Add();
                        string cellValue;

                        if (item.InvoiceNo == "Opening Balance")
                        {
                            cellValue = item.Description != null ? $"{item.InvoiceNo} ({item.Description})" : item.InvoiceNo;
                        }
                        else
                        {
                            cellValue = item.SupplierInvoiceNo;
                        }
                        row.Cells.Add(cellValue);
                        row.Cells.Add(item.Date?.ToString("dd-MM-yyyy"));
                        row.Cells.Add(item.SiteName != null ? item.SiteName : "");
                        row.Cells.Add(item.GroupName != null ? item.GroupName : "");
                        row.Cells.Add(item.SupplierName);
                        if (item.InvoiceNo == "PayOut")
                        {
                            row.Cells.Add("");
                            row.Cells.Add(FormatIndianCurrency(item.TotalAmount));
                        }
                        else
                        {
                            row.Cells.Add(FormatIndianCurrency(item.TotalAmount));
                            row.Cells.Add("");
                        }
                        var backgroundColor = table.Rows.Count % 2 == 0 ? Aspose.Pdf.Color.LightGray : Aspose.Pdf.Color.White;
                        foreach (var cell in row.Cells)
                        {
                            cell.BackgroundColor = backgroundColor;
                        }
                    }

                    var footerRow = table.Rows.Add();
                    footerRow.Cells.Add("Total");
                    footerRow.Cells.Add("");
                    footerRow.Cells.Add("");
                    footerRow.Cells.Add("");
                    footerRow.Cells.Add("");
                    footerRow.Cells.Add(FormatIndianCurrency(SupplierDetails.TotalCreadit));
                    footerRow.Cells.Add(FormatIndianCurrency(SupplierDetails.TotalPurchase));

                    TextState boldTextState = new TextState
                    {
                        FontStyle = FontStyles.Bold
                    };

                    foreach (var cell in footerRow.Cells)
                    {
                        cell.DefaultCellTextState = boldTextState;
                    }

                    pdfPage.Paragraphs.Add(table);

                    using (var streamout = new MemoryStream())
                    {
                        document.Save(streamout);
                        return new FileContentResult(streamout.ToArray(), "application/pdf")
                        {
                            FileDownloadName = Guid.NewGuid() + "_ReportList.pdf",
                        };
                    }
                }
                return RedirectToAction("ReportDetails");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public async Task<IActionResult> ExportToExcel(InvoiceReportModel invoiceReport)
        {
            try
            {
                ApiResponseModel response = await APIServices.PostAsync(invoiceReport, "SupplierInvoice/GetInvoiceDetailsPdfReport");

                if (response.code == 200)
                {
                    var SupplierDetails = JsonConvert.DeserializeObject<InvoiceTotalAmount>(response.data.ToString());

                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Report");

                        var row = 1;

                        // Header Row 1
                        ws.Cell(row, 1).Value = "Site";
                        ws.Cell(row, 2).Value = "Supplier";
                        ws.Cell(row, 3).Value = "Company";

                        var headerRange1 = ws.Range(row, 1, row, 3);
                        headerRange1.Style.Font.Bold = true;
                        headerRange1.Style.Fill.BackgroundColor = XLColor.Black;
                        headerRange1.Style.Font.FontColor = XLColor.White;
                        headerRange1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        headerRange1.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        headerRange1.Style.Border.BottomBorderColor = XLColor.Black;
                        headerRange1.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        headerRange1.Style.Border.RightBorderColor = XLColor.Black;

                        row++;
                        ws.Cell(row, 1).Value = UserSession.SiteName ?? string.Empty;
                        ws.Cell(row, 2).Value = invoiceReport.SupplierName ?? string.Empty;
                        ws.Cell(row, 3).Value = invoiceReport.CompanyName ?? string.Empty;

                        var dataRange1 = ws.Range(row, 1, row, 3);
                        dataRange1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        dataRange1.Style.Font.Bold = true;
                        dataRange1.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        dataRange1.Style.Border.BottomBorderColor = XLColor.Black;
                        dataRange1.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        dataRange1.Style.Border.LeftBorderColor = XLColor.Black;
                        dataRange1.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        dataRange1.Style.Border.RightBorderColor = XLColor.Black;

                        row += 2;

                        // Table-2

                        ws.Cell(row, 1).Value = "Credit";
                        ws.Cell(row, 2).Value = "Debit";
                        ws.Cell(row, 3).Value = "Net Balance";

                        var headerRange = ws.Range(row, 1, row, 3);
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.BackgroundColor = XLColor.Gray;
                        headerRange.Style.Font.FontColor = XLColor.White;
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        headerRange.Style.Border.BottomBorderColor = XLColor.Black;
                        headerRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        headerRange.Style.Border.RightBorderColor = XLColor.Black;

                        row++;
                        ws.Cell(row, 1).Value = FormatIndianCurrency(SupplierDetails.TotalCreadit);
                        ws.Cell(row, 2).Value = FormatIndianCurrency(SupplierDetails.TotalPurchase);
                        ws.Cell(row, 3).Value = FormatIndianCurrency(SupplierDetails.TotalPending);

                        var dataRange = ws.Range(row, 1, row, 3);
                        dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(row, 1).Style.Font.Bold = true;
                        ws.Cell(row, 2).Style.Font.Bold = true;
                        ws.Cell(row, 3).Style.Font.Bold = true;
                        ws.Cell(row, 3).Style.Font.FontColor = XLColor.Green;
                        dataRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        dataRange.Style.Border.BottomBorderColor = XLColor.Black;
                        dataRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        dataRange.Style.Border.LeftBorderColor = XLColor.Black;
                        dataRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        dataRange.Style.Border.RightBorderColor = XLColor.Black;

                        // Header Row 3
                        row += 2;

                        ws.Cell(row, 1).Value = "Invoice No";
                        ws.Cell(row, 2).Value = "Date";
                        ws.Cell(row, 3).Value = "Site";
                        ws.Cell(row, 4).Value = "Group";
                        ws.Cell(row, 5).Value = "Supplier";
                        ws.Cell(row, 6).Value = "Credit";
                        ws.Cell(row, 7).Value = "Debit";

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

                        foreach (var item in SupplierDetails.InvoiceList)
                        {
                            string cellValue;
                            if (item.InvoiceNo == "Opening Balance")
                            {
                                cellValue = item.Description != null ? $"{item.InvoiceNo} ({item.Description})" : item.InvoiceNo;
                            }
                            else
                            {
                                cellValue = item.SupplierInvoiceNo;
                            }
                            ws.Cell(row, 1).Value = cellValue;
                            ws.Cell(row, 2).Value = item.Date?.ToString("dd-MM-yyyy") ?? string.Empty;
                            ws.Cell(row, 3).Value = item.SiteName ?? string.Empty;
                            ws.Cell(row, 4).Value = item.GroupName ?? string.Empty;
                            ws.Cell(row, 5).Value = item.SupplierName ?? string.Empty;
                            if (item.InvoiceNo == "PayOut")
                            {
                                ws.Cell(row, 6).Value = "";
                                ws.Cell(row, 7).Value = FormatIndianCurrency(item.TotalAmount);
                            }
                            else
                            {

                                ws.Cell(row, 6).Value = FormatIndianCurrency(item.TotalAmount);
                                ws.Cell(row, 7).Value = "";
                            }
                            row++;
                        }

                        ws.Cell(row, 1).Value = "Total";
                        ws.Cell(row, 2).Value = string.Empty;
                        ws.Cell(row, 3).Value = string.Empty;
                        ws.Cell(row, 4).Value = string.Empty;
                        ws.Cell(row, 5).Value = string.Empty;
                        ws.Cell(row, 6).Value = FormatIndianCurrency(SupplierDetails.TotalCreadit);
                        ws.Cell(row, 7).Value = FormatIndianCurrency(SupplierDetails.TotalPurchase);

                        var footerRange2 = ws.Range(row, 1, row, 7);
                        footerRange2.Style.Font.Bold = true;

                        using (var stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            var fileName = $"{Guid.NewGuid()}_ReportDetails.xlsx";
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                        }
                    }
                }
                return RedirectToAction("ReportDetails");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePayoutDetails(Guid InvoiceId)
        {
            try
            {
                ApiResponseModel payout = await APIServices.PostAsync("", "SupplierInvoice/DeletePayoutDetails?InvoiceId=" + InvoiceId);
                if (payout.code == 200)
                {
                    return Ok(new { Message = string.Format(payout.message), Code = payout.code });
                }
                else
                {
                    return Ok(new { Message = string.Format(payout.message), Code = payout.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetPayoutDetailsbyId(Guid InvoiceId)
        {
            try
            {
                SupplierInvoiceModel payoutDetails = new SupplierInvoiceModel();
                ApiResponseModel response = await APIServices.GetAsync("", "SupplierInvoice/GetPayoutDetailsbyId?InvoiceId=" + InvoiceId);
                if (response.code == 200)
                {
                    payoutDetails = JsonConvert.DeserializeObject<SupplierInvoiceModel>(response.data.ToString());
                }
                return new JsonResult(payoutDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePayoutDetails(SupplierInvoiceModel updatepayoutDetails)
        {
            try
            {
                ApiResponseModel payout = await APIServices.PostAsync(updatepayoutDetails, "SupplierInvoice/UpdatePayoutDetails");
                if (payout.code == 200)
                {
                    return Ok(new { Message = string.Format(payout.message), Code = payout.code });
                }
                else
                {
                    return Ok(new { Message = string.Format(payout.message), Code = payout.code });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportNetReportToPDF(InvoiceReportModel PayOutReport)
        {
            try
            {
                ApiResponseModel response = await APIServices.PostAsync(PayOutReport, "SupplierInvoice/GetPayoutInvoiceDetailsPdfReport");

                if (response.code == 200)
                {
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

                    var NetInvoiceDetails = JsonConvert.DeserializeObject<InvoiceTotalAmount>(response.data.ToString());

                    var document = new Aspose.Pdf.Document
                    {
                        PageInfo = new PageInfo { Margin = new MarginInfo(20, 20, 20, 20) }
                    };

                    var pdfPage = document.Pages.Add();

                    // Table 1
                    Aspose.Pdf.Table secondTable = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "33% 33% 34%",
                        DefaultCellPadding = new MarginInfo(2, 2, 2, 2),
                        Border = new BorderInfo(BorderSide.All, 1f),
                        DefaultCellBorder = new BorderInfo(BorderSide.None),
                    };

                    var secondTableHeaderRow = secondTable.Rows.Add();
                    secondTableHeaderRow.Cells.Add("Site");
                    secondTableHeaderRow.Cells.Add("Supplier");
                    secondTableHeaderRow.Cells.Add("Company");

                    foreach (var cell in secondTableHeaderRow.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                    }

                    for (int i = 1; i < secondTableHeaderRow.Cells.Count; i++)
                    {
                        secondTableHeaderRow.Cells[i].Border = new BorderInfo(BorderSide.Left, 1f);
                    }

                    var secondTableRow1 = secondTable.Rows.Add();
                    secondTableRow1.Cells.Add(UserSession.SiteName ?? string.Empty);
                    secondTableRow1.Cells.Add(PayOutReport.SupplierName ?? string.Empty);
                    secondTableRow1.Cells.Add(PayOutReport.CompanyName ?? string.Empty);

                    foreach (var cell in secondTableRow1.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                    }

                    for (int i = 1; i < secondTableRow1.Cells.Count; i++)
                    {
                        secondTableRow1.Cells[i].Border = new BorderInfo(BorderSide.Left, 1f);
                    }

                    pdfPage.Paragraphs.Add(secondTable);

                    pdfPage.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment("\n\n"));

                    // Table 2
                    Aspose.Pdf.Table newTable = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "33% 33% 34%",
                        DefaultCellPadding = new MarginInfo(2, 2, 2, 2),
                        Border = new BorderInfo(BorderSide.All, 1f),
                        DefaultCellBorder = new BorderInfo(BorderSide.None),
                    };

                    var newTableHeaderRow = newTable.Rows.Add();
                    newTableHeaderRow.Cells.Add("Credit");
                    newTableHeaderRow.Cells.Add("Debit");
                    newTableHeaderRow.Cells.Add("Net Balance");

                    foreach (var cell in newTableHeaderRow.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                    }

                    for (int i = 1; i < newTableHeaderRow.Cells.Count; i++)
                    {
                        newTableHeaderRow.Cells[i].Border = new BorderInfo(BorderSide.Left, 1f);
                    }

                    var newTableRow1 = newTable.Rows.Add();
                    var creditCell = newTableRow1.Cells.Add(FormatIndianCurrency(NetInvoiceDetails.TotalCreadit));
                    var debitCell = newTableRow1.Cells.Add(FormatIndianCurrency(NetInvoiceDetails.TotalPurchase));

                    creditCell.DefaultCellTextState = new TextState { FontStyle = FontStyles.Bold };
                    debitCell.DefaultCellTextState = new TextState { FontStyle = FontStyles.Bold };

                    var netBalanceCell = newTableRow1.Cells.Add(FormatIndianCurrency(NetInvoiceDetails.TotalPending));
                    netBalanceCell.Alignment = HorizontalAlignment.Right;
                    netBalanceCell.DefaultCellTextState = new TextState
                    {
                        ForegroundColor = Aspose.Pdf.Color.FromRgb(System.Drawing.Color.Green),
                        FontStyle = FontStyles.Bold
                    };


                    for (int i = 1; i < newTableRow1.Cells.Count; i++)
                    {
                        newTableRow1.Cells[i].Border = new BorderInfo(BorderSide.Left, 1f);
                    }

                    foreach (var cell in newTableRow1.Cells)
                    {
                        cell.Alignment = HorizontalAlignment.Center;
                    }

                    pdfPage.Paragraphs.Add(newTable);

                    pdfPage.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment("\n\n"));



                    // Table 3
                    string currencyFormat = "#,##,##0.00";
                    var table = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "40% 20% 20% 20%",
                        DefaultCellPadding = new MarginInfo(2, 2, 2, 2),
                        Border = new BorderInfo(BorderSide.None),
                        DefaultCellBorder = new BorderInfo(BorderSide.None),
                    };

                    var headerRow = table.Rows.Add();
                    headerRow.Cells.Add("Supplier");
                    headerRow.Cells.Add("Credit");
                    headerRow.Cells.Add("Debit");
                    headerRow.Cells.Add("Net Total");

                    foreach (var cell in headerRow.Cells)
                    {
                        cell.BackgroundColor = Aspose.Pdf.Color.Black;
                        var fragment = cell.Paragraphs[0] as TextFragment;
                        if (fragment != null)
                        {
                            fragment.TextState.ForegroundColor = Aspose.Pdf.Color.White;
                        }
                    }

                    decimal yougavetotal = 0;
                    decimal yougettotal = 0;
                    decimal nettotal = 0;
                    decimal netbalance = 0;

                    foreach (var item in NetInvoiceDetails.InvoiceList)
                    {
                        var row = table.Rows.Add();
                        row.Cells.Add(item.SupplierName);
                        row.Cells.Add(FormatIndianCurrency(item.NonPayOutTotalAmount));
                        yougettotal += item.NonPayOutTotalAmount;
                        row.Cells.Add(FormatIndianCurrency(item.PayOutTotalAmount));
                        yougavetotal += item.PayOutTotalAmount;
                        netbalance = item.NonPayOutTotalAmount - item.PayOutTotalAmount;
                        row.Cells.Add(FormatIndianCurrency(netbalance));

                        var backgroundColor = table.Rows.Count % 2 == 0 ? Aspose.Pdf.Color.LightGray : Aspose.Pdf.Color.White;
                        foreach (var cell in row.Cells)
                        {
                            cell.BackgroundColor = backgroundColor;
                        }
                    }
                    var footerRow = table.Rows.Add();
                    footerRow.Cells.Add("Total");
                    footerRow.Cells.Add(FormatIndianCurrency(NetInvoiceDetails.TotalCreadit));
                    footerRow.Cells.Add(FormatIndianCurrency(NetInvoiceDetails.TotalPurchase));
                    footerRow.Cells.Add(FormatIndianCurrency(NetInvoiceDetails.TotalPending));

                    TextState boldTextState = new TextState
                    {
                        FontStyle = FontStyles.Bold
                    };

                    foreach (var cell in footerRow.Cells)
                    {
                        cell.DefaultCellTextState = boldTextState;
                    }
                    pdfPage.Paragraphs.Add(table);

                    using (var streamout = new MemoryStream())
                    {
                        document.Save(streamout);
                        return new FileContentResult(streamout.ToArray(), "application/pdf")
                        {
                            FileDownloadName = Guid.NewGuid() + "_NetReportList.pdf",
                        };
                    }
                }
                return RedirectToAction("ReportDetails");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public async Task<IActionResult> ExportNetReportToExcel(InvoiceReportModel PayOutReport)
        {
            try
            {
                ApiResponseModel response = await APIServices.PostAsync(PayOutReport, "SupplierInvoice/GetPayoutInvoiceDetailsPdfReport");

                if (response.code == 200)
                {
                    var NetInvoiceDetails = JsonConvert.DeserializeObject<InvoiceTotalAmount>(response.data.ToString());

                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Report");

                        int row = 1;

                        // Table-1
                        ws.Cell(row, 1).Value = "Site";
                        ws.Cell(row, 2).Value = "Supplier";
                        ws.Cell(row, 3).Value = "Company";

                        var headerRangeNewTable = ws.Range(row, 1, row, 3);
                        headerRangeNewTable.Style.Font.Bold = true;
                        headerRangeNewTable.Style.Fill.BackgroundColor = XLColor.Gray;
                        headerRangeNewTable.Style.Font.FontColor = XLColor.White;
                        headerRangeNewTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        headerRangeNewTable.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        headerRangeNewTable.Style.Border.BottomBorderColor = XLColor.Black;
                        headerRangeNewTable.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        headerRangeNewTable.Style.Border.RightBorderColor = XLColor.Black;

                        row++;
                        ws.Cell(row, 1).Value = UserSession.SiteName ?? string.Empty;
                        ws.Cell(row, 2).Value = PayOutReport.SupplierName ?? string.Empty;
                        ws.Cell(row, 3).Value = PayOutReport.CompanyName ?? string.Empty;

                        var dataRangeNewTable = ws.Range(row, 1, row, 3);
                        dataRangeNewTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(row, 1).Style.Font.Bold = true;
                        ws.Cell(row, 2).Style.Font.Bold = true;
                        ws.Cell(row, 3).Style.Font.Bold = true;
                        dataRangeNewTable.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        dataRangeNewTable.Style.Border.BottomBorderColor = XLColor.Black;
                        dataRangeNewTable.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        dataRangeNewTable.Style.Border.LeftBorderColor = XLColor.Black;
                        dataRangeNewTable.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        dataRangeNewTable.Style.Border.RightBorderColor = XLColor.Black;


                        row += 2;

                        // Table-2

                        ws.Cell(row, 1).Value = "Credit";
                        ws.Cell(row, 2).Value = "Debit";
                        ws.Cell(row, 3).Value = "Net Balance";

                        var headerRange1 = ws.Range(row, 1, row, 3);
                        headerRange1.Style.Font.Bold = true;
                        headerRange1.Style.Fill.BackgroundColor = XLColor.Gray;
                        headerRange1.Style.Font.FontColor = XLColor.White;
                        headerRange1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        headerRange1.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        headerRange1.Style.Border.BottomBorderColor = XLColor.Black;
                        headerRange1.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        headerRange1.Style.Border.RightBorderColor = XLColor.Black;

                        row++;
                        ws.Cell(row, 1).Value = FormatIndianCurrency(NetInvoiceDetails.TotalCreadit);
                        ws.Cell(row, 2).Value = FormatIndianCurrency(NetInvoiceDetails.TotalPurchase);
                        ws.Cell(row, 3).Value = FormatIndianCurrency(NetInvoiceDetails.TotalPending);

                        var dataRange1 = ws.Range(row, 1, row, 3);
                        dataRange1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(row, 1).Style.Font.Bold = true;
                        ws.Cell(row, 2).Style.Font.Bold = true;
                        ws.Cell(row, 3).Style.Font.Bold = true;
                        ws.Cell(row, 3).Style.Font.FontColor = XLColor.Green;
                        dataRange1.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        dataRange1.Style.Border.BottomBorderColor = XLColor.Black;
                        dataRange1.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        dataRange1.Style.Border.LeftBorderColor = XLColor.Black;
                        dataRange1.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        dataRange1.Style.Border.RightBorderColor = XLColor.Black;

                        // Table-3

                        row += 2;

                        ws.Cell(row, 1).Value = "Supplier";
                        ws.Cell(row, 2).Value = "Credit";
                        ws.Cell(row, 3).Value = "Debit";
                        ws.Cell(row, 4).Value = "Net Total";

                        var headerRange3 = ws.Range(row, 1, row, 4);
                        headerRange3.Style.Font.Bold = true;
                        headerRange3.Style.Fill.BackgroundColor = XLColor.Gray;
                        headerRange3.Style.Font.FontColor = XLColor.White;
                        headerRange3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        headerRange3.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        headerRange3.Style.Border.BottomBorderColor = XLColor.Black;
                        headerRange3.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        headerRange3.Style.Border.RightBorderColor = XLColor.Black;

                        decimal yougavetotal = 0;
                        decimal yougettotal = 0;
                        decimal nettotal = 0;
                        decimal netbalance = 0;

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

                        row++;
                        foreach (var item in NetInvoiceDetails.InvoiceList)
                        {
                            ws.Cell(row, 1).Value = item.SupplierName;
                            ws.Cell(row, 2).Value = FormatIndianCurrency(item.NonPayOutTotalAmount);
                            ws.Cell(row, 3).Value = FormatIndianCurrency(item.PayOutTotalAmount);
                            yougavetotal += item.PayOutTotalAmount;
                            yougettotal += item.NonPayOutTotalAmount;
                            netbalance = item.NonPayOutTotalAmount - item.PayOutTotalAmount;
                            ws.Cell(row, 4).Value = FormatIndianCurrency(netbalance);
                            row++;
                        }

                        nettotal = yougettotal - yougavetotal;

                        ws.Cell(row, 1).Value = "Total";
                        ws.Cell(row, 2).Value = FormatIndianCurrency(yougettotal);
                        ws.Cell(row, 3).Value = FormatIndianCurrency(yougavetotal);
                        ws.Cell(row, 4).Value = FormatIndianCurrency(nettotal);


                        ws.Columns().AdjustToContents();

                        using (var stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            string fileName = Guid.NewGuid() + "_NetReportDetails.xlsx";
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                        }
                    }
                }
                return RedirectToAction("ReportDetails");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}

