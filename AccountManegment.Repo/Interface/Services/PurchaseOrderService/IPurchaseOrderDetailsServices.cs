using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Services.PurchaseOrderService
{
    public interface IPurchaseOrderDetailsServices
    {
        Task<IEnumerable<PurchaseOrderDetailsModel>> GetPurchaseOrderDetailsList();
        Task<ApiResponseModel> AddPurchaseOrderDetails(PurchaseOrderDetailsModel PurchaseOrderDetails);
        Task<PurchaseOrderDetailsModel> GetPurchaseOrderDetailsById(int Id);
        Task<ApiResponseModel> UpdatePurchaseOrderDetails(PurchaseOrderDetailsModel PurchaseOrderDetails);
        Task<ApiResponseModel> DeletePurchaseOrderDetails(int Id);
        Task<ApiResponseModel> InsertMultiplePurchaseOrderDetails(List<PurchaseOrderDetailsModel> PurchaseOrderDetails);
    }
}
