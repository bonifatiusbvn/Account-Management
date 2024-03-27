using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.PurchaseRequest;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Repository.PurchaseRequest
{
    public interface IPurchaseRequest
    {
        Task<IEnumerable<PurchaseRequestModel>> GetPurchaseRequestList(string? searchText, string? searchBy, string? sortBy);
        Task<ApiResponseModel> AddPurchaseRequestDetails(PurchaseRequestModel PurchaseRequestDetails);
        Task<PurchaseRequestModel> GetPurchaseRequestDetailsById(Guid PurchaseId);
        Task<ApiResponseModel> UpdatePurchaseRequestDetails(PurchaseRequestModel PurchaseRequestDetails);
        Task<ApiResponseModel> PurchaseRequestIsApproved(Guid PurchaseId);
        Task<ApiResponseModel> DeletePurchaseRequest(Guid PurchaseId);
        string CheckPRNo();
    }
}
