﻿using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Repository.PurchaseOrder
{
    public interface IPurchaseOrder
    {
        Task<IEnumerable<PurchaseOrderView>> GetPurchaseOrderList();
        Task<ApiResponseModel> AddPurchaseOrderDetails(PurchaseOrderView PurchaseOrderDetails);
        Task<PurchaseOrderView> GetPurchaseOrderDetailsById(Guid POId);
        Task<ApiResponseModel> UpdatePurchaseOrderDetails(PurchaseOrderView PurchaseOrderDetails);
        Task<ApiResponseModel> DeletePurchaseOrderDetails(Guid POId);
        string CheckPONo();
    }
}
