﻿using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.Repository.Interface.Repository.PurchaseOrder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Repository.PurchaseOrderRepository
{
    public class PurchaseOrderRepo : IPurchaseOrder
    {
        public PurchaseOrderRepo(DbaccManegmentContext context)
        {
            Context = context;
        }
        public DbaccManegmentContext Context { get; }

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
                responseModel.message = "Purchase Order Inserted Successfully";
                Context.PurchaseOrders.Add(PurchaseOrder);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return responseModel;
        }

        public string CheckPONo()
        {
            try
            {
                var LastPO = Context.PurchaseOrders.OrderByDescending(e => e.CreatedOn).FirstOrDefault();
                var currentDate = DateTime.Now;

                int currentYear;
                int lastYear;
                if (currentDate.Month > 4)
                {

                    currentYear = currentDate.Year + 1;
                    lastYear = currentDate.Year;
                }
                else
                {

                    currentYear = currentDate.Year;
                    lastYear = currentDate.Year - 1;
                }

                string PurchaseOrderId;
                if (LastPO == null)
                {

                    PurchaseOrderId = $"DMInfra/PO/{(lastYear % 100).ToString("D2")}-{(currentYear % 100).ToString("D2")}/001";
                }
                else
                {
                    if (LastPO.Poid.Length >= 19)
                    {

                        int PrNumber = int.Parse(LastPO.Poid.Substring(18)) + 1;
                        PurchaseOrderId = $"DMInfra/PO/{(lastYear % 100).ToString("D2")}-{(currentYear % 100).ToString("D2")}/" + PrNumber.ToString("D3");
                    }
                    else
                    {
                        throw new Exception("Purchase Order Id does not have the expected format.");
                    }
                }
                return PurchaseOrderId;
            }
            catch (Exception ex)
            {
                throw ex;
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
                response.message = "Purchase Order Details are deleted successfully";
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
                                     SupplierFullAddress = b.BuildingName + "-" + b.Area + "," + e.CityName + "," + f.StatesName + "-" + b.PinCode
                                 }).First();

                List<POItemDetailsModel> itemlist = (from a in Context.PurchaseOrderDetails.Where(a => a.PorefId == PurchaseOrder.Id)
                                                     join b in Context.ItemMasters on a.ItemId equals b.ItemId
                                                     join c in Context.UnitMasters on a.UnitTypeId equals c.UnitId
                                                     select new POItemDetailsModel
                                                     {
                                                         ItemName = a.Item,
                                                         ItemId = a.ItemId,
                                                         Quantity = a.Quantity,
                                                         ItemAmount = a.ItemTotal,
                                                         Gstamount = a.Gst,
                                                         UnitType = a.UnitTypeId,
                                                         UnitTypeName=c.UnitName,
                                                         PricePerUnit = a.Price,
                                                         GstPercentage = b.Gstper,
                                                         Hsncode = b.Hsncode,
                                                     }).ToList();

                List<PODeliveryAddressModel> addresslist = (from a in Context.PodeliveryAddresses.Where(a => a.Poid == PurchaseOrder.Id)
                                                            select new PODeliveryAddressModel
                                                            {
                                                                Aid=a.Aid,
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
                var PurchaseOrder = (from a in Context.PurchaseOrders
                                     join b in Context.SupplierMasters on a.FromSupplierId equals b.SupplierId
                                     join c in Context.Companies on a.ToCompanyId equals c.CompanyId
                                     join d in Context.Sites on a.SiteId equals d.SiteId
                                     where a.IsDeleted == false
                                     select new PurchaseOrderView
                                     {
                                         Id = a.Id,
                                         SiteId = a.SiteId,
                                         SiteName = d.SiteName,
                                         Poid = a.Poid,
                                         FromSupplierId = a.FromSupplierId,
                                         SupplierName = b.SupplierName,
                                         ToCompanyId = a.ToCompanyId,
                                         CompanyName = c.CompanyName,
                                         TotalAmount = a.TotalAmount,
                                         Description = a.Description,
                                         DeliveryShedule = a.DeliveryShedule,
                                         TotalDiscount = a.TotalDiscount,
                                         TotalGstamount = a.TotalGstamount,
                                         BillingAddress = a.BillingAddress,
                                         CreatedBy = a.CreatedBy,
                                         CreatedOn = a.CreatedOn,
                                     });
                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    PurchaseOrder = PurchaseOrder.Where(u =>
                        u.SupplierName.ToLower().Contains(searchText) ||
                        u.TotalGstamount.ToString().Contains(searchText) ||
                        u.TotalAmount.ToString().Contains(searchText) ||
                        u.CompanyName.ToLower().Contains(searchText) ||
                        u.Poid.ToLower().Contains(searchText)
                    );
                }

                if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
                {
                    searchText = searchText.ToLower();
                    switch (searchBy.ToLower())
                    {
                        case "suppliername":
                            PurchaseOrder = PurchaseOrder.Where(u => u.SupplierName.ToLower().Contains(searchText));
                            break;
                        case "totalgstamount":
                            PurchaseOrder = PurchaseOrder.Where(u => u.TotalGstamount.ToString().Contains(searchText));
                            break;
                        case "totalamount":
                            PurchaseOrder = PurchaseOrder.Where(u => u.TotalAmount.ToString().Contains(searchText));
                            break;
                        default:

                            break;
                    }
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    string sortOrder = sortBy.StartsWith("Ascending") ? "ascending" : "descending";
                    string field = sortBy.Substring(sortOrder.Length);

                    switch (field.ToLower())
                    {
                        case "suppliername":
                            if (sortOrder == "ascending")
                                PurchaseOrder = PurchaseOrder.OrderBy(u => u.SupplierName);
                            else if (sortOrder == "descending")
                                PurchaseOrder = PurchaseOrder.OrderByDescending(u => u.SupplierName);
                            break;
                        case "createdon":
                            if (sortOrder == "ascending")
                                PurchaseOrder = PurchaseOrder.OrderBy(u => u.CreatedOn);
                            else if (sortOrder == "descending")
                                PurchaseOrder = PurchaseOrder.OrderByDescending(u => u.CreatedOn);
                            break;
                        case "totalgstamount":
                            if (sortOrder == "ascending")
                                PurchaseOrder = PurchaseOrder.OrderBy(u => u.TotalGstamount);
                            else if (sortOrder == "descending")
                                PurchaseOrder = PurchaseOrder.OrderByDescending(u => u.TotalGstamount);
                            break;
                        case "totalamount":
                            if (sortOrder == "ascending")
                                PurchaseOrder = PurchaseOrder.OrderBy(u => u.TotalAmount);
                            else if (sortOrder == "descending")
                                PurchaseOrder = PurchaseOrder.OrderByDescending(u => u.TotalAmount);
                            break;
                        default:
                            break;
                    }
                }
                return PurchaseOrder;
            }
            catch (Exception)
            {
                throw;
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
                        Item = item.Item,
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
                        UnitTypeId= PurchaseOrderDetails.UnitTypeId,
                        Quantity=item.ShippingQuantity,
                    };
                    Context.PodeliveryAddresses.Add(PurchaseAddress);
                }

                    await Context.SaveChangesAsync();
                response.code = (int)HttpStatusCode.OK;
                response.message = "Purchase Order Inserted Successfully";
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
                    response.message = $"Purchase order with ID {PurchaseOrderDetails.Id} not found";
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
                response.message = "Purchase Order Updated Successfully";
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
                responseModel.message = "Purchase Order Updated Successfully";
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
