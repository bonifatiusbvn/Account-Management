using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.SupplierMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Repository.ItemMaster
{
    public interface IItemMaster
    {
        Task<IEnumerable<ItemMasterModel>> GetItemList(string? searchText, string? searchBy, string? sortBy);
        Task<ApiResponseModel> AddItemDetails(ItemMasterModel ItemDetails);
        Task<ItemMasterModel> GetItemDetailsById(Guid ItemId);
        Task<ApiResponseModel> UpdateItemDetails(ItemMasterModel ItemDetails);
        Task<IEnumerable<UnitMasterView>> GetAllUnitType();
        Task<ApiResponseModel> ItemIsApproved(Guid ItemId);
        Task<ApiResponseModel> DeleteItemDetails(Guid ItemId);
        Task<IEnumerable<ItemMasterModel>> GetItemNameList();
        Task<IEnumerable<ItemMasterModel>> GetAllItemDetailsList(string? searchText);

        Task<ApiResponseModel> InsertItemDetailsFromExcel(List<ItemMasterModel> itemDetailsList);
        Task<List<POItemDetailsModel>> GetItemDetailsListById(Guid ItemId);
        Task<ApiResponseModel> MutipleItemsIsApproved(ItemIsApprovedMasterModel ItemIdList);
        Task<SupplierInvoiceList> GetItemHistory(Guid ItemId);
    }
}
