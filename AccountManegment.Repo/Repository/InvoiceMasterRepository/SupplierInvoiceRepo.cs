using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.Common;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.Repository.Interface.Repository.InvoiceMaster;
using AccountManagement.Repository.Interface.Repository.PurchaseOrder;
using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
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
        public SupplierInvoiceRepo(DbaccManegmentContext context, IConfiguration configuration)
        {
            Context = context;
            Configuration = configuration;
        }

        public DbaccManegmentContext Context { get; }
        public IConfiguration Configuration { get; }

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
                if (InvoiceItemList != null)
                {
                    foreach (var item in InvoiceItemList)
                    {
                        Context.SupplierInvoiceDetails.Remove(item);
                    }
                }
                if (SupplierInvoice != null)
                {
                    Context.SupplierInvoices.Remove(SupplierInvoice);
                    response.message = "Supplierinvoice is removed successfully!";
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
            try
            {
                string dbConnectionStr = Configuration.GetConnectionString("ACCDbconn");
                var sqlPar = new SqlParameter[]
                {
                    new SqlParameter("@InvoiceId", Id),
                };
                var DS = DbHelper.GetDataSet("[spGetSupplierInvoiceDetailsById]", System.Data.CommandType.StoredProcedure, sqlPar, dbConnectionStr);

                SupplierInvoiceMasterView SupplierInvoiceDetails = new SupplierInvoiceMasterView();

                if (DS != null && DS.Tables.Count > 0)
                {
                    if (DS.Tables[0].Rows.Count > 0)
                    {
                        SupplierInvoiceDetails.Id = DS.Tables[0].Rows[0]["Id"] != DBNull.Value ? (Guid)DS.Tables[0].Rows[0]["Id"] : Guid.Empty;
                        SupplierInvoiceDetails.InvoiceNo = DS.Tables[0].Rows[0]["InvoiceNo"]?.ToString();
                        SupplierInvoiceDetails.SiteId = DS.Tables[0].Rows[0]["SiteId"] != DBNull.Value ? (Guid)DS.Tables[0].Rows[0]["SiteId"] : Guid.Empty;
                        SupplierInvoiceDetails.SiteName = DS.Tables[0].Rows[0]["SiteName"]?.ToString();
                        SupplierInvoiceDetails.SupplierId = DS.Tables[0].Rows[0]["SupplierId"] != DBNull.Value ? (Guid)DS.Tables[0].Rows[0]["SupplierId"] : Guid.Empty;
                        SupplierInvoiceDetails.SupplierName = DS.Tables[0].Rows[0]["SupplierName"]?.ToString();
                        SupplierInvoiceDetails.Description = DS.Tables[0].Rows[0]["Description"]?.ToString();
                        SupplierInvoiceDetails.SupplierArea = DS.Tables[0].Rows[0]["SupplierArea"]?.ToString();
                        SupplierInvoiceDetails.SupplierAccountNo = DS.Tables[0].Rows[0]["SupplierAccountNo"]?.ToString();
                        SupplierInvoiceDetails.SupplierBankName = DS.Tables[0].Rows[0]["SupplierBankName"]?.ToString();
                        SupplierInvoiceDetails.SupplierBuildingName = DS.Tables[0].Rows[0]["SupplierBuildingName"]?.ToString();
                        SupplierInvoiceDetails.SupplierCity = DS.Tables[0].Rows[0]["SupplierCity"]?.ToString();
                        SupplierInvoiceDetails.SupplierGstNo = DS.Tables[0].Rows[0]["SupplierGstNo"]?.ToString();
                        SupplierInvoiceDetails.SupplierIFSCCode = DS.Tables[0].Rows[0]["SupplierIFSCCode"]?.ToString();
                        SupplierInvoiceDetails.SupplierEmail = DS.Tables[0].Rows[0]["SupplierEmail"]?.ToString();
                        SupplierInvoiceDetails.SupplierState = DS.Tables[0].Rows[0]["SupplierState"]?.ToString();
                        SupplierInvoiceDetails.SupplierPincode = DS.Tables[0].Rows[0]["SupplierPincode"]?.ToString();
                        SupplierInvoiceDetails.CompanyId = DS.Tables[0].Rows[0]["CompanyId"] != DBNull.Value ? (Guid)DS.Tables[0].Rows[0]["CompanyId"] : Guid.Empty;
                        SupplierInvoiceDetails.CompanyName = DS.Tables[0].Rows[0]["CompanyName"]?.ToString();
                        SupplierInvoiceDetails.CompanyAddress = DS.Tables[0].Rows[0]["CompanyAddress"]?.ToString();
                        SupplierInvoiceDetails.SupplierInvoiceNo = DS.Tables[0].Rows[0]["SupplierInvoiceNo"]?.ToString();
                        SupplierInvoiceDetails.CompanyArea = DS.Tables[0].Rows[0]["CompanyArea"]?.ToString();
                        SupplierInvoiceDetails.CompanyCityName = DS.Tables[0].Rows[0]["CompanyCityName"]?.ToString();
                        SupplierInvoiceDetails.CompanyCountryName = DS.Tables[0].Rows[0]["CompanyCountryName"]?.ToString();
                        SupplierInvoiceDetails.CompanyStateName = DS.Tables[0].Rows[0]["CompanyStateName"]?.ToString();
                        SupplierInvoiceDetails.CompanyGstNo = DS.Tables[0].Rows[0]["CompanyGstNo"]?.ToString();
                        SupplierInvoiceDetails.CompanyPincode = DS.Tables[0].Rows[0]["CompanyPincode"]?.ToString();
                        SupplierInvoiceDetails.CompanyPanNo = DS.Tables[0].Rows[0]["CompanyPanNo"]?.ToString();
                        SupplierInvoiceDetails.ShippingAddress = DS.Tables[0].Rows[0]["ShippingAddress"]?.ToString();
                        SupplierInvoiceDetails.ShippingAddress = DS.Tables[0].Rows[0]["ShippingAddress"]?.ToString();
                        SupplierInvoiceDetails.SupplierMobileNo = DS.Tables[0].Rows[0]["SupplierMobileNo"]?.ToString();
                        SupplierInvoiceDetails.Date = DS.Tables[0].Rows[0]["Date"] != DBNull.Value ? (DateTime)DS.Tables[0].Rows[0]["Date"] : DateTime.MinValue;
                        SupplierInvoiceDetails.TotalAmountInvoice = DS.Tables[0].Rows[0]["TotalAmountInvoice"] != DBNull.Value ? (decimal)DS.Tables[0].Rows[0]["TotalAmountInvoice"] : 0m;
                        SupplierInvoiceDetails.TotalDiscount = DS.Tables[0].Rows[0]["TotalDiscount"] != DBNull.Value ? (decimal)DS.Tables[0].Rows[0]["TotalDiscount"] : 0m;
                        SupplierInvoiceDetails.TotalGstamount = DS.Tables[0].Rows[0]["TotalGSTAmount"] != DBNull.Value ? (decimal)DS.Tables[0].Rows[0]["TotalGSTAmount"] : 0m;
                        SupplierInvoiceDetails.PaymentStatus = DS.Tables[0].Rows[0]["PaymentStatus"]?.ToString();
                        SupplierInvoiceDetails.IsPayOut = DS.Tables[0].Rows[0]["IsPayOut"] != DBNull.Value && (bool)DS.Tables[0].Rows[0]["IsPayOut"];
                        SupplierInvoiceDetails.Roundoff = DS.Tables[0].Rows[0]["Roundoff"] != DBNull.Value ? (decimal)DS.Tables[0].Rows[0]["Roundoff"] : 0m;
                        SupplierInvoiceDetails.ChallanNo = DS.Tables[0].Rows[0]["ChallanNo"]?.ToString();
                        SupplierInvoiceDetails.Lrno = DS.Tables[0].Rows[0]["LRNo"]?.ToString();
                        SupplierInvoiceDetails.VehicleNo = DS.Tables[0].Rows[0]["VehicleNo"]?.ToString();
                        SupplierInvoiceDetails.DispatchBy = DS.Tables[0].Rows[0]["DispatchBy"]?.ToString();
                        SupplierInvoiceDetails.PaymentTerms = DS.Tables[0].Rows[0]["PaymentTerms"]?.ToString();
                        SupplierInvoiceDetails.ContactName = DS.Tables[0].Rows[0]["ContactName"]?.ToString();
                        SupplierInvoiceDetails.ContactNumber = DS.Tables[0].Rows[0]["ContactNumber"]?.ToString();
                        SupplierInvoiceDetails.SupplierFullAddress = DS.Tables[0].Rows[0]["SupplierFullAddress"]?.ToString();
                        SupplierInvoiceDetails.CompanyFullAddress = DS.Tables[0].Rows[0]["CompanyFullAddress"]?.ToString();
                        SupplierInvoiceDetails.CreatedOn = DS.Tables[0].Rows[0]["CreatedOn"] != DBNull.Value ? (DateTime)DS.Tables[0].Rows[0]["CreatedOn"] : DateTime.MinValue; ;
                        SupplierInvoiceDetails.StateCode = DS.Tables[0].Rows[0]["StateCode"] != DBNull.Value ? (int)DS.Tables[0].Rows[0]["StateCode"] : 0;
                    }
                    SupplierInvoiceDetails.ItemList = new List<POItemDetailsModel>();

                    foreach (DataRow row in DS.Tables[1].Rows)
                    {
                        var InvoiceDetails = new POItemDetailsModel
                        {
                            ItemId = row["ItemId"] != DBNull.Value ? (Guid)row["ItemId"] : Guid.Empty,
                            ItemName = row["ItemName"]?.ToString(),
                            Hsncode = row["Hsncode"]?.ToString(),
                            Quantity = row["Quantity"] != DBNull.Value ? (decimal)row["Quantity"] : 0,
                            UnitType = row["UnitType"] != DBNull.Value ? (int)row["UnitType"] : 0,
                            UnitTypeName = row["UnitTypeName"]?.ToString(),
                            PricePerUnit = row["PricePerUnit"] != DBNull.Value ? (decimal)row["PricePerUnit"] : 0m,
                            Gstamount = row["Gstamount"] != DBNull.Value ? (decimal)row["Gstamount"] : 0m,
                            GstPercentage = row["GstPercentage"] != DBNull.Value ? (decimal)row["GstPercentage"] : 0m,
                            TotalAmount = row["TotalAmount"] != DBNull.Value ? (decimal)row["TotalAmount"] : 0m,
                            DiscountPer = row["DiscountPer"] != DBNull.Value ? (decimal)row["DiscountPer"] : 0m,
                            DiscountAmount = row["DiscountAmount"] != DBNull.Value ? (decimal)row["DiscountAmount"] : 0m,
                        };
                        SupplierInvoiceDetails.ItemList.Add(InvoiceDetails);
                    }
                }
                return SupplierInvoiceDetails;
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
                string dbConnectionStr = Configuration.GetConnectionString("ACCDbconn");
                var dataSet = DbHelper.GetDataSet("[spGetSupplierInvoiceList]", System.Data.CommandType.StoredProcedure, new SqlParameter[] { }, dbConnectionStr);

                var SupplierInvoiceList = new List<SupplierInvoiceModel>();

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    var InvoiceDetails = new SupplierInvoiceModel
                    {
                        Id = row["Id"] == DBNull.Value ? Guid.Empty : Guid.Parse(row["Id"].ToString()),
                        InvoiceNo = row["InvoiceNo"]?.ToString() ?? string.Empty,
                        SiteId = row["SiteId"] == DBNull.Value ? Guid.Empty : Guid.Parse(row["SiteId"].ToString()),
                        SupplierId = row["SupplierId"] == DBNull.Value ? Guid.Empty : Guid.Parse(row["SupplierId"].ToString()),
                        CompanyId = row["CompanyId"] == DBNull.Value ? Guid.Empty : Guid.Parse(row["CompanyId"].ToString()),
                        TotalAmount = row["TotalAmount"] == DBNull.Value ? 0.0m : Convert.ToDecimal(row["TotalAmount"]),
                        TotalDiscount = row["TotalDiscount"] == DBNull.Value ? 0.0m : Convert.ToDecimal(row["TotalDiscount"]),
                        TotalGstamount = row["TotalGSTAmount"] == DBNull.Value ? 0.0m : Convert.ToDecimal(row["TotalGSTAmount"]),
                        Roundoff = row["Roundoff"] == DBNull.Value ? 0.0m : Convert.ToDecimal(row["Roundoff"]),
                        Description = row["Description"]?.ToString() ?? string.Empty,
                        CompanyName = row["CompanyName"]?.ToString() ?? string.Empty,
                        SiteName = row["SiteName"]?.ToString() ?? string.Empty,
                        SupplierName = row["SupplierName"]?.ToString() ?? string.Empty,
                        SupplierInvoiceNo = row["SupplierInvoiceNo"]?.ToString() ?? string.Empty,
                        Date = row["Date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["Date"]),
                        CreatedOn = row["CreatedOn"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["CreatedOn"])
                    };
                    SupplierInvoiceList.Add(InvoiceDetails);
                }

                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    SupplierInvoiceList = SupplierInvoiceList.Where(u =>
                        u.SiteName.ToLower().Contains(searchText) ||
                        u.CompanyName.ToLower().Contains(searchText) ||
                        u.SupplierName.ToLower().Contains(searchText) ||
                        u.TotalAmount.ToString().Contains(searchText)
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
                {
                    searchText = searchText.ToLower();
                    switch (searchBy.ToLower())
                    {
                        case "sitename":
                            SupplierInvoiceList = SupplierInvoiceList.Where(u => u.SiteName.ToLower().Contains(searchText)).ToList();
                            break;
                        case "companyname":
                            SupplierInvoiceList = SupplierInvoiceList.Where(u => u.CompanyName.ToLower().Contains(searchText)).ToList();
                            break;
                        default:

                            break;
                    }
                }

                if (string.IsNullOrEmpty(sortBy))
                {

                    SupplierInvoiceList = SupplierInvoiceList.OrderByDescending(u => u.CreatedOn).ToList();
                }
                else
                {
                    string sortOrder = sortBy.StartsWith("Ascending") ? "ascending" : "descending";
                    string field = sortBy.Substring(sortOrder.Length);

                    switch (field.ToLower())
                    {
                        case "companyname":
                            if (sortOrder == "ascending")
                                SupplierInvoiceList = SupplierInvoiceList.OrderBy(u => u.CompanyName).ToList();
                            else if (sortOrder == "descending")
                                SupplierInvoiceList = SupplierInvoiceList.OrderByDescending(u => u.CompanyName).ToList();
                            break;
                        case "invoiceno":
                            if (sortOrder == "ascending")
                                SupplierInvoiceList = SupplierInvoiceList.OrderBy(u => u.InvoiceNo).ToList();
                            else if (sortOrder == "descending")
                                SupplierInvoiceList = SupplierInvoiceList.OrderByDescending(u => u.InvoiceNo).ToList();
                            break;
                        case "createdon":
                            if (sortOrder == "ascending")
                                SupplierInvoiceList = SupplierInvoiceList.OrderBy(u => u.CreatedOn).ToList();
                            else if (sortOrder == "descending")
                                SupplierInvoiceList = SupplierInvoiceList.OrderByDescending(u => u.CreatedOn).ToList();
                            break;
                        default:
                            break;
                    }
                }
                return SupplierInvoiceList;
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
