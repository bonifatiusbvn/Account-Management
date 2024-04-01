﻿using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Repository.InvoiceMaster
{
    public interface ISupplierInvoice
    {
        Task<IEnumerable<SupplierInvoiceModel>> GetSupplierInvoiceList(string? searchText, string? searchBy, string? sortBy);
        Task<ApiResponseModel> AddSupplierInvoice(SupplierInvoiceModel SupplierInvoiceDetails);
        Task<SupplierInvoiceModel> GetSupplierInvoiceById(Guid Id);
        Task<ApiResponseModel> UpdateSupplierInvoice(SupplierInvoiceModel SupplierInvoiceDetails);
        Task<ApiResponseModel> DeleteSupplierInvoice(Guid Id);
        Task<ApiResponseModel> InsertMultipleSupplierItemDetails(List<SupplierInvoiceMasterView> SupplierItemDetails);
        string CheckSupplierInvoiceNo();
    }
}
