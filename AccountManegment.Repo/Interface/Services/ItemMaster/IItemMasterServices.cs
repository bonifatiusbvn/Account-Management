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
        Task<IEnumerable<ItemMasterModel>> GetItemList(string? searchText, string? searchBy, string? sortBy);
        Task<ApiResponseModel> AddItemDetails(ItemMasterModel ItemDetails);
        Task<ItemMasterModel> GetItemDetailsById(Guid ItemId);
        Task<ApiResponseModel> UpdateItemDetails(ItemMasterModel ItemDetails);
        Task<IEnumerable<UnitMasterView>> GetAllUnitType();
        Task<ApiResponseModel> ItemIsApproved(Guid ItemId);
        Task<ApiResponseModel> DeleteItemDetails(Guid ItemId);
        Task<IEnumerable<ItemMasterModel>> GetItemNameList();
    }
}
