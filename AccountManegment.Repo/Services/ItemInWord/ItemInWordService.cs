using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemInWord;
using AccountManagement.Repository.Interface.Repository.ItemInWord;
using AccountManagement.Repository.Interface.Repository.PurchaseRequest;
using AccountManagement.Repository.Interface.Services.ItemInWordService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Services.ItemInWord
{
    public class ItemInWordService : IiteminwordService
    {
        private readonly Iiteminword itemInWord;

        public ItemInWordService(Iiteminword itemInWord)
        {
            this.itemInWord = itemInWord;
        }
        public async Task<ApiResponseModel> AddItemInWordDetails(ItemInWordModel ItemInWordDetails)
        {
            return await itemInWord.AddItemInWordDetails(ItemInWordDetails);
        }

        public async Task<ApiResponseModel> DeleteItemInWord(Guid InwordId)
        {
            return await itemInWord.DeleteItemInWord(InwordId);
        }

        public async Task<IEnumerable<ItemInWordModel>> GetItemInWordList(string? searchText, string? searchBy, string? sortBy, Guid? siteId)
        {
            return await itemInWord.GetItemInWordList(searchText, searchBy, sortBy, siteId);
        }

        public async Task<ItemInWordModel> GetItemInWordtDetailsById(Guid InwordId)
        {
            return await itemInWord.GetItemInWordtDetailsById(InwordId);
        }

        public async Task<ApiResponseModel> ItemInWordIsApproved(Guid InwordId)
        {
            return await itemInWord.ItemInWordIsApproved(InwordId);
        }

        public async Task<ApiResponseModel> UpdateItemInWordDetails(ItemInWordModel ItemInWordDetails)
        {
            return await itemInWord.UpdateItemInWordDetails(ItemInWordDetails);
        }
        public async Task<ApiResponseModel> InsertMultipleItemInWordDetails(List<ItemInWordMasterView> ItemInWordDetails)
        {
            return await itemInWord.InsertMultipleItemInWordDetails(ItemInWordDetails);
        }
    }
}
