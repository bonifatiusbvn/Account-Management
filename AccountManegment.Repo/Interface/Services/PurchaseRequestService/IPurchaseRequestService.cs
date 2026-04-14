using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.PurchaseRequest;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Services.PurchaseRequestService
{
    public interface IPurchaseRequestService
    {
        Task<IEnumerable<PurchaseRequestModel>> GetPurchaseRequestList(string? searchText, string? searchBy, string? sortBy, Guid? siteId);
        Task<ApiResponseModel> AddPurchaseRequestDetails(PurchaseRequestModel PurchaseRequestDetails);
        Task<PurchaseRequestModel> GetPurchaseRequestDetailsById(Guid PurchaseId);
        Task<ApiResponseModel> UpdatePurchaseRequestDetails(PurchaseRequestModel PurchaseRequestDetails);
        Task<ApiResponseModel> DeletePurchaseRequest(Guid PurchaseId);
        Task<ApiResponseModel> PurchaseRequestIsApproved(Guid PurchaseId);
        string CheckPRNo();
        Task<ApiResponseModel> MultiplePurchaseRequestIsApproved(PRIsApprovedMasterModel PRIdList);
    }
}
