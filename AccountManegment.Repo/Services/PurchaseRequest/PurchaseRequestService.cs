using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.PurchaseRequest;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.Repository.Interface.Repository.PurchaseRequest;
using AccountManagement.Repository.Interface.Services.PurchaseRequestService;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Services.PurchaseRequest
{
    public class PurchaseRequestService:IPurchaseRequestService
    {
        private readonly IPurchaseRequest PurchaseRequest;
        public PurchaseRequestService(IPurchaseRequest purchaseRequest)
        {
            PurchaseRequest = purchaseRequest;
        }    

        public async Task<ApiResponseModel> AddPurchaseRequestDetails(PurchaseRequestModel PurchaseRequestDetails)
        {
            return await PurchaseRequest.AddPurchaseRequestDetails(PurchaseRequestDetails);
        }

        public async Task<ApiResponseModel> DeletePurchaseRequest(Guid PurchaseId)
        {
            return await PurchaseRequest.DeletePurchaseRequest(PurchaseId);
        }

        public async Task<PurchaseRequestModel> GetPurchaseRequestDetailsById(Guid PurchaseId)
        {
            return await PurchaseRequest.GetPurchaseRequestDetailsById(PurchaseId);
        }

        public async Task<IEnumerable<PurchaseRequestModel>> GetPurchaseRequestList(string? searchText, string? searchBy, string? sortBy,Guid? siteId)
        {
            return await PurchaseRequest.GetPurchaseRequestList(searchText, searchBy, sortBy,siteId);
        }

        public async Task<ApiResponseModel> UpdatePurchaseRequestDetails(PurchaseRequestModel PurchaseRequestDetails)
        {
            return await PurchaseRequest.UpdatePurchaseRequestDetails(PurchaseRequestDetails);
        }

        public async Task<ApiResponseModel> PurchaseRequestIsApproved(Guid PurchaseId)
        {
            return await PurchaseRequest.PurchaseRequestIsApproved(PurchaseId);
        }

        public string CheckPRNo()
        {
            return PurchaseRequest.CheckPRNo();  
        }
    }
}
