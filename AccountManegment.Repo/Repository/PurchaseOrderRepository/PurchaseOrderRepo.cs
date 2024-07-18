using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.Repository.Interface.Repository.PurchaseOrder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Extensions.Configuration;
using AccountManagement.DBContext.Models.Common;

namespace AccountManagement.Repository.Repository.PurchaseOrderRepository
{
    public class PurchaseOrderRepo : IPurchaseOrder
    {
        public PurchaseOrderRepo(DbaccManegmentContext context, IConfiguration configuration)
        {
            Context = context;
            _configuration = configuration;
        }
        public DbaccManegmentContext Context { get; }
        public IConfiguration _configuration { get; }

        public async Task<ApiResponseModel> AddPurchaseOrderDetails(PurchaseOrderView PurchaseOrderDetails)
        {
            ApiResponseModel responseModel = new ApiResponseModel();
            try
            {
                var PurchaseOrder = new PurchaseOrder()
                {
                    Id = Guid.NewGuid(),
                    SiteId = PurchaseOrderDetails.SiteId,
                    FromSupplierId = PurchaseOrderDetails.FromSupplierId,
                    ToCompanyId = PurchaseOrderDetails.ToCompanyId,
                    TotalAmount = PurchaseOrderDetails.TotalAmount,
                    Description = PurchaseOrderDetails.Description,
                    DeliveryShedule = PurchaseOrderDetails.DeliveryShedule,
                    TotalDiscount = PurchaseOrderDetails.TotalDiscount,
                    TotalGstamount = PurchaseOrderDetails.TotalGstamount,
                    BillingAddress = PurchaseOrderDetails.BillingAddress,
                    CreatedBy = PurchaseOrderDetails.CreatedBy,
                    CreatedOn = DateTime.Now,
                };
                responseModel.code = (int)HttpStatusCode.OK;
                responseModel.message = "Purchase order successfully created.";
                Context.PurchaseOrders.Add(PurchaseOrder);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return responseModel;
        }

        public string CheckPONo(Guid? CompanyId)
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
                    var LastPO = Context.PurchaseOrders.Where(a => a.ToCompanyId == CompanyId).OrderByDescending(e => e.CreatedOn).FirstOrDefault();

                    var currentDate = DateTime.Now;
                    int currentYear = currentDate.Month > 4 ? currentDate.Year + 1 : currentDate.Year;
                    int lastYear = currentYear - 1;

                    string PurchaseOrderId;
                    string trimmedPOPef = CompanyDetails.InvoicePef.Trim();
                    if (LastPO == null)
                    {

                        PurchaseOrderId = $"{trimmedPOPef}/PO/{(lastYear % 100):D2}-{(currentYear % 100):D2}/001";
                    }
                    else
                    {
                        string lastPONumber = LastPO.Poid.Substring(LastPO.Poid.LastIndexOf('/') + 1);
                        Match match = Regex.Match(lastPONumber, @"\d+$");
                        if (match.Success)
                        {
                            int lastPONumberValue = int.Parse(match.Value);
                            int newPONumberValue = lastPONumberValue + 1;
                            PurchaseOrderId = $"{trimmedPOPef}/PO/{(lastYear % 100):D2}-{(currentYear % 100):D2}/{newPONumberValue:D3}";
                        }
                        else
                        {
                            throw new Exception("Purchase order id does not have the expected format.");
                        }
                    }
                    return PurchaseOrderId;
                }
            }
            catch (Exception ex)
            {
                string error;
                error = "Error generating Purchase Order number.";
                return error;
            }
        }

        public async Task<ApiResponseModel> DeletePurchaseOrderDetails(Guid POId)
        {
            ApiResponseModel response = new ApiResponseModel();

            var GetPOdata = Context.PurchaseOrders.Where(a => a.Id == POId).FirstOrDefault();
            var PODDataList = Context.PurchaseOrderDetails.Where(a => a.PorefId == POId).ToList();
            var POADataList = Context.PodeliveryAddresses.Where(a => a.Poid == POId).ToList();

            GetPOdata.IsDeleted = true;
            Context.PurchaseOrders.Update(GetPOdata);

            if (PODDataList.Any() || POADataList.Any())
            {
                foreach (var PODData in PODDataList)
                {
                    PODData.IsDeleted = true;
                    Context.PurchaseOrderDetails.Update(PODData);
                }

                foreach (var POAData in POADataList)
                {
                    POAData.IsDeleted = true;
                    Context.PodeliveryAddresses.Update(POAData);
                }

                Context.SaveChanges();

                response.code = 200;
                response.message = "Purchase order details are successfully deleted.";
            }
            else
            {
                response.code = 404;
                response.message = "No related records found to delete";
            }

            return response;
        }


        public async Task<PurchaseOrderMasterView> GetPurchaseOrderDetailsById(Guid POId)
        {
            PurchaseOrderMasterView PurchaseOrder = new PurchaseOrderMasterView();
            try
            {
                PurchaseOrder = (from a in Context.PurchaseOrders.Where(x => x.Id == POId)
                                 join b in Context.SupplierMasters on a.FromSupplierId equals b.SupplierId
                                 join c in Context.Companies on a.ToCompanyId equals c.CompanyId
                                 join d in Context.Sites on a.SiteId equals d.SiteId
                                 join g in Context.PodeliveryAddresses on a.Id equals g.Poid
                                 join e in Context.Cities on b.City equals e.CityId
                                 join f in Context.States on b.State equals f.StatesId
                                 select new PurchaseOrderMasterView
                                 {
                                     Id = a.Id,
                                     SiteId = a.SiteId,
                                     SiteName = d.SiteName,
                                     Poid = a.Poid,
                                     FromSupplierId = a.FromSupplierId,
                                     Area = b.Area,
                                     BuildingName = b.BuildingName,
                                     SupplierName = b.SupplierName,
                                     SupplierGstno = b.Gstno,
                                     SupplierMobile = b.Mobile,
                                     Cityname = e.CityName,
                                     Statename = f.StatesName,
                                     Pincode = b.PinCode,
                                     ToCompanyId = a.ToCompanyId,
                                     CompanyName = c.CompanyName,
                                     CompanyGstno = c.Gstno,
                                     TotalAmount = a.TotalAmount,
                                     Description = a.Description,
                                     DeliveryShedule = a.DeliveryShedule,
                                     TotalDiscount = a.TotalDiscount,
                                     TotalGstamount = a.TotalGstamount,
                                     BillingAddress = a.BillingAddress,
                                     ShippingAddress = g.Address,
                                     Date = a.Date,
                                     ContactName = a.ContactName,
                                     ContactNumber = a.ContactNumber,
                                     CreatedBy = a.CreatedBy,
                                     CreatedOn = a.CreatedOn,
                                     Terms = a.Terms,
                                     SupplierFullAddress = b.BuildingName + "-" + b.Area + "," + e.CityName + "," + f.StatesName
                                 }).First();

                List<POItemDetailsModel> itemlist = (from a in Context.PurchaseOrderDetails.Where(a => a.PorefId == PurchaseOrder.Id)
                                                     join b in Context.ItemMasters on a.ItemId equals b.ItemId
                                                     join c in Context.UnitMasters on a.UnitTypeId equals c.UnitId
                                                     join i in Context.ItemMasters on a.ItemId equals i.ItemId
                                                     select new POItemDetailsModel
                                                     {
                                                         ItemName = i.ItemName,
                                                         ItemId = a.ItemId,
                                                         Quantity = a.Quantity,
                                                         ItemAmount = a.ItemTotal,
                                                         Gstamount = a.Gst,
                                                         UnitType = a.UnitTypeId,
                                                         UnitTypeName = c.UnitName,
                                                         PricePerUnit = a.Price,
                                                         GstPercentage = b.Gstper,
                                                         Hsncode = b.Hsncode,
                                                         TotalAmount = a.ItemTotal,
                                                     }).ToList();

                List<PODeliveryAddressModel> addresslist = (from a in Context.PodeliveryAddresses.Where(a => a.Poid == PurchaseOrder.Id)
                                                            select new PODeliveryAddressModel
                                                            {
                                                                Aid = a.Aid,
                                                                Poid = a.Poid,
                                                                Quantity = a.Quantity,
                                                                UnitTypeId = a.UnitTypeId,
                                                                Address = a.Address,
                                                                IsDeleted = a.IsDeleted,
                                                            }).ToList();

                PurchaseOrder.ItemList = itemlist;
                PurchaseOrder.AddressList = addresslist;

                return PurchaseOrder;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<PurchaseOrderView>> GetPurchaseOrderList(string? searchText, string? searchBy, string? sortBy)
        {
            try
            {
                string dbConnectionStr = _configuration.GetConnectionString("ACCDbconn");
                var dataSet = DbHelper.GetDataSet("[GetPurchaseOrderList]", CommandType.StoredProcedure, new SqlParameter[] { }, dbConnectionStr);

                var POList = new List<PurchaseOrderView>();

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    var purchaseOrder = new PurchaseOrderView
                    {
                        Id = Guid.Parse(row["Id"].ToString()),
                        SiteId = Guid.Parse(row["SiteId"].ToString()),
                        SiteName = row["SiteName"]?.ToString(),
                        Poid = row["Poid"]?.ToString(),
                        FromSupplierId = Guid.Parse(row["FromSupplierId"].ToString()),
                        SupplierName = row["SupplierName"]?.ToString(),
                        ToCompanyId = Guid.Parse(row["ToCompanyId"].ToString()),
                        CompanyName = row["CompanyName"]?.ToString(),
                        TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                        Description = row["Description"]?.ToString(),
                        DeliveryShedule = row["DeliveryShedule"]?.ToString(),
                        TotalDiscount = Convert.ToInt32(row["TotalDiscount"]),
                        TotalGstamount = Convert.ToInt32(row["TotalGstamount"]),
                        BillingAddress = row["BillingAddress"]?.ToString(),
                        CreatedOn = row["CreatedOn"] != DBNull.Value ? (DateTime)row["CreatedOn"] : DateTime.MinValue,
                        CreatedBy = Guid.Parse(row["CreatedBy"].ToString()),
                        Terms = row["Terms"]?.ToString(),
                    };
                    POList.Add(purchaseOrder);
                }

                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    POList = POList.Where(u =>
                        u.SupplierName?.ToLower().Contains(searchText) == true ||
                        u.TotalGstamount.ToString().Contains(searchText) ||
                        u.TotalAmount.ToString().Contains(searchText) ||
                        u.CompanyName?.ToLower().Contains(searchText) == true ||
                        u.Poid?.ToLower().Contains(searchText) == true
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
                {
                    searchText = searchText.ToLower();
                    switch (searchBy.ToLower())
                    {
                        case "suppliername":
                            POList = POList.Where(u => u.SupplierName?.ToLower().Contains(searchText) == true).ToList();
                            break;
                        case "totalgstamount":
                            POList = POList.Where(u => u.TotalGstamount.ToString().Contains(searchText)).ToList();
                            break;
                        case "totalamount":
                            POList = POList.Where(u => u.TotalAmount.ToString().Contains(searchText)).ToList();
                            break;
                        default:
                            break;
                    }
                }

                if (string.IsNullOrEmpty(sortBy))
                {
                    POList = POList.OrderByDescending(u => u.CreatedOn).ToList();
                }
                else
                {
                    string sortOrder = sortBy.StartsWith("Ascending") ? "ascending" : "descending";
                    string field = sortBy.Substring(sortOrder.Length);

                    switch (field.ToLower())
                    {
                        case "suppliername":
                            if (sortOrder == "ascending")
                                POList = POList.OrderBy(u => u.SupplierName).ToList();
                            else if (sortOrder == "descending")
                                POList = POList.OrderByDescending(u => u.SupplierName).ToList();
                            break;
                        case "createdon":
                            if (sortOrder == "ascending")
                                POList = POList.OrderBy(u => u.CreatedOn).ToList();
                            else if (sortOrder == "descending")
                                POList = POList.OrderByDescending(u => u.CreatedOn).ToList();
                            break;
                        case "totalgstamount":
                            if (sortOrder == "ascending")
                                POList = POList.OrderBy(u => u.TotalGstamount).ToList();
                            else if (sortOrder == "descending")
                                POList = POList.OrderByDescending(u => u.TotalGstamount).ToList();
                            break;
                        case "totalamount":
                            if (sortOrder == "ascending")
                                POList = POList.OrderBy(u => u.TotalAmount).ToList();
                            else if (sortOrder == "descending")
                                POList = POList.OrderByDescending(u => u.TotalAmount).ToList();
                            break;
                        default:
                            break;
                    }
                }
                return POList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ApiResponseModel> InsertMultiplePurchaseOrderDetails(PurchaseOrderMasterView PurchaseOrderDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var PurchaseOrder = new PurchaseOrder()
                {
                    Id = Guid.NewGuid(),
                    Poid = PurchaseOrderDetails.Poid,
                    SiteId = PurchaseOrderDetails.SiteId,
                    Date = PurchaseOrderDetails.Date,
                    FromSupplierId = PurchaseOrderDetails.FromSupplierId,
                    ToCompanyId = PurchaseOrderDetails.ToCompanyId,
                    TotalAmount = PurchaseOrderDetails.TotalAmount,
                    Description = PurchaseOrderDetails.Description,
                    DeliveryShedule = PurchaseOrderDetails.DeliveryShedule,
                    TotalDiscount = PurchaseOrderDetails.TotalDiscount,
                    TotalGstamount = PurchaseOrderDetails.TotalGstamount,
                    BillingAddress = PurchaseOrderDetails.BillingAddress,
                    ContactName = PurchaseOrderDetails.ContactName,
                    ContactNumber = PurchaseOrderDetails.ContactNumber,
                    IsDeleted = false,
                    Terms = PurchaseOrderDetails.Terms,
                    CreatedBy = PurchaseOrderDetails.CreatedBy,
                    CreatedOn = DateTime.Now,
                };
                Context.PurchaseOrders.Add(PurchaseOrder);

                foreach (var item in PurchaseOrderDetails.ItemOrderlist)
                {
                    var PurchaseOrderDetail = new PurchaseOrderDetail()
                    {
                        PorefId = PurchaseOrder.Id,
                        ItemId = item.ItemId,
                        ItemName = item.ItemName,
                        ItemTotal = item.ItemTotal,
                        UnitTypeId = item.UnitTypeId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Discount = item.Discount,
                        Gst = item.Gst,
                        IsDeleted = false,
                        CreatedBy = PurchaseOrderDetails.CreatedBy,
                        CreatedOn = DateTime.Now,
                    };
                    Context.PurchaseOrderDetails.Add(PurchaseOrderDetail);
                }
                foreach (var item in PurchaseOrderDetails.ShippingAddressList)
                {
                    var PurchaseAddress = new PodeliveryAddress()
                    {
                        Poid = PurchaseOrder.Id,
                        Address = item.ShippingAddress,
                        IsDeleted = false,
                        UnitTypeId = PurchaseOrderDetails.UnitTypeId,
                        Quantity = item.ShippingQuantity,
                    };
                    Context.PodeliveryAddresses.Add(PurchaseAddress);
                }

                await Context.SaveChangesAsync();
                response.code = (int)HttpStatusCode.OK;
                response.message = "Purchase order successfully inserted.";
                response.data = PurchaseOrder.Id;
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.message = "Error creating orders: " + ex.Message;
            }
            return response;
        }

        public async Task<ApiResponseModel> UpdateMultiplePurchaseOrderDetails(PurchaseOrderMasterView PurchaseOrderDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var PurchaseOrder = await Context.PurchaseOrders.FindAsync(PurchaseOrderDetails.Id);

                if (PurchaseOrder == null)
                {
                    response.code = (int)HttpStatusCode.NotFound;
                    response.message = $"Purchase order with id {PurchaseOrderDetails.Id} not found";
                    return response;
                }

                PurchaseOrder.Id = PurchaseOrderDetails.Id;
                PurchaseOrder.Poid = PurchaseOrderDetails.Poid;
                PurchaseOrder.SiteId = PurchaseOrderDetails.SiteId;
                PurchaseOrder.Date = PurchaseOrderDetails.Date;
                PurchaseOrder.FromSupplierId = PurchaseOrderDetails.FromSupplierId;
                PurchaseOrder.ToCompanyId = PurchaseOrderDetails.ToCompanyId;
                PurchaseOrder.TotalAmount = PurchaseOrderDetails.TotalAmount;
                PurchaseOrder.Description = PurchaseOrderDetails.Description;
                PurchaseOrder.DeliveryShedule = PurchaseOrderDetails.DeliveryShedule;
                PurchaseOrder.TotalDiscount = PurchaseOrderDetails.TotalDiscount;
                PurchaseOrder.TotalGstamount = PurchaseOrderDetails.TotalGstamount;
                PurchaseOrder.BillingAddress = PurchaseOrderDetails.BillingAddress;
                PurchaseOrder.ContactName = PurchaseOrderDetails.ContactName;
                PurchaseOrder.ContactNumber = PurchaseOrderDetails.ContactNumber;
                PurchaseOrder.Terms = PurchaseOrderDetails.Terms;
                Context.PurchaseOrders.Update(PurchaseOrder);

                foreach (var address in PurchaseOrderDetails.ShippingAddressList)
                {
                    var existingDeliveryAddress = Context.PodeliveryAddresses.FirstOrDefault(e => e.Poid == PurchaseOrderDetails.Id && e.Address == address.ShippingAddress);

                    if (existingDeliveryAddress != null)
                    {
                        existingDeliveryAddress.Address = address.ShippingAddress;
                        existingDeliveryAddress.Quantity = address.ShippingQuantity;
                        existingDeliveryAddress.UnitTypeId = PurchaseOrderDetails.UnitTypeId;

                        Context.PodeliveryAddresses.Update(existingDeliveryAddress);
                    }
                    else
                    {
                        var newDeliveryAddress = new PodeliveryAddress()
                        {
                            Poid = PurchaseOrderDetails.Id,
                            Address = address.ShippingAddress,
                            Quantity = address.ShippingQuantity,
                            UnitTypeId = PurchaseOrderDetails.UnitTypeId,
                            IsDeleted = false,
                        };

                        Context.PodeliveryAddresses.Add(newDeliveryAddress);
                    }
                }

                var purchaseOrderShippingAddresses = PurchaseOrderDetails.ShippingAddressList.Select(a => a.ShippingAddress).ToList();

                var deliveryAddressesToRemove = Context.PodeliveryAddresses
                    .Where(e => e.Poid == PurchaseOrderDetails.Id && !purchaseOrderShippingAddresses.Contains(e.Address))
                    .ToList();

                Context.PodeliveryAddresses.RemoveRange(deliveryAddressesToRemove);

                await Context.SaveChangesAsync();
                response.code = (int)HttpStatusCode.OK;
                response.message = "Purchase order successfully updated.";
                response.data = PurchaseOrderDetails.Id;
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.message = "Error creating orders: " + ex.Message;
            }
            return response;
        }

        public async Task<ApiResponseModel> UpdatePurchaseOrderDetails(PurchaseOrderView PurchaseOrderDetails)
        {
            ApiResponseModel responseModel = new ApiResponseModel();
            var PurchaseOrder = Context.PurchaseOrders.Where(e => e.Id == PurchaseOrderDetails.Id).FirstOrDefault();
            try
            {
                if (PurchaseOrder != null)
                {
                    PurchaseOrder.Id = PurchaseOrderDetails.Id;
                    PurchaseOrder.SiteId = PurchaseOrderDetails.SiteId;
                    PurchaseOrder.FromSupplierId = PurchaseOrderDetails.FromSupplierId;
                    PurchaseOrder.ToCompanyId = PurchaseOrderDetails.ToCompanyId;
                    PurchaseOrder.TotalAmount = PurchaseOrderDetails.TotalAmount;
                    PurchaseOrder.Description = PurchaseOrderDetails.Description;
                    PurchaseOrder.DeliveryShedule = PurchaseOrderDetails.DeliveryShedule;
                    PurchaseOrder.TotalDiscount = PurchaseOrderDetails.TotalDiscount;
                    PurchaseOrder.TotalGstamount = PurchaseOrderDetails.TotalGstamount;
                    PurchaseOrder.BillingAddress = PurchaseOrderDetails.BillingAddress;
                    PurchaseOrder.CreatedBy = PurchaseOrderDetails.CreatedBy;
                    PurchaseOrder.CreatedOn = PurchaseOrderDetails.CreatedOn;
                };
                responseModel.code = (int)HttpStatusCode.OK;
                responseModel.message = "Purchase order successfully updated.";
                Context.PurchaseOrders.Update(PurchaseOrder);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return responseModel;
        }
    }
}
