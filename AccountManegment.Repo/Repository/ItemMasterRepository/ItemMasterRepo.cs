using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Repository.ItemMaster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Repository.ItemMasterRepository
{
    public class ItemMasterRepo : IItemMaster
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
                if (ItemDetails != null)
                {
                    var existingItem = Context.ItemMasters.FirstOrDefault(x => x.ItemName == ItemDetails.ItemName);
                    if (existingItem != null)
                    {
                        response.code = 403;
                        response.message = "ItemDetails Already Inserted";
                    }
                    else
                    {
                        var ItemMaster = new ItemMaster()
                        {
                            ItemId = Guid.NewGuid(),
                            ItemName = ItemDetails.ItemName,
                            UnitType = ItemDetails.UnitType,
                            PricePerUnit = ItemDetails.PricePerUnit,
                            IsWithGst = ItemDetails.IsWithGst,
                            Gstamount = ItemDetails.Gstamount,
                            Gstper = ItemDetails.Gstper,
                            Hsncode = ItemDetails.Hsncode,
                            IsDeleted = ItemDetails.IsDeleted,
                            IsApproved = ItemDetails.IsApproved,
                            CreatedBy = ItemDetails.CreatedBy,
                            CreatedOn = DateTime.Now,
                        };
                        response.code = (int)HttpStatusCode.OK;
                        response.message = "ItemDetails Successfully Inserted";
                        Context.ItemMasters.Add(ItemMaster);
                        Context.SaveChanges();
                    }
                }
                else
                {
                    response.code = 400;
                    response.message = "ItemDetails is null";
                }
            }
            catch (Exception ex)
            {

                response.code = 500;
                response.message = "An error occurred while processing the request";
            }
            return response;
        }


        public async Task<ApiResponseModel> DeleteItemDetails(Guid ItemId)
        {
            ApiResponseModel response = new ApiResponseModel();
            var GetItemdata = Context.ItemMasters.Where(a => a.ItemId == ItemId).FirstOrDefault();

            if (GetItemdata != null)
            {
                GetItemdata.IsDeleted = true;
                Context.ItemMasters.Update(GetItemdata);
                Context.SaveChanges();
                response.code = 200;
                response.data = GetItemdata;
                response.message = "Item is Deleted Successfully";
            }
            return response;
        }

        public async Task<IEnumerable<UnitMasterView>> GetAllUnitType()
        {
            try
            {
                IEnumerable<UnitMasterView> UnitType = Context.UnitMasters.ToList().Select(a => new UnitMasterView
                {
                    UnitId = a.UnitId,
                    UnitName = a.UnitName,
                });
                return UnitType;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                                UnitTypeName = b.UnitName,
                                PricePerUnit = a.PricePerUnit,
                                IsWithGst = a.IsWithGst,
                                Gstamount = a.Gstamount,
                                Gstper = a.Gstper,
                                Hsncode = a.Hsncode,
                                IsApproved = a.IsApproved,
                                CreatedBy = a.CreatedBy,
                                CreatedOn = a.CreatedOn,
                            }).First();
                return ItemList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ItemMasterModel>> GetItemList(string? searchText, string? searchBy, string? sortBy)
        {
            try
            {
                var ItemList = (from a in Context.ItemMasters
                                join b in Context.UnitMasters on a.UnitType equals b.UnitId
                                where a.IsDeleted == false
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
                                    CreatedOn = a.CreatedOn,
                                });
                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    ItemList = ItemList.Where(u =>
                        u.ItemName.ToLower().Contains(searchText) ||
                        u.PricePerUnit.ToString().Contains(searchText) ||
                        u.IsApproved.ToString().Contains(searchText)
                    );
                }

                if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
                {
                    searchText = searchText.ToLower();
                    switch (searchBy.ToLower())
                    {
                        case "itemname":
                            ItemList = ItemList.Where(u => u.ItemName.ToLower().Contains(searchText));
                            break;
                        case "isapproved":
                            ItemList = ItemList.Where(u => u.IsApproved.ToString().Contains(searchText));
                            break;
                        case "perunitprice":
                            ItemList = ItemList.Where(u => u.PricePerUnit.ToString().Contains(searchText));
                            break;
                        default:

                            break;
                    }
                }

                if (string.IsNullOrEmpty(sortBy))
                {
                    ItemList = ItemList.OrderByDescending(u => u.CreatedOn);
                }
                else
                {
                    string sortOrder = sortBy.StartsWith("Ascending") ? "ascending" : "descending";
                    string field = sortBy.Substring(sortOrder.Length);

                    switch (field.ToLower())
                    {
                        case "createdon":
                            if (sortOrder == "ascending")
                                ItemList = ItemList.OrderBy(u => u.CreatedOn);
                            else if (sortOrder == "descending")
                                ItemList = ItemList.OrderByDescending(u => u.CreatedOn);
                            break;
                        case "itemname":
                            if (sortOrder == "ascending")
                                ItemList = ItemList.OrderBy(u => u.ItemName);
                            else if (sortOrder == "descending")
                                ItemList = ItemList.OrderByDescending(u => u.ItemName);
                            break;
                        case "perunitprice":
                            if (sortOrder == "ascending")
                                ItemList = ItemList.OrderBy(u => u.PricePerUnit);
                            else if (sortOrder == "descending")
                                ItemList = ItemList.OrderByDescending(u => u.PricePerUnit);
                            break;
                        default:
                            break;
                    }
                }
                return ItemList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<ItemMasterModel>> GetItemNameList()
        {
            try
            {
                IEnumerable<ItemMasterModel> ItemName = Context.ItemMasters.ToList().Select(a => new ItemMasterModel
                {
                    ItemId = a.ItemId,
                    ItemName = a.ItemName,
                });
                return ItemName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ApiResponseModel> ItemIsApproved(Guid ItemId)
        {
            ApiResponseModel response = new ApiResponseModel();
            var ItemData = Context.ItemMasters.Where(a => a.ItemId == ItemId).FirstOrDefault();

            if (ItemData != null)
            {

                if (ItemData.IsApproved == true)
                {
                    ItemData.IsApproved = false;
                    Context.ItemMasters.Update(ItemData);
                    Context.SaveChanges();
                    response.code = 200;
                    response.data = ItemData;
                    response.message = "Item Is UnApproved!";
                }

                else
                {
                    ItemData.IsApproved = true;
                    Context.ItemMasters.Update(ItemData);
                    Context.SaveChanges();
                    response.code = 200;
                    response.data = ItemData;
                    response.message = "Item Is Approved Successfully";
                }
            }
            return response;
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

        public async Task<ApiResponseModel> InsertItemDetailsFromExcel(List<ItemMasterModel> itemDetailsList)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                foreach (var itemDetails in itemDetailsList)
                {
                    var existingItem = Context.ItemMasters.FirstOrDefault(x => x.ItemName == itemDetails.ItemName);
                    if (existingItem != null)
                    {
                        response.code = 400;
                        response.message = "Item is Already Inserted";
                    }
                    else
                    {
                        var itemMaster = new ItemMaster()
                        {
                            ItemId = Guid.NewGuid(),
                            ItemName = itemDetails.ItemName,
                            UnitType = itemDetails.UnitType,
                            PricePerUnit = itemDetails.PricePerUnit,
                            IsWithGst = itemDetails.IsWithGst,
                            Gstamount = itemDetails.Gstamount,
                            Gstper = itemDetails.Gstper,
                            Hsncode = itemDetails.Hsncode,
                            IsDeleted = itemDetails.IsDeleted,
                            IsApproved = itemDetails.IsApproved,
                            CreatedBy = itemDetails.CreatedBy,
                            CreatedOn = DateTime.Now,
                        };
                        Context.ItemMasters.Add(itemMaster);
                        await Context.SaveChangesAsync();
                        response.code = (int)HttpStatusCode.OK;
                        response.message = "Item Details Successfully Inserted";
                    }
                }
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.message = "An error occurred while processing the request";
            }
            return response;
        }

        public async Task<IEnumerable<ItemMasterModel>> GetAllItemDetailsList(string? searchText)
        {
            var ItemList = (from a in Context.ItemMasters
                            join b in Context.UnitMasters on a.UnitType equals b.UnitId
                            where a.IsDeleted == false
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
                                CreatedOn = a.CreatedOn,
                            });

            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.ToLower();
                ItemList = ItemList.Where(u =>
                    u.ItemName.ToLower().Contains(searchText)
                );
            }

            return ItemList;
        }
    }
}

