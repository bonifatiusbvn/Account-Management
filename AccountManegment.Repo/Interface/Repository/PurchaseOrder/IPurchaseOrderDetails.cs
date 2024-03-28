using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Repository.PurchaseOrder
{
    public interface IPurchaseOrderDetails
    {
        Task<IEnumerable<PurchaseOrderDetailsModel>> GetPurchaseOrderDetailsList();
        Task<ApiResponseModel> AddPurchaseOrderDetails(PurchaseOrderDetailsModel PurchaseOrderDetails);
        Task<PurchaseOrderDetailsModel> GetPurchaseOrderDetailsById(int Id);
        Task<ApiResponseModel> UpdatePurchaseOrderDetails(PurchaseOrderDetailsModel PurchaseOrderDetails);
        Task<ApiResponseModel> DeletePurchaseOrderDetails(int Id);
    }
}
