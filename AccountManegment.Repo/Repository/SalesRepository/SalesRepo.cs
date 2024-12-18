using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SalesMaster;
using AccountManagement.Repository.Interface.Repository.InvoiceMaster;
using AccountManagement.Repository.Interface.Repository.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Repository.SalesRepository
{
    public class SalesRepo: ISalesInvoice
    {
        public SalesRepo(DbaccManegmentContext context)
        {
            Context = context;
        }
        public DbaccManegmentContext Context { get; }

        public string CheckSalesInvoiceNo(Guid? CompanyId)
        {
            try
            {
                var CompanyDetails = Context.Companies.FirstOrDefault(e => e.CompanyId == CompanyId);
                if (CompanyDetails == null)
                {
                    throw new Exception("Company details not found.");
                }
                else
                {
                    var lastInvoice = Context.SalesInvoices
                        .Where(a => a.CompanyId == CompanyId)
                        .OrderByDescending(e => e.CreatedOn)
                        .FirstOrDefault();

                    var currentDate = DateTime.Now;
                    int currentYear = currentDate.Month > 4 ? currentDate.Year + 1 : currentDate.Year;
                    int lastYear = currentYear - 1;

                    string SalesInvoiceId;
                    string trimmedInvoicePef = CompanyDetails.InvoicePef.Trim();
                    if (lastInvoice == null)
                    {
                        SalesInvoiceId = $"{trimmedInvoicePef}/{(lastYear % 100):D2}-{(currentYear % 100):D2}/001";
                    }
                    else
                    {
                        string lastInvoiceNumber = lastInvoice.SalesInvoiceNo.Substring(lastInvoice.SalesInvoiceNo.LastIndexOf('/') + 1);
                        Match match = Regex.Match(lastInvoiceNumber, @"\d+$");
                        if (match.Success)
                        {
                            int lastInvoiceNumberValue = int.Parse(match.Value);
                            int newInvoiceNumberValue = lastInvoiceNumberValue + 1;
                            SalesInvoiceId = $"{trimmedInvoicePef}/{(lastYear % 100):D2}-{(currentYear % 100):D2}/{newInvoiceNumberValue:D3}";
                        }
                        else
                        {
                            throw new Exception("Sales invoice id does not have the expected format.");
                        }
                    }
                    return SalesInvoiceId;
                }
            }
            catch (Exception ex)
            {
                string error;
                error = "Error generating sales invoice number.";
                return error;
            }
        }

        public async Task<ApiResponseModel> InsertSalesInvoiceDetails(SalesInvoiceMasterModel SalesInvoiceDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                bool PayOut;
                if (SalesInvoiceDetails.PaymentStatus == "Unpaid")
                {
                    PayOut = false;
                }
                else
                {
                    PayOut = true;
                }

                var salesInvoice = new SalesInvoice()
                {
                    Id = Guid.NewGuid(),
                    SalesInvoiceNo = SalesInvoiceDetails.SalesInvoiceNo,
                    InvoiceType = SalesInvoiceDetails.InvoiceType,
                    SiteId = SalesInvoiceDetails.SiteId,
                    SupplierId = SalesInvoiceDetails.SupplierId,
                    CompanyId = SalesInvoiceDetails.CompanyId,
                    SupplierInvoiceNo = SalesInvoiceDetails.SupplierInvoiceNo,
                    Date = SalesInvoiceDetails.Date,
                    ChallanNo = SalesInvoiceDetails.ChallanNo,
                    Lrno = SalesInvoiceDetails.Lrno,
                    VehicleNo = SalesInvoiceDetails.VehicleNo,
                    DispatchBy = SalesInvoiceDetails.DispatchBy,
                    PaymentTerms = SalesInvoiceDetails.PaymentTerms,
                    Description = SalesInvoiceDetails.Description,
                    TotalAmount = SalesInvoiceDetails.TotalAmount,
                    TotalGstamount = SalesInvoiceDetails.TotalGstamount,
                    TotalDiscount = SalesInvoiceDetails.TotalDiscount,
                    DiscountRoundoff = SalesInvoiceDetails.DiscountRoundoff,
                    Tds = SalesInvoiceDetails.Tds,
                    PaymentStatus = SalesInvoiceDetails.PaymentStatus,
                    IsPayOut = PayOut,
                    ContactName = SalesInvoiceDetails.ContactName,
                    ContactNumber = SalesInvoiceDetails.ContactNumber,
                    ShippingAddress = SalesInvoiceDetails.ShippingAddress,
                    IsApproved = true,
                    CreatedBy = SalesInvoiceDetails.CreatedBy,
                    CreatedOn = DateTime.Now,
                };
                Context.SalesInvoices.Add(salesInvoice);

                foreach (var item in SalesInvoiceDetails.SalesInvoiceDetails)
                {
                    var salesInvoiceDetail = new SalesInvoiceDetail()
                    {
                        RefSalesInvoiceId = salesInvoice.Id,
                        ItemId = item.ItemId,
                        ItemName = item.ItemName,
                        ItemDescription = item.ItemDescription,
                        UnitTypeId = item.UnitTypeId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        DiscountPer = item.DiscountPer,
                        DiscountAmount = item.DiscountAmount,
                        Gstper = item.Gstper,
                        Gst = item.Gst,
                        TotalAmount = item.TotalAmount,
                        Date = salesInvoice.Date,
                        CreatedBy = salesInvoice.CreatedBy,
                        CreatedOn = DateTime.Now,
                        
                    };
                    Context.SalesInvoiceDetails.Add(salesInvoiceDetail);
                }

                await Context.SaveChangesAsync();
                response.code = (int)HttpStatusCode.OK;
                response.message = "SalesInvoice inserted successfully.";
                response.data = salesInvoice.Id;
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.message = "Error creating invoice: " + ex.Message;
            }
            return response;
        }
    }
}
