using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManegments.Web.Helper;
using AccountManegments.Web.Models;
using Aspose.Pdf;
using ClosedXML.Excel;
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

                TempData["SiteId"] = UserSession.SiteId;
                TempData["CompanyId"] = invoiceReport.CompanyId;
                TempData["SupplierId"] = invoiceReport.SupplierId;
                TempData["filterType"] = invoiceReport.filterType;
                TempData["startDate"] = invoiceReport.startDate;
                TempData["endDate"] = invoiceReport.endDate;

                InvoiceReportModel invoiceReportModel = new InvoiceReportModel
                {
                    SiteId = !string.IsNullOrEmpty(UserSession.SiteId) && Guid.TryParse(UserSession.SiteId, out Guid parsedSiteId) ? (Guid?)parsedSiteId : null,
                    CompanyId = invoiceReport.CompanyId,
                    SupplierId = invoiceReport.SupplierId,
                    filterType = invoiceReport.filterType,
                    startDate = invoiceReport.startDate,
                    endDate = invoiceReport.endDate
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
                ApiResponseModel response = await APIServices.PostAsync(invoiceReport, "SupplierInvoice/GetSupplierInvoiceDetailsReport");

                if (response.data.Count != 0)
                {
                    var SupplierDetails = JsonConvert.DeserializeObject<List<SupplierInvoiceModel>>(response.data.ToString());

                    var document = new Aspose.Pdf.Document
                    {
                        PageInfo = new PageInfo { Margin = new MarginInfo(25, 25, 25, 40) }
                    };

                    var pdfPage = document.Pages.Add();

                    Aspose.Pdf.Table table = new Aspose.Pdf.Table
                    {
                        ColumnWidths = "18% 15% 23% 20% 12% 12%",
                        DefaultCellPadding = new MarginInfo(5, 5, 5, 5),
                        Border = new BorderInfo(BorderSide.All, .5f, Aspose.Pdf.Color.Black),
                        DefaultCellBorder = new BorderInfo(BorderSide.All, .2f, Aspose.Pdf.Color.Black),
                    };

                    var headerRow = table.Rows.Add();
                    headerRow.Cells.Add("Invoice No");
                    headerRow.Cells.Add("Date");
                    headerRow.Cells.Add("Company");
                    headerRow.Cells.Add("Supplier");
                    headerRow.Cells.Add("Debit");
                    headerRow.Cells.Add("Credit");


                    decimal yougavetotal = 0;
                    decimal yougettotal = 0;

                    foreach (var item in SupplierDetails)
                    {
                        var row = table.Rows.Add();
                        row.Cells.Add(item.InvoiceNo);
                        row.Cells.Add(item.Date?.ToString("MM-dd-yyyy"));
                        row.Cells.Add(item.CompanyName);
                        row.Cells.Add(item.SupplierName);
                        if (item.InvoiceNo == "PayOut")
                        {
                            row.Cells.Add(item.TotalAmount.ToString());
                            row.Cells.Add("");
                            yougavetotal += item.TotalAmount;
                        }
                        else
                        {
                            row.Cells.Add("");
                            row.Cells.Add(item.TotalAmount.ToString());
                            yougettotal += item.TotalAmount;
                        }
                    }

                    var footerRow = table.Rows.Add();
                    footerRow.Cells.Add("Total");
                    footerRow.Cells.Add("");
                    footerRow.Cells.Add(yougavetotal.ToString());
                    footerRow.Cells.Add(yougettotal.ToString());

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
                ApiResponseModel response = await APIServices.PostAsync(invoiceReport, "SupplierInvoice/GetSupplierInvoiceDetailsReport");

                if (response.data.Count != 0)
                {
                    var SupplierDetails = JsonConvert.DeserializeObject<List<SupplierInvoiceModel>>(response.data.ToString());

                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Report");

                        ws.Cell(1, 1).Value = "Invoice No";
                        ws.Cell(1, 2).Value = "Date";
                        ws.Cell(1, 3).Value = "Company";
                        ws.Cell(1, 4).Value = "Supplier";
                        ws.Cell(1, 5).Value = "Debit";
                        ws.Cell(1, 6).Value = "Credit";

                        decimal yougavetotal = 0;
                        decimal yougettotal = 0;

                        int row = 2;
                        foreach (var item in SupplierDetails)
                        {
                            ws.Cell(row, 1).Value = item.InvoiceNo;
                            ws.Cell(row, 2).Value = item.Date?.ToString("MM-dd-yyyy");
                            ws.Cell(row, 3).Value = item.CompanyName;
                            ws.Cell(row, 4).Value = item.SupplierName;
                            if (item.InvoiceNo == "PayOut")
                            {
                                ws.Cell(row, 5).Value = item.TotalAmount;
                                ws.Cell(row, 6).Value = "";
                                yougavetotal += item.TotalAmount;
                            }
                            else
                            {
                                ws.Cell(row, 5).Value = "";
                                ws.Cell(row, 6).Value = item.TotalAmount;
                                yougettotal += item.TotalAmount;
                            }
                            row++;
                        }

                        ws.Cell(row, 1).Value = "Total";
                        ws.Cell(row, 2).Value = "";
                        ws.Cell(row, 3).Value = "";
                        ws.Cell(row, 4).Value = "";
                        ws.Cell(row, 5).Value = yougavetotal;
                        ws.Cell(row, 6).Value = yougettotal;


                        var headerRange = ws.Range(1, 1, 1, 6);
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.BackgroundColor = XLColor.Gray;
                        headerRange.Style.Font.FontColor = XLColor.White;
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        var totalRange = ws.Range(row, 1, row, 6);
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
    }
}
