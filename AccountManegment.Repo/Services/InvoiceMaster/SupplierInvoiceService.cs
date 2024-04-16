﻿using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.Repository.Interface.Repository.InvoiceMaster;
using AccountManagement.Repository.Interface.Services.InvoiceMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Services.InvoiceMaster
{
    public class SupplierInvoiceService : ISupplierInvoiceService
    {
        public SupplierInvoiceService(ISupplierInvoice supplierInvoice)
        {
            SupplierInvoice = supplierInvoice;
        }
        public ISupplierInvoice SupplierInvoice { get; }

        public async Task<ApiResponseModel> AddSupplierInvoice(SupplierInvoiceModel SupplierInvoiceDetails)
        {
            return await SupplierInvoice.AddSupplierInvoice(SupplierInvoiceDetails);
        }

        public async Task<ApiResponseModel> DeleteSupplierInvoice(Guid Id)
        {
            return await SupplierInvoice.DeleteSupplierInvoice(Id);
        }

        public async Task<InvoiceTotalAmount> GetInvoiceDetailsById(Guid CompanyId, Guid SupplierId)
        {
            return await SupplierInvoice.GetInvoiceDetailsById(CompanyId, SupplierId);
        }

        public async Task<SupplierInvoiceMasterView> GetSupplierInvoiceById(Guid Id)
        {
            return await SupplierInvoice.GetSupplierInvoiceById(Id);
        }

        public async Task<IEnumerable<SupplierInvoiceModel>> GetSupplierInvoiceList(string? searchText, string? searchBy, string? sortBy)
        {
            return await SupplierInvoice.GetSupplierInvoiceList(searchText, searchBy, sortBy);
        }

        public async Task<ApiResponseModel> InsertMultipleSupplierItemDetails(SupplierInvoiceMasterView SupplierItemDetails)
        {
            return await SupplierInvoice.InsertMultipleSupplierItemDetails(SupplierItemDetails);
        }

        public async Task<ApiResponseModel> UpdateSupplierInvoice(SupplierInvoiceModel SupplierInvoiceDetails)
        {
            return await SupplierInvoice.UpdateSupplierInvoice(SupplierInvoiceDetails);
        }

        public string CheckSupplierInvoiceNo()
        {
            return SupplierInvoice.CheckSupplierInvoiceNo();
        }

        public async Task<IEnumerable<SupplierInvoiceModel>> GetSupplierInvoiceDetailsById(Guid SupplierId)
        {
            return await SupplierInvoice.GetSupplierInvoiceDetailsById(SupplierId);
        }
    }
}
