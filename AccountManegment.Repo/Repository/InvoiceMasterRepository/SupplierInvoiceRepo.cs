using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.Repository.Interface.Repository.InvoiceMaster;
using AccountManagement.Repository.Interface.Repository.PurchaseOrder;
using Azure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#nullable disable
namespace AccountManagement.Repository.Repository.InvoiceMasterRepository
{
    public class SupplierInvoiceRepo : ISupplierInvoice
    {
        public SupplierInvoiceRepo(DbaccManegmentContext context)
        {
            Context = context;
        }

        public DbaccManegmentContext Context { get; }

        public async Task<ApiResponseModel> AddSupplierInvoice(SupplierInvoiceModel SupplierInvoiceDetail)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var SupplierInvoice = new SupplierInvoice()
                {
                    Id = Guid.NewGuid(),
                    InvoiceNo = SupplierInvoiceDetail.InvoiceNo,
                    SiteId = SupplierInvoiceDetail.SiteId,
                    SupplierId = SupplierInvoiceDetail.SupplierId,
                    CompanyId = SupplierInvoiceDetail.CompanyId,
                    TotalAmount = SupplierInvoiceDetail.TotalAmount,
                    Description = SupplierInvoiceDetail.Description,
                    Date = DateTime.Now,
                    TotalDiscount = SupplierInvoiceDetail.TotalDiscount,
                    TotalGstamount = SupplierInvoiceDetail.TotalGstamount,
                    Roundoff = SupplierInvoiceDetail.Roundoff,
                    IsPayOut = true,
                    PaymentStatus = SupplierInvoiceDetail.PaymentStatus,
                    CreatedBy = SupplierInvoiceDetail.CreatedBy,
                    CreatedOn = DateTime.Now,
                };
                response.code = (int)HttpStatusCode.OK;
                response.message = "Payment out successfully.";
                Context.SupplierInvoices.Add(SupplierInvoice);
                Context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

        public async Task<ApiResponseModel> DeleteSupplierInvoice(Guid Id)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var SupplierInvoice = Context.SupplierInvoices.Where(a => a.Id == Id).FirstOrDefault();
                var InvoiceItemList = Context.SupplierInvoiceDetails.Where(b => b.RefInvoiceId == Id).ToList();
                if (SupplierInvoice != null)
                {
                    Context.SupplierInvoices.Remove(SupplierInvoice);
                    response.message = "Supplierinvoice is removed successfully!";
                    response.code = 200;
                }
                if (InvoiceItemList != null)
                {
                    foreach (var item in InvoiceItemList)
                    {
                        Context.SupplierInvoiceDetails.Remove(item);
                    }
                }
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public async Task<InvoiceTotalAmount> GetInvoiceDetailsById(Guid CompanyId, Guid SupplierId)
        {
            try
            {


                var onlineCashSum = await Context.SupplierInvoices
                 .Where(si => si.SupplierId == SupplierId &&
                 si.CompanyId == CompanyId &&
                 (si.PaymentStatus == "Online" || si.PaymentStatus == "Cash") &&
                 si.InvoiceNo == "PayOut")
                 .SumAsync(si => si.TotalAmount);

                var unpaidSum = await Context.SupplierInvoices
                    .Where(si => si.SupplierId == SupplierId &&
                                 si.CompanyId == CompanyId &&
                                 si.PaymentStatus == "Unpaid")
                    .SumAsync(si => si.TotalAmount);

                var totalPurchase = await Context.SupplierInvoices
                    .Where(si => si.SupplierId == SupplierId &&
                                 si.CompanyId == CompanyId &&
                                 si.InvoiceNo != "PayOut")
                    .SumAsync(si => si.TotalAmount);

                var difference = unpaidSum - onlineCashSum;

                var supplierInvoices = await (from a in Context.SupplierInvoices
                                              where a.CompanyId == CompanyId
                                                  && a.SupplierId == SupplierId
                                                  && a.IsPayOut == false
                                                  && a.PaymentStatus == "Unpaid"
                                              join b in Context.SupplierMasters on a.SupplierId equals b.SupplierId
                                              join c in Context.Companies on a.CompanyId equals c.CompanyId
                                              select new SupplierInvoiceModel
                                              {
                                                  Id = a.Id,
                                                  InvoiceNo = a.InvoiceNo,
                                                  SupplierId = a.SupplierId,
                                                  SupplierName = b.SupplierName,
                                                  CompanyId = a.CompanyId,
                                                  CompanyName = c.CompanyName,
                                                  Date = DateTime.Now,
                                                  TotalAmount = a.TotalAmount,
                                                  TotalDiscount = a.TotalDiscount,
                                                  TotalGstamount = a.TotalGstamount,
                                                  Description = a.Description,
                                                  Roundoff = a.Roundoff,
                                                  IsPayOut = a.IsPayOut,
                                                  SupplierInvoiceNo = a.SupplierInvoiceNo,
                                                  PaymentStatus = a.PaymentStatus
                                              }).ToListAsync();

                var invoiceTotalAmount = new InvoiceTotalAmount
                {
                    InvoiceList = supplierInvoices.ToList(),
                    TotalPending = difference,
                    TotalCreadit = onlineCashSum,
                    TotalOutstanding = unpaidSum,
                    TotalPurchase = totalPurchase

                };

                return (invoiceTotalAmount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SupplierInvoiceMasterView> GetSupplierInvoiceById(Guid Id)
        {

            SupplierInvoiceMasterView supplierList = new SupplierInvoiceMasterView();
            try
            {
                supplierList = (from a in Context.SupplierInvoices.Where(x => x.Id == Id)
                                join b in Context.SupplierMasters on a.SupplierId equals b.SupplierId
                                join c in Context.Companies on a.CompanyId equals c.CompanyId
                                //join d in Context.Sites on a.SiteId equals d.SiteId
                                join e in Context.Cities on c.CityId equals e.CityId
                                join f in Context.States on c.StateId equals f.StatesId
                                join g in Context.Countries on c.Country equals g.CountryId
                                join supCity in Context.Cities on b.City equals supCity.CityId
                                join supState in Context.States on b.State equals supState.StatesId
                                select new SupplierInvoiceMasterView
                                {
                                    Id = a.Id,
                                    InvoiceNo = a.InvoiceNo,
                                    SiteId = a.SiteId,
                                    //SiteName = d.SiteName,
                                    SupplierId = a.SupplierId,
                                    SupplierName = b.SupplierName,
                                    Description = a.Description,
                                    SupplierArea = b.Area,
                                    SupplierAccountNo = b.AccountNo,
                                    SupplierBankName = b.BankName,
                                    SupplierBuildingName = b.BuildingName,
                                    SupplierCity = supCity.CityName,
                                    SupplierGstNo = b.Gstno,
                                    SupplierIFSCCode = b.Iffccode,
                                    SupplierEmail = b.Email,
                                    SupplierState = supState.StatesName,
                                    SupplierPincode = b.PinCode,
                                    CompanyId = a.CompanyId,
                                    CompanyName = c.CompanyName,
                                    CompanyAddress = c.Address,
                                    SupplierInvoiceNo = a.SupplierInvoiceNo,
                                    CompanyArea = c.Area,
                                    CompanyCityName = e.CityName,
                                    CompanyCountryName = g.CountryName,
                                    CompanyStateName = f.StatesName,
                                    CompanyGstNo = c.Gstno,
                                    CompanyPincode = c.Pincode,
                                    CompanyPanNo = c.PanNo,
                                    ShippingAddress = a.ShippingAddress,
                                    SupplierMobileNo = b.Mobile,
                                    Date = a.Date,
                                    TotalAmountInvoice = a.TotalAmount,
                                    TotalDiscount = a.TotalDiscount,
                                    TotalGstamount = a.TotalGstamount,
                                    PaymentStatus = a.PaymentStatus,
                                    IsPayOut = a.IsPayOut,
                                    Roundoff = a.Roundoff,
                                    ChallanNo = a.ChallanNo,
                                    Lrno = a.Lrno,
                                    VehicleNo = a.VehicleNo,
                                    DispatchBy=a.DispatchBy,
                                    PaymentTerms = a.PaymentTerms,
                                    ContactName = a.ContactName,
                                    ContactNumber = a.ContactNumber,
                                    CreatedOn = a.CreatedOn,
                                    CompanyFullAddress = c.Address + "-" + c.Area + "," + e.CityName + "," + f.StatesName + "-" + c.Pincode,
                                    SupplierFullAddress = b.BuildingName + "-" + b.Area + "," + supCity.CityName + "," + supState.StatesName + "-" + b.PinCode,
                                }).FirstOrDefault();
                List<POItemDetailsModel> itemlist = (from a in Context.SupplierInvoiceDetails.Where(a => a.RefInvoiceId == supplierList.Id)
                                                     join b in Context.UnitMasters on a.UnitTypeId equals b.UnitId
                                                     join i in Context.ItemMasters on a.ItemId equals i.ItemId
                                                     select new POItemDetailsModel
                                                     {
                                                         ItemId = a.ItemId,
                                                         ItemName = i.ItemName,
                                                         Quantity = a.Quantity,
                                                         Gstamount = a.Gst,
                                                         TotalAmount = a.TotalAmount,
                                                         UnitType = a.UnitTypeId,
                                                         UnitTypeName = b.UnitName,
                                                         PricePerUnit = a.Price,
                                                         GstPercentage = a.Gstper,
                                                         DiscountAmount = a.DiscountAmount,
                                                         DiscountPer = a.DiscountPer,
                                                     }).ToList();


                supplierList.ItemList = itemlist;
                return supplierList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<SupplierInvoiceModel>> GetSupplierInvoiceList(string? searchText, string? searchBy, string? sortBy)
        {
            try
            {
                var supplierList = (from a in Context.SupplierInvoices
                                    join b in Context.SupplierMasters on a.SupplierId equals b.SupplierId
                                    join c in Context.Companies on a.CompanyId equals c.CompanyId
                                    //join d in Context.Sites on a.SiteId equals d.SiteId
                                    where a.InvoiceNo != "PayOut"
                                    select new SupplierInvoiceModel
                                    {
                                        Id = a.Id,
                                        InvoiceNo = a.InvoiceNo,
                                        SiteId = a.SiteId,
                                        SupplierId = a.SupplierId,
                                        TotalAmount = a.TotalAmount,
                                        TotalDiscount = a.TotalDiscount,
                                        TotalGstamount = a.TotalGstamount,
                                        Description = a.Description,
                                        Roundoff = a.Roundoff,
                                        CompanyId = a.CompanyId,
                                        Date = DateTime.Now,
                                        CompanyName = c.CompanyName,
                                        //SiteName = d.SiteName,
                                        SupplierName = b.SupplierName,
                                        CreatedOn = a.CreatedOn,
                                        SupplierInvoiceNo = a.SupplierInvoiceNo,
                                    });

                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    supplierList = supplierList.Where(u =>
                        u.SiteName.ToLower().Contains(searchText) ||
                        u.CompanyName.ToLower().Contains(searchText) ||
                        u.SupplierName.ToLower().Contains(searchText) ||
                        u.TotalAmount.ToString().Contains(searchText)
                    );
                }

                if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
                {
                    searchText = searchText.ToLower();
                    switch (searchBy.ToLower())
                    {
                        case "sitename":
                            supplierList = supplierList.Where(u => u.SiteName.ToLower().Contains(searchText));
                            break;
                        case "companyname":
                            supplierList = supplierList.Where(u => u.CompanyName.ToLower().Contains(searchText));
                            break;
                        default:

                            break;
                    }
                }

                if (string.IsNullOrEmpty(sortBy))
                {

                    supplierList = supplierList.OrderByDescending(u => u.CreatedOn);
                }
                else
                {
                    string sortOrder = sortBy.StartsWith("Ascending") ? "ascending" : "descending";
                    string field = sortBy.Substring(sortOrder.Length);

                    switch (field.ToLower())
                    {
                        case "companyname":
                            if (sortOrder == "ascending")
                                supplierList = supplierList.OrderBy(u => u.CompanyName);
                            else if (sortOrder == "descending")
                                supplierList = supplierList.OrderByDescending(u => u.CompanyName);
                            break;
                        case "invoiceno":
                            if (sortOrder == "ascending")
                                supplierList = supplierList.OrderBy(u => u.InvoiceNo);
                            else if (sortOrder == "descending")
                                supplierList = supplierList.OrderByDescending(u => u.InvoiceNo);
                            break;
                        case "createdon":
                            if (sortOrder == "ascending")
                                supplierList = supplierList.OrderBy(u => u.CreatedOn);
                            else if (sortOrder == "descending")
                                supplierList = supplierList.OrderByDescending(u => u.CreatedOn);
                            break;
                        default:

                            break;
                    }
                }

                return supplierList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ApiResponseModel> UpdateSupplierInvoice(SupplierInvoiceMasterView SupplierInvoiceDetail)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {

                var supplierInvoice = new SupplierInvoice()
                {
                    Id = SupplierInvoiceDetail.Id,
                    InvoiceNo = SupplierInvoiceDetail.InvoiceNo,
                    SiteId = SupplierInvoiceDetail.SiteId,
                    SupplierId = SupplierInvoiceDetail.SupplierId,
                    CompanyId = SupplierInvoiceDetail.CompanyId,
                    SupplierInvoiceNo = SupplierInvoiceDetail.SupplierInvoiceNo,
                    Description = SupplierInvoiceDetail.Description,
                    TotalDiscount = SupplierInvoiceDetail.TotalDiscount,
                    TotalGstamount = SupplierInvoiceDetail.TotalGstamount,
                    TotalAmount = SupplierInvoiceDetail.TotalAmountInvoice,
                    PaymentStatus = SupplierInvoiceDetail.PaymentStatus,
                    Roundoff = SupplierInvoiceDetail.Roundoff,
                    ShippingAddress = SupplierInvoiceDetail.ShippingAddress,
                    ChallanNo = SupplierInvoiceDetail.ChallanNo,
                    Lrno = SupplierInvoiceDetail.Lrno,
                    VehicleNo = SupplierInvoiceDetail.VehicleNo,
                    DispatchBy = SupplierInvoiceDetail.DispatchBy,
                    PaymentTerms = SupplierInvoiceDetail.PaymentTerms,
                    IsPayOut = SupplierInvoiceDetail.IsPayOut,
                    Date = SupplierInvoiceDetail.Date,
                    UpdatedOn = DateTime.Now,
                    UpdatedBy = SupplierInvoiceDetail.UpdatedBy,
                    CreatedOn = SupplierInvoiceDetail.CreatedOn,
                };
                Context.SupplierInvoices.Update(supplierInvoice);

                foreach (var item in SupplierInvoiceDetail.ItemList)
                {
                    var existingSupplierInvoice = Context.SupplierInvoiceDetails.FirstOrDefault(e => e.RefInvoiceId == supplierInvoice.Id && e.ItemId == item.ItemId);

                    if (existingSupplierInvoice != null)
                    {
                        existingSupplierInvoice.RefInvoiceId = supplierInvoice.Id;
                        existingSupplierInvoice.ItemId = item.ItemId;
                        existingSupplierInvoice.ItemName = item.ItemName;
                        existingSupplierInvoice.UnitTypeId = item.UnitType;
                        existingSupplierInvoice.Quantity = item.Quantity;
                        existingSupplierInvoice.Price = item.PricePerUnit;
                        existingSupplierInvoice.DiscountAmount = item.DiscountAmount;
                        existingSupplierInvoice.DiscountPer = item.DiscountPer;
                        existingSupplierInvoice.Gst = item.Gstamount;
                        existingSupplierInvoice.Gstper = item.GstPercentage;
                        existingSupplierInvoice.TotalAmount = item.TotalAmount;
                        existingSupplierInvoice.Date = supplierInvoice.Date;
                        existingSupplierInvoice.UpdatedOn = DateTime.Now;
                        existingSupplierInvoice.UpdatedBy = supplierInvoice.UpdatedBy;
                        existingSupplierInvoice.CreatedOn = supplierInvoice.CreatedOn;

                        Context.SupplierInvoiceDetails.Update(existingSupplierInvoice);
                    }
                    else
                    {
                        var newSupplierInvoice = new SupplierInvoiceDetail()
                        {
                            RefInvoiceId = supplierInvoice.Id,
                            ItemId = item.ItemId,
                            ItemName = item.ItemName,
                            UnitTypeId = item.UnitType,
                            Quantity = item.Quantity,
                            Price = item.PricePerUnit,
                            DiscountAmount = item.DiscountAmount,
                            DiscountPer = item.DiscountPer,
                            Gst = item.Gstamount,
                            Gstper = item.GstPercentage,
                            TotalAmount = item.TotalAmount,
                            Date = supplierInvoice.Date,
                            CreatedOn = DateTime.Now,
                        };

                        Context.SupplierInvoiceDetails.Add(newSupplierInvoice);
                    }
                }

                var deletedSupplierInvoice = SupplierInvoiceDetail.ItemList.Select(a => a.ItemId).ToList();

                var SupplierInvoiceToRemove = Context.SupplierInvoiceDetails
                    .Where(e => e.RefInvoiceId == SupplierInvoiceDetail.Id && !deletedSupplierInvoice.Contains(e.ItemId))
                    .ToList();

                Context.SupplierInvoiceDetails.RemoveRange(SupplierInvoiceToRemove);
                await Context.SaveChangesAsync();
                response.code = (int)HttpStatusCode.OK;
                response.message = "Invoice Update successfully.";
                response.data = supplierInvoice.Id;
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.message = "Error creating invoice: " + ex.Message;
            }
            return response;
        }

        public async Task<ApiResponseModel> InsertMultipleSupplierItemDetails(SupplierInvoiceMasterView SupplierItemDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                bool PayOut;
                if (SupplierItemDetails.PaymentStatus == "Unpaid")
                {
                    PayOut = false;
                }
                else
                {
                    PayOut = true;
                }

                var supplierInvoice = new SupplierInvoice()
                {
                    Id = Guid.NewGuid(),
                    InvoiceNo = SupplierItemDetails.InvoiceNo,
                    SiteId = SupplierItemDetails.SiteId,
                    SupplierId = SupplierItemDetails.SupplierId,
                    CompanyId = SupplierItemDetails.CompanyId,
                    SupplierInvoiceNo = SupplierItemDetails.SupplierInvoiceNo,
                    Description = SupplierItemDetails.Description,
                    TotalDiscount = SupplierItemDetails.TotalDiscount,
                    TotalGstamount = SupplierItemDetails.TotalGstamount,
                    TotalAmount = SupplierItemDetails.TotalAmountInvoice,
                    PaymentStatus = SupplierItemDetails.PaymentStatus,
                    Roundoff = SupplierItemDetails.Roundoff,
                    ChallanNo = SupplierItemDetails.ChallanNo,
                    Lrno = SupplierItemDetails.Lrno,
                    VehicleNo = SupplierItemDetails.VehicleNo,
                    DispatchBy = SupplierItemDetails.DispatchBy,
                    PaymentTerms = SupplierItemDetails.PaymentTerms,
                    ShippingAddress = SupplierItemDetails.ShippingAddress,
                    IsPayOut = PayOut,
                    Date = SupplierItemDetails.Date,
                    CreatedBy = SupplierItemDetails.CreatedBy,
                    CreatedOn = DateTime.Now,
                };
                Context.SupplierInvoices.Add(supplierInvoice);

                foreach (var item in SupplierItemDetails.ItemList)
                {
                    var supplierInvoiceDetail = new SupplierInvoiceDetail()
                    {
                        RefInvoiceId = supplierInvoice.Id,
                        ItemId = item.ItemId,
                        ItemName = item.ItemName,
                        UnitTypeId = item.UnitType,
                        Quantity = item.Quantity,
                        Price = item.PricePerUnit,
                        DiscountAmount = item.DiscountAmount,
                        DiscountPer = item.DiscountPer,
                        Gst = item.Gstamount,
                        Gstper = item.GstPercentage,
                        TotalAmount = item.TotalAmount,
                        CreatedBy = SupplierItemDetails.CreatedBy,
                        CreatedOn = DateTime.Now,
                        Date = supplierInvoice.Date,
                    };
                    Context.SupplierInvoiceDetails.Add(supplierInvoiceDetail);
                }

                await Context.SaveChangesAsync();
                response.code = (int)HttpStatusCode.OK;
                response.message = "Invoice inserted successfully.";
                response.data = supplierInvoice.Id;
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.message = "Error creating invoice: " + ex.Message;
            }
            return response;
        }

        public string CheckSuppliersInvoiceNo(Guid? CompanyId)
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
                    var lastInvoice = Context.SupplierInvoices
                        .Where(a => a.InvoiceNo != "PayOut" && a.CompanyId == CompanyId)
                        .OrderByDescending(e => e.CreatedOn)
                        .FirstOrDefault();

                    var currentDate = DateTime.Now;
                    int currentYear = currentDate.Month > 4 ? currentDate.Year + 1 : currentDate.Year;
                    int lastYear = currentYear - 1;

                    string supplierInvoiceId;
                    string trimmedInvoicePef = CompanyDetails.InvoicePef.Trim();
                    if (lastInvoice == null)
                    {
                        supplierInvoiceId = $"{trimmedInvoicePef}/Invoice/{(lastYear % 100):D2}-{(currentYear % 100):D2}/001";
                    }
                    else
                    {
                        string lastInvoiceNumber = lastInvoice.InvoiceNo.Substring(lastInvoice.InvoiceNo.LastIndexOf('/') + 1);
                        Match match = Regex.Match(lastInvoiceNumber, @"\d+$");
                        if (match.Success)
                        {
                            int lastInvoiceNumberValue = int.Parse(match.Value);
                            int newInvoiceNumberValue = lastInvoiceNumberValue + 1;
                            supplierInvoiceId = $"{trimmedInvoicePef}/Invoice/{(lastYear % 100):D2}-{(currentYear % 100):D2}/{newInvoiceNumberValue:D3}";
                        }
                        else
                        {
                            throw new Exception("Supplier invoice id does not have the expected format.");
                        }
                    }
                    return supplierInvoiceId;
                }
            }
            catch (Exception ex)
            {
                string error;
                error = "Error generating supplier invoice number.";
                return error;
            }
        }
        public async Task<IEnumerable<SupplierInvoiceModel>> GetSupplierInvoiceDetailsById(Guid SupplierId)
        {
            try
            {
                var supplierInvoices = (from a in Context.SupplierInvoices
                                        join b in Context.SupplierMasters on a.SupplierId equals b.SupplierId
                                        join c in Context.Companies on a.CompanyId equals c.CompanyId
                                        where a.SupplierId == SupplierId
                                        select new SupplierInvoiceModel
                                        {
                                            Id = a.Id,
                                            InvoiceNo = a.InvoiceNo,
                                            SupplierId = a.SupplierId,
                                            SupplierName = b.SupplierName,
                                            CompanyId = a.CompanyId,
                                            CompanyName = c.CompanyName,
                                            Date = DateTime.Now,
                                            TotalAmount = a.TotalAmount,
                                            TotalDiscount = a.TotalDiscount,
                                            TotalGstamount = a.TotalGstamount,
                                            Description = a.Description,
                                            Roundoff = a.Roundoff,
                                            IsPayOut = a.IsPayOut,
                                            SupplierInvoiceNo = a.SupplierInvoiceNo,
                                            PaymentStatus = a.PaymentStatus
                                        });
                return (supplierInvoices);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
