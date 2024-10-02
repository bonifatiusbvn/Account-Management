﻿using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Services.PurchaseOrderService
{
    public interface IPurchaseOrderServices
    {
        Task<IEnumerable<PurchaseOrderView>> GetPurchaseOrderList(string? searchText, string? searchBy, string? sortBy);
        Task<ApiResponseModel> AddPurchaseOrderDetails(PurchaseOrderView PurchaseOrderDetails);
        Task<PurchaseOrderMasterView> GetPurchaseOrderDetailsById(Guid POId);
        Task<ApiResponseModel> UpdatePurchaseOrderDetails(PurchaseOrderView PurchaseOrderDetails);
        Task<ApiResponseModel> DeletePurchaseOrderDetails(Guid POId);
        string CheckPONo(Guid? CompanyId);
        Task<ApiResponseModel> InsertMultiplePurchaseOrderDetails(PurchaseOrderMasterView PurchaseOrderDetails);
        Task<ApiResponseModel> UpdateMultiplePurchaseOrderDetails(PurchaseOrderMasterView PurchaseOrderDetails);
        Task<ApiResponseModel> PurchaseOrderIsApproved(POIsApprovedMasterModel POIdList);
        Task<SupplierInvoiceMasterView> GetPODetailsInInvoice(Guid POId);

        Task<POPendingData> GetPRPendingData(string PRId);
    }
}
