using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.Repository.Interface.Repository.ItemMaster;
using AccountManagement.Repository.Interface.Services.ItemMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Services.ItemMaster
{
    public class ItemMasterServices : IItemMasterServices
    {
        public ItemMasterServices(IItemMaster itemMaster)
        {
            ItemMaster = itemMaster;
        }
        public IItemMaster ItemMaster { get; }

        public async Task<ApiResponseModel> AddItemDetails(ItemMasterModel ItemDetails)
        {
            return await ItemMaster.AddItemDetails(ItemDetails);
        }

        public async Task<ApiResponseModel> DeleteItemDetails(Guid ItemId)
        {
            return await ItemMaster.DeleteItemDetails(ItemId);
        }

        public async Task<IEnumerable<UnitMasterView>> GetAllUnitType()
        {
            return await ItemMaster.GetAllUnitType();
        }

        public async Task<ItemMasterModel> GetItemDetailsById(Guid ItemId)
        {
            return await ItemMaster.GetItemDetailsById(ItemId);
        }

        public async Task<IEnumerable<ItemMasterModel>> GetItemList(string? searchText, string? searchBy, string? sortBy)
        {
            return await ItemMaster.GetItemList(searchText, searchBy, sortBy);
        }

        public async Task<IEnumerable<ItemMasterModel>> GetItemNameList()
        {
            return await ItemMaster.GetItemNameList();
        }

        public async Task<ApiResponseModel> ItemIsApproved(Guid ItemId)
        {
            return await ItemMaster.ItemIsApproved(ItemId);
        }

        public async Task<ApiResponseModel> UpdateItemDetails(ItemMasterModel ItemDetails)
        {
            return await ItemMaster.UpdateItemDetails(ItemDetails);
        }

        public async Task<ApiResponseModel> InsertItemDetailsFromExcel(List<ItemMasterModel> itemDetailsList)
        {
            return await ItemMaster.InsertItemDetailsFromExcel(itemDetailsList);
        }

        public async Task<IEnumerable<ItemMasterModel>> GetAllItemDetailsList(string? searchText)
        {
            return await ItemMaster.GetAllItemDetailsList(searchText);
        }

        public async Task<List<POItemDetailsModel>> GetItemDetailsListById(Guid ItemId)
        {
            return await ItemMaster.GetItemDetailsListById(ItemId);
        }
    }
}
