using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SupplierMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Repository.Supplier;
using AccountManagement.Repository.Interface.Services.SupplierService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Services.Supplier
{
    public class SupplierServices : ISupplierServices
    {
        public SupplierServices(ISupplierMaster supplier)
        {
            Supplier = supplier;
        }

        public ISupplierMaster Supplier { get; }

        public async Task<ApiResponseModel> ActiveDeactiveSupplier(Guid UserId)
        {
            return await Supplier.ActiveDeactiveSupplier(UserId);
        }

        public async Task<ApiResponseModel> CreateSupplier(SupplierModel CreateUser)
        {
            return await Supplier.CreateSupplier(CreateUser);
        }

        public async Task<ApiResponseModel> DeleteSupplierDetails(Guid SupplierId)
        {
            return await Supplier.DeleteSupplierDetails(SupplierId);
        }

        public async Task<SupplierModel> GetSupplierById(Guid SupplierId)
        {
            return await Supplier.GetSupplierById(SupplierId);
        }

        public async Task<IEnumerable<SupplierModel>> GetSupplierList(string? searchText, string? searchBy, string? sortBy)
        {
            return await Supplier.GetSupplierList(searchText, searchBy, sortBy);
        }

        public async Task<IEnumerable<SupplierModel>> GetSupplierNameList()
        {
            return await Supplier.GetSupplierNameList();
        }

        public async Task<ApiResponseModel> UpdateSupplierDetails(SupplierModel UpdateUser)
        {
            return await Supplier.UpdateSupplierDetails(UpdateUser);
        }

        public async Task<ApiResponseModel> ImportSupplierListFromExcel(List<SupplierModel> supplierList)
        {
            return await Supplier.ImportSupplierListFromExcel(supplierList);
        }
    }
}
