using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.Repository.Interface.Repository.PurchaseOrder;
using AccountManagement.Repository.Interface.Services.PurchaseOrderService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Services.PurchaseOrder
{
    public class PurchaseOrderDetailsServices : IPurchaseOrderDetailsServices
    {
        public PurchaseOrderDetailsServices(IPurchaseOrderDetails pODetails)
        {
            PODetails = pODetails;
        }
        public IPurchaseOrderDetails PODetails { get; }

        public async Task<ApiResponseModel> AddPurchaseOrderDetails(PurchaseOrderDetailsModel PurchaseOrderDetails)
        {
            return await PODetails.AddPurchaseOrderDetails(PurchaseOrderDetails);              
        }

        public Task<ApiResponseModel> DeletePurchaseOrderDetails(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<PurchaseOrderDetailsModel> GetPurchaseOrderDetailsById(int Id)
        {
            return await PODetails.GetPurchaseOrderDetailsById(Id);
        }

        public async Task<IEnumerable<PurchaseOrderDetailsModel>> GetPurchaseOrderDetailsList()
        {
            return await PODetails.GetPurchaseOrderDetailsList();
        }

        public async Task<ApiResponseModel> InsertMultiplePurchaseOrderDetails(List<PurchaseOrderDetailsModel> PurchaseOrderDetails)
        {
            return await PODetails.InsertMultiplePurchaseOrderDetails(PurchaseOrderDetails);
        }

        public async Task<ApiResponseModel> UpdatePurchaseOrderDetails(PurchaseOrderDetailsModel PurchaseOrderDetails)
        {
            return await PODetails.UpdatePurchaseOrderDetails(PurchaseOrderDetails);
        }
    }
}
