using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Services.ItemMaster
{
    public interface IItemMasterServices
    {
        Task<IEnumerable<ItemMasterModel>> GetItemList();
        Task<ApiResponseModel> AddItemDetails(ItemMasterModel ItemDetails);
        Task<ItemMasterModel> GetItemDetailsById(Guid ItemId);
        Task<ApiResponseModel> UpdateItemDetails(ItemMasterModel ItemDetails);
    }
}
