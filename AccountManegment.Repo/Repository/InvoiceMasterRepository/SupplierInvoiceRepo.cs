using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.Repository.Interface.Repository.InvoiceMaster;
using AccountManagement.Repository.Interface.Repository.PurchaseOrder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Text;
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
                response.message = "Supplier Invoice Successfully Inserted";
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
                if (SupplierInvoice != null)
                {
                    Context.SupplierInvoices.Remove(SupplierInvoice);
                    response.message = "SupplierInvoice" + " " + SupplierInvoice.Id + "is Removed Successfully!";
                    response.code = 200;
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

        public async Task<SupplierInvoiceMasterView> GetSupplierInvoiceById(Guid InvoiceId)
        {

            SupplierInvoiceMasterView supplierList = new SupplierInvoiceMasterView();
            try
            {
                supplierList = (from a in Context.SupplierInvoices.Where(x => x.Id == InvoiceId)
                                join b in Context.SupplierMasters on a.SupplierId equals b.SupplierId
                                join c in Context.Companies on a.CompanyId equals c.CompanyId
                                join d in Context.Sites on a.SiteId equals d.SiteId
                                join e in Context.Cities on c.CityId equals e.CityId
                                join f in Context.States on c.StateId equals f.StatesId
                                join g in Context.Countries on c.Country equals g.CountryId
                                select new SupplierInvoiceMasterView
                                {
                                    Id = a.Id,
                                    InvoiceId = a.InvoiceNo,
                                    SiteId = a.SiteId,
                                    SiteName = d.SiteName,
                                    SupplierId = a.SupplierId,
                                    SupplierName = b.SupplierName,
                                    CompanyId = a.CompanyId,
                                    CompanyName = c.CompanyName,
                                    Date = a.Date,
                                    Description = a.Description,
                                    TotalAmount = a.TotalAmount,
                                    TotalDiscount = a.TotalDiscount,
                                    TotalGstamount = a.TotalGstamount,
                                    PaymentStatus = a.PaymentStatus,
                                    IsPayOut = a.IsPayOut,
                                    Roundoff = a.Roundoff,
                                    CompanyAddress = c.Address,
                                    CompanyArea = c.Area,
                                    CompanyCityName = e.CityName,
                                    CompanyCountryName = g.CountryName,
                                    CompanyStateName = f.StatesName,
                                    CompanyGstNo = c.Gstno,
                                    CompanyPincode = c.Pincode,
                                }).First();
                List<POItemDetailsModel> itemlist = (from a in Context.SupplierInvoiceDetails.Where(a => a.RefInvoiceId == supplierList.Id)
                                                     join b in Context.UnitMasters on a.UnitTypeId equals b.UnitId
                                                     select new POItemDetailsModel
                                                     {
                                                         ItemName = a.Item,
                                                         Quantity = a.Quantity,
                                                         Gstamount = a.Gst,
                                                         UnitType = a.UnitTypeId,
                                                         UnitTypeName = b.UnitName,
                                                         PricePerUnit = a.Price,
                                                         GstPercentage = a.Gstper,
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
                                    join d in Context.Sites on a.SiteId equals d.SiteId
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
                                        SiteName = d.SiteName,
                                        SupplierName = b.SupplierName,
                                        CreatedOn = a.CreatedOn,
                                    });

                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    supplierList = supplierList.Where(u =>
                        u.SiteName.ToLower().Contains(searchText) ||
                        u.CompanyName.ToLower().Contains(searchText)
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
                        case "sitename":
                            if (sortOrder == "ascending")
                                supplierList = supplierList.OrderBy(u => u.SiteName);
                            else if (sortOrder == "descending")
                                supplierList = supplierList.OrderByDescending(u => u.SiteName);
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

        public async Task<ApiResponseModel> UpdateSupplierInvoice(SupplierInvoiceModel SupplierInvoiceDetail)
        {
            ApiResponseModel model = new ApiResponseModel();
            var supplierInvoice = Context.SupplierInvoices.Where(e => e.Id == SupplierInvoiceDetail.Id).FirstOrDefault();
            try
            {
                if (supplierInvoice != null)
                {
                    supplierInvoice.Id = SupplierInvoiceDetail.Id;
                    supplierInvoice.SiteId = SupplierInvoiceDetail.SiteId;
                    supplierInvoice.SupplierId = SupplierInvoiceDetail.SupplierId;
                    supplierInvoice.CompanyId = SupplierInvoiceDetail.CompanyId;
                    supplierInvoice.TotalAmount = SupplierInvoiceDetail.TotalAmount;
                    supplierInvoice.Description = SupplierInvoiceDetail.Description;
                    supplierInvoice.TotalDiscount = SupplierInvoiceDetail.TotalDiscount;
                    supplierInvoice.TotalGstamount = SupplierInvoiceDetail.TotalGstamount;
                    supplierInvoice.Roundoff = SupplierInvoiceDetail.Roundoff;
                }
                Context.SupplierInvoices.Update(supplierInvoice);
                Context.SaveChanges();
                model.code = 200;
                model.message = "Supplier Invoice Updated Successfully!";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return model;
        }

        public async Task<ApiResponseModel> InsertMultipleSupplierItemDetails(List<SupplierInvoiceMasterView> SupplierItemDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var firstOrderDetail = SupplierItemDetails.First();
                bool PayOut;
                if (firstOrderDetail.PaymentStatus == "Unpaid")
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
                    InvoiceNo = firstOrderDetail.InvoiceId,
                    SiteId = firstOrderDetail.SiteId,
                    SupplierId = firstOrderDetail.SupplierId,
                    CompanyId = firstOrderDetail.CompanyId,
                    Description = firstOrderDetail.Description,
                    TotalDiscount = firstOrderDetail.TotalDiscount,
                    TotalGstamount = firstOrderDetail.TotalGstamount,
                    TotalAmount = firstOrderDetail.TotalAmount,
                    PaymentStatus = firstOrderDetail.PaymentStatus,
                    Roundoff = firstOrderDetail.Roundoff,
                    IsPayOut = PayOut,
                    Date = DateTime.Now,
                    CreatedBy = firstOrderDetail.CreatedBy,
                    CreatedOn = DateTime.Now,
                };
                Context.SupplierInvoices.Add(supplierInvoice);

                foreach (var item in SupplierItemDetails)
                {
                    var supplierInvoiceDetail = new SupplierInvoiceDetail()
                    {
                        RefInvoiceId = supplierInvoice.Id,
                        Item = item.Item,
                        UnitTypeId = item.UnitTypeId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        DiscountPer = item.DiscountPer,
                        Gst = item.Gst,
                        Gstper = item.Gstper,
                        TotalAmount = item.TotalAmount,
                        CreatedBy = item.CreatedBy,
                        CreatedOn = DateTime.Now,
                    };
                    Context.SupplierInvoiceDetails.Add(supplierInvoiceDetail);
                }

                await Context.SaveChangesAsync();
                response.code = (int)HttpStatusCode.OK;
                response.message = "Supplier Order Inserted Successfully";
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.message = "Error creating orders: " + ex.Message;
            }
            return response;
        }

        public string CheckSupplierInvoiceNo()
        {
            try
            {
                var lastInvoice = Context.SupplierInvoices
                                         .OrderByDescending(e => e.CreatedOn).Where(a=>a.InvoiceNo!="PayOut")
                                         .FirstOrDefault();
                var currentDate = DateTime.Now;
                int currentYear = currentDate.Month > 4 ? currentDate.Year + 1 : currentDate.Year;
                int lastYear = currentYear - 1;

                string supplierInvoiceId;
                if (lastInvoice == null )
                {
                    supplierInvoiceId = $"DMInfra/Invoice/{(lastYear % 100):D2}-{(currentYear % 100):D2}/001";
                }
                else
                {
                    string lastInvoiceNumber = lastInvoice.InvoiceNo.Substring(24);
                    if (int.TryParse(lastInvoiceNumber, out int lastInvoiceNumberValue))
                    {
                        int newInvoiceNumberValue = lastInvoiceNumberValue + 1;
                        supplierInvoiceId = $"DMInfra/Invoice/{(lastYear % 100):D2}-{(currentYear % 100):D2}/" + newInvoiceNumberValue.ToString("D3");
                    }
                    else
                    {
                        throw new Exception("Supplier Invoice Id does not have the expected format.");
                    }
                }
                return supplierInvoiceId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating supplier invoice number.", ex);
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
