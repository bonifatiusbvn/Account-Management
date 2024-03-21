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
    public class ItemMasterServices:IItemMasterServices
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

        public async Task<ItemMasterModel> GetItemDetailsById(Guid ItemId)
        {
            return await ItemMaster.GetItemDetailsById(ItemId);
        }

        public async Task<IEnumerable<ItemMasterModel>> GetItemList()
        {
            return await ItemMaster.GetItemList();
        }

        public async Task<ApiResponseModel> UpdateItemDetails(ItemMasterModel ItemDetails)
        {
            return await ItemMaster.UpdateItemDetails(ItemDetails);
        }
    }
}
