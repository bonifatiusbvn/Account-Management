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
    public class PurchaseOrderServices : IPurchaseOrderServices
    {
        public PurchaseOrderServices(IPurchaseOrder purchaseOrder)
        {
            PurchaseOrder = purchaseOrder;
        }
        public IPurchaseOrder PurchaseOrder { get; }

        public async Task<ApiResponseModel> AddPurchaseOrderDetails(PurchaseOrderView PurchaseOrderDetails)
        {
            return await PurchaseOrder.AddPurchaseOrderDetails(PurchaseOrderDetails);
        }

        public string CheckPONo()
        {
            return PurchaseOrder.CheckPONo();
        }

        public Task<ApiResponseModel> DeletePurchaseOrderDetails(Guid POId)
        {
            throw new NotImplementedException();
        }

        public async Task<PurchaseOrderMasterView> GetPurchaseOrderDetailsById(Guid POId)
        {
            return await PurchaseOrder.GetPurchaseOrderDetailsById(POId);
        }

        public async Task<IEnumerable<PurchaseOrderView>> GetPurchaseOrderList(string? searchText, string? searchBy, string? sortBy)
        {
            return await PurchaseOrder.GetPurchaseOrderList(searchText, searchBy, sortBy);
        }

        public async Task<ApiResponseModel> InsertMultiplePurchaseOrderDetails(List<PurchaseOrderMasterView> PurchaseOrderDetails)
        {
            return await PurchaseOrder.InsertMultiplePurchaseOrderDetails(PurchaseOrderDetails);
        }

        public async Task<ApiResponseModel> UpdatePurchaseOrderDetails(PurchaseOrderView PurchaseOrderDetails)
        {
            return await PurchaseOrder.UpdatePurchaseOrderDetails(PurchaseOrderDetails);
        }
    }
}
