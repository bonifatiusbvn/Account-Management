using AccountManagement.DBContext.Models.API;
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
        [HttpPost]
        public async Task<IActionResult> GetSupplierInvoiceDetailsReport(InvoiceReportModel invoiceReport)
        {
            try
            {
                List<SupplierInvoiceModel> SupplierDetails = new List<SupplierInvoiceModel>();

                InvoiceReportModel invoiceReportModel = new InvoiceReportModel
                {
                    SiteId = !string.IsNullOrEmpty(UserSession.SiteId) && Guid.TryParse(UserSession.SiteId, out Guid parsedSiteId) ? (Guid?)parsedSiteId : null,
                    CompanyId = invoiceReport.CompanyId,
                    SupplierId = invoiceReport.SupplierId,
                    filterType = invoiceReport.filterType,
                    startDate = invoiceReport.startDate,
                    endDate = invoiceReport.endDate,
                    SelectedYear = invoiceReport.SelectedYear,
                    GroupName = invoiceReport.GroupName,
                };

                ApiResponseModel response = await APIServices.PostAsync(invoiceReportModel, "SupplierInvoice/GetSupplierInvoiceDetailsReport");
                if (response.code == 200)
                {
                    SupplierDetails = JsonConvert.DeserializeObject<List<SupplierInvoiceModel>>(response.data.ToString());
                }
                return PartialView("~/Views/Report/_ReportDetailsPartial.cshtml", SupplierDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportToPdf(InvoiceReportModel invoiceReport)
        {
            try
            {

                InvoiceReportModel invoiceReportModel = new InvoiceReportModel
                {
                    SiteId = !string.IsNullOrEmpty(UserSession.SiteId) && Guid.TryParse(UserSession.SiteId, out Guid parsedSiteId) ? (Guid?)parsedSiteId : null,
                    CompanyId = invoiceReport.CompanyId,
                    SupplierId = invoiceReport.SupplierId,
                    filterType = invoiceReport.filterType,
                    startDate = invoiceReport.startDate,
                    endDate = invoiceReport.endDate,
                    SelectedYear = invoiceReport.SelectedYear,
                    GroupName = invoiceReport.GroupName,
                    sortDates = invoiceReport.sortDates,
                };

                ApiResponseModel response = await APIServices.PostAsync(invoiceReportModel, "SupplierInvoice/GetSupplierInvoiceDetailsReport");

                if (response.code == 200)
                {
                    var SupplierDetails = JsonConvert.DeserializeObject<List<SupplierInvoiceModel>>(response.data.ToString());

                    var document = new Aspose.Pdf.Document
                    {
                        PageInfo = new PageInfo { Margin = new MarginInfo(20, 20, 20, 20) }
                    };

                    var pdfPage = document.Pages.Add();

                    Aspose.Pdf.Table table = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "10% 11% 12% 8% 12% 12% 11% 11% 11%",
                        DefaultCellPadding = new MarginInfo(3, 3, 3, 3),
                        Border = new BorderInfo(BorderSide.All, .5f, Aspose.Pdf.Color.Black),
                        DefaultCellBorder = new BorderInfo(BorderSide.All, .2f, Aspose.Pdf.Color.Black),
                    };

                    var headerRow = table.Rows.Add();
                    headerRow.Cells.Add("Invoice No");
                    headerRow.Cells.Add("Date");
                    headerRow.Cells.Add("Site");
                    headerRow.Cells.Add("Group");
                    headerRow.Cells.Add("Company");
                    headerRow.Cells.Add("Supplier");
                    headerRow.Cells.Add("Debit");
                    headerRow.Cells.Add("Credit");
                    headerRow.Cells.Add("Net Total");


                    decimal yougavetotal = 0;
                    decimal yougettotal = 0;
                    decimal nettotal = 0;

                    foreach (var item in SupplierDetails)
                    {
                        var row = table.Rows.Add();
                        row.Cells.Add(item.InvoiceNo == "PayOut" ? item.InvoiceNo : item.SupplierInvoiceNo);
                        row.Cells.Add(item.Date?.ToString("dd-MM-yyyy"));
                        row.Cells.Add(item.SiteName);
                        row.Cells.Add(item.GroupName != null ? item.GroupName : "");
                        row.Cells.Add(item.CompanyName);
                        row.Cells.Add(item.SupplierName);
                        if (item.InvoiceNo == "PayOut")
                        {
                            row.Cells.Add(item.TotalAmount.ToString());
                            row.Cells.Add("0.00");
                            yougavetotal += item.TotalAmount;
                        }
                        else
                        {
                            row.Cells.Add("0.00");
                            row.Cells.Add(item.TotalAmount.ToString());
                            yougettotal += item.TotalAmount;
                        }
                        row.Cells.Add();
                    }

                    nettotal = yougettotal-yougavetotal;
                    var footerRow = table.Rows.Add();
                    footerRow.Cells.Add("Total");
                    footerRow.Cells.Add("");
                    footerRow.Cells.Add("");
                    footerRow.Cells.Add("");
                    footerRow.Cells.Add("");
                    footerRow.Cells.Add("");
                    footerRow.Cells.Add(yougavetotal.ToString());
                    footerRow.Cells.Add(yougettotal.ToString());
                    footerRow.Cells.Add(nettotal.ToString());

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
                InvoiceReportModel invoiceReportModel = new InvoiceReportModel
                {
                    SiteId = !string.IsNullOrEmpty(UserSession.SiteId) && Guid.TryParse(UserSession.SiteId, out Guid parsedSiteId) ? (Guid?)parsedSiteId : null,
                    CompanyId = invoiceReport.CompanyId,
                    SupplierId = invoiceReport.SupplierId,
                    filterType = invoiceReport.filterType,
                    startDate = invoiceReport.startDate,
                    endDate = invoiceReport.endDate,
                    SelectedYear = invoiceReport.SelectedYear,
                    GroupName = invoiceReport.GroupName,
                    sortDates = invoiceReport.sortDates,
                };

                ApiResponseModel response = await APIServices.PostAsync(invoiceReportModel, "SupplierInvoice/GetSupplierInvoiceDetailsReport");

                if (response.data.Count != 0)
                {
                    var SupplierDetails = JsonConvert.DeserializeObject<List<SupplierInvoiceModel>>(response.data.ToString());

                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Report");

                        ws.Cell(1, 1).Value = "Invoice No";
                        ws.Cell(1, 2).Value = "Date";
                        ws.Cell(1, 3).Value = "Site";
                        ws.Cell(1, 4).Value = "Group";
                        ws.Cell(1, 5).Value = "Company";
                        ws.Cell(1, 6).Value = "Supplier";
                        ws.Cell(1, 7).Value = "Debit";
                        ws.Cell(1, 8).Value = "Credit";
                        ws.Cell(1, 9).Value = "Net Total";

                        decimal yougavetotal = 0;
                        decimal yougettotal = 0;
                        decimal nettotal = 0;

                        int row = 2;
                        foreach (var item in SupplierDetails)
                        {
                            ws.Cell(row, 1).Value = item.InvoiceNo == "PayOut" ? item.InvoiceNo : item.SupplierInvoiceNo;
                            ws.Cell(row, 2).Value = item.Date?.ToString("dd-MM-yyyy");
                            ws.Cell(row, 3).Value = item.SiteName;
                            ws.Cell(row, 4).Value = item.GroupName != null ? item.GroupName : "";
                            ws.Cell(row, 5).Value = item.CompanyName;
                            ws.Cell(row, 6).Value = item.SupplierName;
                            if (item.InvoiceNo == "PayOut")
                            {
                                ws.Cell(row, 7).Value = item.TotalAmount;
                                ws.Cell(row, 8).Value = "0.00";
                                yougavetotal += item.TotalAmount;
                            }
                            else
                            {
                                ws.Cell(row, 7).Value = "0.00";
                                ws.Cell(row, 8).Value = item.TotalAmount;
                                yougettotal += item.TotalAmount;
                            }
                            ws.Cell(row, 9).Value = "";
                            row++;
                        }

                        nettotal = yougettotal-yougavetotal;

                        ws.Cell(row, 1).Value = "Total";
                        ws.Cell(row, 2).Value = "";
                        ws.Cell(row, 3).Value = "";
                        ws.Cell(row, 4).Value = "";
                        ws.Cell(row, 5).Value = "";
                        ws.Cell(row, 6).Value = "";
                        ws.Cell(row, 7).Value = yougavetotal;
                        ws.Cell(row, 8).Value = yougettotal;
                        ws.Cell(row, 9).Value = nettotal;


                        var headerRange = ws.Range(1, 1, 1, 9);
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.BackgroundColor = XLColor.Gray;
                        headerRange.Style.Font.FontColor = XLColor.White;
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        var totalRange = ws.Range(row, 1, row, 9);
                        totalRange.Style.Font.Bold = true;

                        using (var stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            string fileName = Guid.NewGuid() + "_ReportDetails.xlsx";
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
                ApiResponseModel response = await APIServices.PostAsync(PayOutReport, "SupplierInvoice/GetInvoiceDetailsById");

                if (response.code == 200)
                {
                    var NetInvoiceDetails = JsonConvert.DeserializeObject<InvoiceTotalAmount>(response.data.ToString());

                    var document = new Aspose.Pdf.Document
                    {
                        PageInfo = new PageInfo { Margin = new MarginInfo(20, 20, 20, 20) }
                    };

                    var pdfPage = document.Pages.Add();

                    // Table 1
                    Aspose.Pdf.Table newTable = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "33% 33% 34%",
                        DefaultCellPadding = new MarginInfo(3, 3, 3, 3),
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
                    var creditCell = newTableRow1.Cells.Add("₹" + NetInvoiceDetails.TotalCreadit.ToString("N2"));
                    var debitCell = newTableRow1.Cells.Add("₹" + NetInvoiceDetails.TotalPurchase.ToString("N2"));

                    creditCell.DefaultCellTextState = new TextState { FontStyle = FontStyles.Bold };
                    debitCell.DefaultCellTextState = new TextState { FontStyle = FontStyles.Bold };

                    var netBalanceCell = newTableRow1.Cells.Add("₹" + NetInvoiceDetails.TotalPending.ToString("N2"));
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

                    // Table 2
                    var table = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "25% 25% 17% 17% 16%",
                        DefaultCellPadding = new MarginInfo(3, 3, 3, 3),
                        Border = new BorderInfo(BorderSide.All, .5f, Aspose.Pdf.Color.Black),
                        DefaultCellBorder = new BorderInfo(BorderSide.All, .2f, Aspose.Pdf.Color.Black),
                    };

                    var headerRow = table.Rows.Add();
                    headerRow.Cells.Add("Company");
                    headerRow.Cells.Add("Supplier");
                    headerRow.Cells.Add("Debit");
                    headerRow.Cells.Add("Credit");
                    headerRow.Cells.Add("Net Total");

                    decimal yougavetotal = 0;
                    decimal yougettotal = 0;
                    decimal nettotal = 0;
                    decimal netbalance = 0;

                    foreach (var item in NetInvoiceDetails.InvoiceList)
                    {
                        var row = table.Rows.Add();
                        row.Cells.Add(item.CompanyName);
                        row.Cells.Add(item.SupplierName);
                        row.Cells.Add(item.PayOutTotalAmount.ToString("F2"));
                        yougettotal += item.PayOutTotalAmount;
                        row.Cells.Add(item.NonPayOutTotalAmount.ToString("F2"));
                        yougavetotal += item.NonPayOutTotalAmount;
                        netbalance = item.NonPayOutTotalAmount-item.PayOutTotalAmount;
                        row.Cells.Add(netbalance.ToString("F2")); 
                    }

                    nettotal = yougavetotal - yougettotal;
                    var footerRow = table.Rows.Add();
                    footerRow.Cells.Add("Total");
                    footerRow.Cells.Add("");
                    footerRow.Cells.Add(yougettotal.ToString("F2"));
                    footerRow.Cells.Add(yougavetotal.ToString("F2"));
                    footerRow.Cells.Add(nettotal.ToString("F2"));

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
                ApiResponseModel response = await APIServices.PostAsync(PayOutReport, "SupplierInvoice/GetInvoiceDetailsById");

                if (response.code == 200)
                {
                    var NetInvoiceDetails = JsonConvert.DeserializeObject<InvoiceTotalAmount>(response.data.ToString());

                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Report");

                        // Table-1
                        ws.Cell(1, 1).Value = "Credit";
                        ws.Cell(1, 2).Value = "Debit";
                        ws.Cell(1, 3).Value = "Net Balance";

                        var headerRange1 = ws.Range(1, 1, 1, 3);
                        headerRange1.Style.Font.Bold = true;
                        headerRange1.Style.Fill.BackgroundColor = XLColor.Gray;
                        headerRange1.Style.Font.FontColor = XLColor.White;
                        headerRange1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        headerRange1.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        headerRange1.Style.Border.BottomBorderColor = XLColor.Black;
                        headerRange1.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        headerRange1.Style.Border.RightBorderColor = XLColor.Black;

                     
                        int row = 2;
                        ws.Cell(row, 1).Value = "₹" + NetInvoiceDetails.TotalCreadit.ToString("N2");
                        ws.Cell(row, 2).Value = "₹" + NetInvoiceDetails.TotalPurchase.ToString("N2");
                        ws.Cell(row, 3).Value = "₹" + NetInvoiceDetails.TotalPending.ToString("N2");

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

               
                        row += 3;

                        // Table-2
                        ws.Cell(row, 1).Value = "Company";
                        ws.Cell(row, 2).Value = "Supplier";
                        ws.Cell(row, 3).Value = "Debit";
                        ws.Cell(row, 4).Value = "Credit";
                        ws.Cell(row, 5).Value = "Net Total";

                        var headerRange2 = ws.Range(row, 1, row, 5);
                        headerRange2.Style.Font.Bold = true;
                        headerRange2.Style.Fill.BackgroundColor = XLColor.Gray;
                        headerRange2.Style.Font.FontColor = XLColor.White;
                        headerRange2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        headerRange2.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        headerRange2.Style.Border.BottomBorderColor = XLColor.Black;
                        headerRange2.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        headerRange2.Style.Border.RightBorderColor = XLColor.Black;

                        decimal yougavetotal = 0;
                        decimal yougettotal = 0;
                        decimal nettotal = 0;
                        decimal netbalance = 0;

                        row++;
                        foreach (var item in NetInvoiceDetails.InvoiceList)
                        {
                            ws.Cell(row, 1).Value = item.CompanyName;
                            ws.Cell(row, 2).Value = item.SupplierName;
                            ws.Cell(row, 3).Value = item.PayOutTotalAmount;
                            ws.Cell(row, 4).Value = item.NonPayOutTotalAmount;
                          
                            yougavetotal += item.PayOutTotalAmount;
                            yougettotal += item.NonPayOutTotalAmount;
                            netbalance = item.NonPayOutTotalAmount-item.PayOutTotalAmount;
                            ws.Cell(row, 5).Value = netbalance;
                            row++;
                        }

                        nettotal = yougettotal - yougavetotal;

                        ws.Cell(row, 1).Value = "Total";
                        ws.Cell(row, 2).Value = "";
                        ws.Cell(row, 3).Value = yougavetotal;
                        ws.Cell(row, 4).Value = yougettotal;
                        ws.Cell(row, 5).Value = nettotal;

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

