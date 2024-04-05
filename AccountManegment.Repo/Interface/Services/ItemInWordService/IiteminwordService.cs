﻿using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemInWord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Services.ItemInWordService
{
    public interface IiteminwordService
    {
        Task<IEnumerable<ItemInWordModel>> GetItemInWordList(string? searchText, string? searchBy, string? sortBy, Guid? siteId);
        Task<ApiResponseModel> AddItemInWordDetails(ItemInWordModel ItemInWordDetails);
        Task<ItemInWordModel> GetItemInWordtDetailsById(Guid InwordId);
        Task<ApiResponseModel> UpdateItemInWordDetails(ItemInWordModel ItemInWordDetails);
        Task<ApiResponseModel> ItemInWordIsApproved(Guid InwordId);
        Task<ApiResponseModel> DeleteItemInWord(Guid InwordId);
    }
}