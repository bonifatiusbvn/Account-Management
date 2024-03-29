﻿using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.Repository.Interface.Repository.ItemMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Repository.ItemMasterRepository
{
    public class ItemMasterRepo:IItemMaster
    {
        public ItemMasterRepo(DbaccManegmentContext context)
        {
            Context = context;
        }
        public DbaccManegmentContext Context { get; }

        public async Task<ApiResponseModel> AddItemDetails(ItemMasterModel ItemDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var ItemMaster = new ItemMaster()
                {
                    ItemId = Guid.NewGuid(),
                    ItemName = ItemDetails.ItemName,
                    UnitType = ItemDetails.UnitType,
                    PricePerUnit = ItemDetails.PricePerUnit,
                    IsWithGst = ItemDetails.IsWithGst,
                    Gstamount = ItemDetails.Gstamount,
                    Gstper=ItemDetails.Gstper,
                    Hsncode = ItemDetails.Hsncode,
                    IsApproved = ItemDetails.IsApproved,
                    CreatedBy= ItemDetails.CreatedBy,
                    CreatedOn = DateTime.Now,
                };
                response.code = (int)HttpStatusCode.OK;
                response.message = "ItemDetails Successfully Inserted";
                Context.ItemMasters.Add(ItemMaster);
                Context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

        public async Task<ItemMasterModel> GetItemDetailsById(Guid ItemId)
        {
            ItemMasterModel ItemList = new ItemMasterModel();
            try
            {
                ItemList = (from a in Context.ItemMasters.Where(x => x.ItemId == ItemId)
                            join b in Context.UnitMasters on a.UnitType equals b.UnitId
                            select new ItemMasterModel
                            {
                                ItemId = a.ItemId,
                                ItemName = a.ItemName,
                                UnitType = a.UnitType,
                                UnitTypeName=b.UnitName,
                                PricePerUnit = a.PricePerUnit,
                                IsWithGst = a.IsWithGst,
                                Gstamount = a.Gstamount,
                                Gstper = a.Gstper,
                                Hsncode = a.Hsncode,
                                IsApproved = a.IsApproved,
                                CreatedBy=a.CreatedBy,
                                CreatedOn=a.CreatedOn,
                            }).First();
                return ItemList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ItemMasterModel>> GetItemList()
        {
            try
            {
                var ItemList = (from a in Context.ItemMasters
                                join b in Context.UnitMasters on a.UnitType equals b.UnitId
                                select new ItemMasterModel
                                {
                                    ItemId = a.ItemId,
                                    ItemName = a.ItemName,
                                    UnitType = a.UnitType,
                                    UnitTypeName = b.UnitName,
                                    PricePerUnit = a.PricePerUnit,
                                    IsWithGst = a.IsWithGst,
                                    Gstamount = a.Gstamount,
                                    Gstper = a.Gstper,
                                    Hsncode = a.Hsncode,
                                    IsApproved = a.IsApproved,
                                });
                return ItemList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ApiResponseModel> UpdateItemDetails(ItemMasterModel ItemDetails)
        {
            ApiResponseModel model = new ApiResponseModel();
            var ItemMaster = Context.ItemMasters.Where(e => e.ItemId == ItemDetails.ItemId).FirstOrDefault();
            try
            {
                if (ItemMaster != null)
                {
                    ItemMaster.ItemId = ItemDetails.ItemId;
                    ItemMaster.ItemName = ItemDetails.ItemName;
                    ItemMaster.UnitType = ItemDetails.UnitType;
                    ItemMaster.PricePerUnit = ItemDetails.PricePerUnit;
                    ItemMaster.IsWithGst = ItemDetails.IsWithGst;
                    ItemMaster.Gstamount = ItemDetails.Gstamount;
                    ItemMaster.Gstper = ItemDetails.Gstper;
                    ItemMaster.Hsncode = ItemDetails.Hsncode;
                    ItemMaster.IsApproved = ItemDetails.IsApproved;
                    ItemMaster.CreatedBy = ItemDetails.CreatedBy;
                    ItemMaster.CreatedOn = ItemDetails.CreatedOn;
                }
                Context.ItemMasters.Update(ItemMaster);
                Context.SaveChanges();
                model.code = 200;
                model.message = "ItemDetails Updated Successfully!";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return model;
        }
    }
}
