using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.SupplierMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Repository.Supplier
{
    public interface ISupplierMaster
    {
        Task<IEnumerable<SupplierModel>> GetSupplierList(string? searchText, string? searchBy, string? sortBy);

        Task<SupplierModel> GetSupplierById(Guid SupplierId);

        Task<ApiResponseModel> CreateSupplier(SupplierModel CreateUser);
        Task<ApiResponseModel> UpdateSupplierDetails(SupplierModel UpdateUser);

        Task<ApiResponseModel> ActiveDeactiveSupplier(Guid UserId);
        Task<ApiResponseModel> DeleteSupplierDetails(Guid SupplierId);
        Task<IEnumerable<SupplierModel>> GetSupplierNameList();
    }
}
