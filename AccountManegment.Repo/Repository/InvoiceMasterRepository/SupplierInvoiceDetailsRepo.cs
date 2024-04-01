using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.Repository.Interface.Repository.InvoiceMaster;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Repository.InvoiceMasterRepository
{
    public class SupplierInvoiceDetailsRepo : ISupplierInvoiceDetails
    {
        public SupplierInvoiceDetailsRepo(DbaccManegmentContext context)
        {
            Context = context;
        }

        public DbaccManegmentContext Context { get; }

        public async Task<ApiResponseModel> AddSupplierInvoiceDetails(SupplierInvoiceDetailsModel SupplierInvoiceDetails)
        {

            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var SupplierInvoice = new SupplierInvoiceDetail()
                {
                    RefInvoiceId = SupplierInvoiceDetails.RefInvoiceId,
                    Item = SupplierInvoiceDetails.Item,
                    UnitTypeId = SupplierInvoiceDetails.UnitTypeId,
                    Quantity = SupplierInvoiceDetails.Quantity,
                    Price = SupplierInvoiceDetails.Price,
                    DiscountPer = SupplierInvoiceDetails.DiscountPer,
                    DiscountAmount = SupplierInvoiceDetails.Discount,
                    Gst = SupplierInvoiceDetails.Gstamount,
                    Gstper = SupplierInvoiceDetails.Gstper,
                    CreatedBy = SupplierInvoiceDetails.CreatedBy,
                    CreatedOn = DateTime.Now,
                };
                response.code = (int)HttpStatusCode.OK;
                response.message = "Supplier Invoice Details Successfully Inserted";
                Context.SupplierInvoiceDetails.Add(SupplierInvoice);
                Context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

        public async Task<ApiResponseModel> DeleteSupplierInvoiceDetails(int InvoiceDetailsId)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var SupplierInvoice = Context.SupplierInvoiceDetails.Where(a => a.InvoiceDetailsId == InvoiceDetailsId).FirstOrDefault();
                if (SupplierInvoice != null)
                {
                    Context.SupplierInvoiceDetails.Remove(SupplierInvoice);
                    response.message = SupplierInvoice.RefInvoiceId + " Id of Supplier Invoice Details is Removed Successfully!";
                    response.code = 200;
                }
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public async Task<SupplierInvoiceDetailsModel> GetSupplierInvoiceDetailsById(int InvoiceDetailsId)
        {

            SupplierInvoiceDetailsModel supplierList = new SupplierInvoiceDetailsModel();
            try
            {
                supplierList = (from a in Context.SupplierInvoiceDetails.Where(x => x.InvoiceDetailsId == InvoiceDetailsId)
                                join b in Context.SupplierInvoices on a.RefInvoiceId equals b.Id
                                join c in Context.UnitMasters on a.UnitTypeId equals c.UnitId
                                select new SupplierInvoiceDetailsModel
                                {
                                    InvoiceDetailsId = InvoiceDetailsId,
                                    Item = a.Item,
                                    RefInvoiceId = a.RefInvoiceId,
                                    UnitTypeId = a.UnitTypeId,
                                    UnitTypeName = c.UnitName,
                                    Gstper = a.Gstper,
                                    Gstamount = a.Gst,
                                    Quantity = a.Quantity,
                                    Price = a.Price,
                                    Discount = a.DiscountAmount,
                                    DiscountPer = a.DiscountPer,


                                }).First();
                return supplierList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<SupplierInvoiceDetailsModel>> GetSupplierInvoiceDetailsList(string? searchText, string? searchBy, string? sortBy)
        {
            try
            {
                var supplierList = (from a in Context.SupplierInvoiceDetails
                                    join b in Context.SupplierInvoices on a.RefInvoiceId equals b.Id
                                    join c in Context.UnitMasters on a.UnitTypeId equals c.UnitId
                                    select new SupplierInvoiceDetailsModel
                                    {
                                        InvoiceDetailsId = a.InvoiceDetailsId,
                                        Item = a.Item,
                                        RefInvoiceId = a.RefInvoiceId,
                                        UnitTypeId = a.UnitTypeId,
                                        UnitTypeName = c.UnitName,
                                        Gstper = a.Gstper,
                                        Gstamount = a.Gst,
                                        Quantity = a.Quantity,
                                        Price = a.Price,
                                        Discount = a.DiscountAmount,
                                        DiscountPer = a.DiscountPer,
                                        CreatedOn = a.CreatedOn,
                                    });

                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    supplierList = supplierList.Where(u =>
                        u.UnitTypeName.ToLower().Contains(searchText)
                    );
                }

                if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
                {
                    searchText = searchText.ToLower();
                    switch (searchBy.ToLower())
                    {
                        case "unittypename":
                            supplierList = supplierList.Where(u => u.UnitTypeName.ToLower().Contains(searchText));
                            break;
                        default:

                            break;
                    }
                }

                if (string.IsNullOrEmpty(sortBy))
                {
                    // Default sorting by CreatedOn in descending order
                    supplierList = supplierList.OrderByDescending(u => u.CreatedOn);
                }
                else
                {
                    string sortOrder = sortBy.StartsWith("Ascending") ? "ascending" : "descending";
                    string field = sortBy.Substring(sortOrder.Length); // Remove the "Ascending" or "Descending" part

                    switch (field.ToLower())
                    {
                        case "refinvoiceid":
                            if (sortOrder == "ascending")
                                supplierList = supplierList.OrderBy(u => u.RefInvoiceId);
                            else if (sortOrder == "descending")
                                supplierList = supplierList.OrderByDescending(u => u.RefInvoiceId);
                            break;
                        case "createdon":
                            if (sortOrder == "ascending")
                                supplierList = supplierList.OrderBy(u => u.CreatedOn);
                            else if (sortOrder == "descending")
                                supplierList = supplierList.OrderByDescending(u => u.CreatedOn);
                            break;
                        default:

                            break;
                    }
                }

                return supplierList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ApiResponseModel> UpdateSupplierInvoiceDetails(SupplierInvoiceDetailsModel SupplierInvoiceDetails)
        {
            ApiResponseModel model = new ApiResponseModel();
            var supplierInvoiceDetail = Context.SupplierInvoiceDetails.Where(e => e.InvoiceDetailsId == SupplierInvoiceDetails.InvoiceDetailsId).FirstOrDefault();
            try
            {
                if (supplierInvoiceDetail != null)
                {
                    supplierInvoiceDetail.InvoiceDetailsId = SupplierInvoiceDetails.InvoiceDetailsId;
                    supplierInvoiceDetail.RefInvoiceId = SupplierInvoiceDetails.RefInvoiceId;
                    supplierInvoiceDetail.Item = SupplierInvoiceDetails.Item;
                    supplierInvoiceDetail.UnitTypeId = SupplierInvoiceDetails.UnitTypeId;
                    supplierInvoiceDetail.Quantity = SupplierInvoiceDetails.Quantity;
                    supplierInvoiceDetail.Price = SupplierInvoiceDetails.Price;
                    supplierInvoiceDetail.Gst = SupplierInvoiceDetails.Gstamount;
                    supplierInvoiceDetail.Gstper = SupplierInvoiceDetails.Gstper;
                }
                Context.SupplierInvoiceDetails.Update(supplierInvoiceDetail);
                Context.SaveChanges();
                model.code = 200;
                model.message = "Supplier Invoice Details Updated Successfully!";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return model;
        }
    }
}
