using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SupplierMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Services.SupplierService
{
    public interface ISupplierServices
    {
        Task<IEnumerable<LoginView>> GetSupplierList(string? searchText, string? searchBy, string? sortBy);

        Task<LoginView> GetSupplierById(Guid SupplierId);

        Task<ApiResponseModel> CreateSupplier(SupplierModel CreateUser);
        Task<ApiResponseModel> UpdateSupplierDetails(SupplierModel UpdateUser);

        Task<ApiResponseModel> ActiveDeactiveSupplier(Guid UserId);
    }
}
