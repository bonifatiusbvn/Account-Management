using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
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

        public string CheckPONo(Guid? CompanyId)
        {
            return PurchaseOrder.CheckPONo(CompanyId);
        }

        public async Task<ApiResponseModel> DeletePurchaseOrderDetails(Guid POId)
        {
            return await PurchaseOrder.DeletePurchaseOrderDetails(POId);
        }

        public async Task<PurchaseOrderMasterView> GetPurchaseOrderDetailsById(Guid POId)
        {
            return await PurchaseOrder.GetPurchaseOrderDetailsById(POId);
        }

        public async Task<IEnumerable<PurchaseOrderView>> GetPurchaseOrderList(string? searchText, string? searchBy, string? sortBy)
        {
            return await PurchaseOrder.GetPurchaseOrderList(searchText, searchBy, sortBy);
        }

        public async Task<ApiResponseModel> InsertMultiplePurchaseOrderDetails(PurchaseOrderMasterView PurchaseOrderDetails)
        {
            return await PurchaseOrder.InsertMultiplePurchaseOrderDetails(PurchaseOrderDetails);
        }

        public async Task<ApiResponseModel> UpdateMultiplePurchaseOrderDetails(PurchaseOrderMasterView PurchaseOrderDetails)
        {
            return await PurchaseOrder.UpdateMultiplePurchaseOrderDetails(PurchaseOrderDetails);
        }

        public async Task<ApiResponseModel> UpdatePurchaseOrderDetails(PurchaseOrderView PurchaseOrderDetails)
        {
            return await PurchaseOrder.UpdatePurchaseOrderDetails(PurchaseOrderDetails);
        }
        public async Task<ApiResponseModel> PurchaseOrderIsApproved(POIsApprovedMasterModel POIdList)
        {
            return await PurchaseOrder.PurchaseOrderIsApproved(POIdList);
        }

        public async Task<SupplierInvoiceMasterView> GetPODetailsInInvoice(Guid POId)
        {
            return await PurchaseOrder.GetPODetailsInInvoice(POId);
        }

        public async Task<IEnumerable<POPendingData>> GetPRPendingData(string PRId)
        {
            return await PurchaseOrder.GetPRPendingData(PRId);
        }

        public async Task<IEnumerable<InvoiceDetailsViewModel>> GetInvoiceDetailsByPOId(string PRId)
        {
            return await PurchaseOrder.GetInvoiceDetailsByPOId(PRId);
        }
    }
}
