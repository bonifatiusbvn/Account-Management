﻿using AccountManagement.DBContext.Models.API;
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
        Task<IEnumerable<SupplierModel>> GetSupplierList(string? searchText, string? searchBy, string? sortBy);

        Task<SupplierModel> GetSupplierById(Guid SupplierId);

        Task<ApiResponseModel> CreateSupplier(SupplierModel CreateUser);
        Task<ApiResponseModel> UpdateSupplierDetails(SupplierModel UpdateUser);

        Task<ApiResponseModel> ActiveDeactiveSupplier(Guid UserId);

        Task<ApiResponseModel> DeleteSupplierDetails(Guid SupplierId);
        Task<IEnumerable<SupplierModel>> GetSupplierNameList();
        Task<ApiResponseModel> ImportSupplierListFromExcel(List<SupplierModel> supplierList);
        Task<ApiResponseModel> MultipleSupplierIsApproved(SupplierIsApprovedMasterModel SupplierList);
    }
}
