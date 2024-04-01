using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.Repository.Interface.Repository.InvoiceMaster;
using AccountManagement.Repository.Interface.Repository.PurchaseOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Repository.InvoiceMasterRepository
{
    public class SupplierInvoiceRepo : ISupplierInvoice
    {
        public SupplierInvoiceRepo(DbaccManegmentContext context)
        {
            Context = context;
        }

        public DbaccManegmentContext Context { get; }

        public async Task<ApiResponseModel> AddSupplierInvoice(SupplierInvoiceModel SupplierInvoiceDetail)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var SupplierInvoice = new SupplierInvoice()
                {
                    InvoiceId = Guid.NewGuid(),
                    SiteId = SupplierInvoiceDetail.SiteId,
                    FromSupplierId = SupplierInvoiceDetail.FromSupplierId,
                    ToCompanyId = SupplierInvoiceDetail.ToCompanyId,
                    TotalAmount = SupplierInvoiceDetail.TotalAmount,
                    Description = SupplierInvoiceDetail.Description,
                    DeliveryShedule = SupplierInvoiceDetail.DeliveryShedule,
                    TotalPrice = SupplierInvoiceDetail.TotalPrice,
                    TotalDiscount = SupplierInvoiceDetail.TotalDiscount,
                    TotalGstamount = SupplierInvoiceDetail.TotalGstamount,
                    Roundoff = SupplierInvoiceDetail.Roundoff,
                    CreatedBy = SupplierInvoiceDetail.CreatedBy,
                    CreatedOn = DateTime.Now,
                };
                response.code = (int)HttpStatusCode.OK;
                response.message = "Supplier Invoice Successfully Inserted";
                Context.SupplierInvoices.Add(SupplierInvoice);
                Context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

        public async Task<ApiResponseModel> DeleteSupplierInvoice(Guid InvoiceId)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var SupplierInvoice = Context.SupplierInvoices.Where(a => a.InvoiceId == InvoiceId).FirstOrDefault();
                if (SupplierInvoice != null)
                {
                    Context.SupplierInvoices.Remove(SupplierInvoice);
                    response.message = "SupplierInvoice" + " " + SupplierInvoice.InvoiceId + "is Removed Successfully!";
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

        public async Task<SupplierInvoiceModel> GetSupplierInvoiceById(Guid InvoiceId)
        {

            SupplierInvoiceModel supplierList = new SupplierInvoiceModel();
            try
            {
                supplierList = (from a in Context.SupplierInvoices.Where(x => x.InvoiceId == InvoiceId)
                            join b in Context.SupplierMasters on a.FromSupplierId equals b.SupplierId 
                            join c in Context.Companies on a.ToCompanyId equals c.CompanyId
                            join d in Context.Sites on a.SiteId equals d.SiteId
                            select new SupplierInvoiceModel
                            {
                                InvoiceId = a.InvoiceId,
                                SiteId = a.SiteId,
                                FromSupplierId = a.FromSupplierId,
                                TotalAmount = a.TotalAmount,
                                TotalDiscount = a.TotalDiscount,
                                TotalGstamount = a.TotalGstamount,
                                TotalPrice = a.TotalPrice,
                                Description = a.Description,
                                DeliveryShedule = a.DeliveryShedule,
                                Roundoff = a.Roundoff,
                                ToCompanyId = a.ToCompanyId,
                                ToCompanyName = c.CompanyName,
                                SiteName = d.SiteName,
                                FromSupplierName = b.SupplierName
                            }).First();
                return supplierList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<SupplierInvoiceModel>> GetSupplierInvoiceList(string? searchText, string? searchBy, string? sortBy)
        {
            try
            {
                var supplierList = (from a in Context.SupplierInvoices
                                    join b in Context.SupplierMasters on a.FromSupplierId equals b.SupplierId
                                    join c in Context.Companies on a.ToCompanyId equals c.CompanyId
                                    join d in Context.Sites on a.SiteId equals d.SiteId
                                    select new SupplierInvoiceModel
                                    {
                                        InvoiceId = a.InvoiceId,
                                        SiteId = a.SiteId,
                                        FromSupplierId = a.FromSupplierId,
                                        TotalAmount = a.TotalAmount,
                                        TotalDiscount = a.TotalDiscount,
                                        TotalGstamount = a.TotalGstamount,
                                        TotalPrice = a.TotalPrice,
                                        Description = a.Description,
                                        DeliveryShedule = a.DeliveryShedule,
                                        Roundoff = a.Roundoff,
                                        ToCompanyId = a.ToCompanyId,
                                        ToCompanyName = c.CompanyName,
                                        SiteName = d.SiteName,
                                        FromSupplierName = b.SupplierName,
                                        CreatedOn = a.CreatedOn,
                                    });

                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    supplierList = supplierList.Where(u =>
                        u.SiteName.ToLower().Contains(searchText) ||
                        u.ToCompanyName.ToLower().Contains(searchText)
                    );
                }

                if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
                {
                    searchText = searchText.ToLower();
                    switch (searchBy.ToLower())
                    {
                        case "sitename":
                            supplierList = supplierList.Where(u => u.SiteName.ToLower().Contains(searchText));
                            break;
                        case "companyname":
                            supplierList = supplierList.Where(u => u.ToCompanyName.ToLower().Contains(searchText));
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
                        case "sitename":
                            if (sortOrder == "ascending")
                                supplierList = supplierList.OrderBy(u => u.SiteName);
                            else if (sortOrder == "descending")
                                supplierList = supplierList.OrderByDescending(u => u.SiteName);
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

        public async Task<ApiResponseModel> InsertMultipleSupplierItemDetails(List<SupplierInvoiceMasterView> SupplierItemDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                // Generate a single InvoiceId for all items
                var invoiceId = Guid.NewGuid();

                foreach (var item in SupplierItemDetails)
                {
                    var supplierInvoice = new SupplierInvoice()
                    {
                        InvoiceId = invoiceId, // Use the same InvoiceId for all items
                        SiteId = item.SiteId,
                        FromSupplierId = item.FromSupplierId,
                        ToCompanyId = item.ToCompanyId,
                        TotalAmount = item.TotalAmount,
                        Description = item.Description,
                        DeliveryShedule = item.DeliveryShedule,
                        TotalPrice = item.TotalPrice,
                        TotalDiscount = item.TotalDiscount,
                        TotalGstamount = item.TotalGstamount,
                        Roundoff = item.Roundoff,
                        CreatedBy = item.CreatedBy,
                        CreatedOn = DateTime.Now,
                    };
                    Context.SupplierInvoices.Add(supplierInvoice);

                    var supplierInvoiceDetail = new SupplierInvoiceDetail()
                    {
                        RefInvoiceId = invoiceId, // Use the same InvoiceId for all items
                        Item = item.Item,
                        UnitTypeId = item.UnitTypeId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Discount = item.Discount,
                        Gst = item.Gst,
                        Gstamount = item.Gstamount,
                        PaymentStatus = item.PaymentStatus,
                        CreatedBy = item.CreatedBy,
                        CreatedOn = DateTime.Now,
                    };
                    Context.SupplierInvoiceDetails.Add(supplierInvoiceDetail);
                }

                await Context.SaveChangesAsync();
                response.code = (int)HttpStatusCode.OK;
                response.message = "Supplier Order Inserted Successfully";
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.message = "Error creating orders: " + ex.Message;
            }
            return response;
        }


        public async Task<ApiResponseModel> UpdateSupplierInvoice(SupplierInvoiceModel SupplierInvoiceDetail)
        {
            ApiResponseModel model = new ApiResponseModel();
            var supplierInvoice = Context.SupplierInvoices.Where(e => e.InvoiceId == SupplierInvoiceDetail.InvoiceId).FirstOrDefault();
            try
            {
                if (supplierInvoice != null)
                {
                    supplierInvoice.InvoiceId = SupplierInvoiceDetail.InvoiceId;
                    supplierInvoice.SiteId = SupplierInvoiceDetail.SiteId;
                    supplierInvoice.FromSupplierId = SupplierInvoiceDetail.FromSupplierId;
                    supplierInvoice.ToCompanyId = SupplierInvoiceDetail.ToCompanyId;
                    supplierInvoice.TotalAmount = SupplierInvoiceDetail.TotalAmount;
                    supplierInvoice.Description = SupplierInvoiceDetail.Description;
                    supplierInvoice.DeliveryShedule = SupplierInvoiceDetail.DeliveryShedule;
                    supplierInvoice.TotalPrice = SupplierInvoiceDetail.TotalPrice;
                    supplierInvoice.TotalDiscount = SupplierInvoiceDetail.TotalDiscount;
                    supplierInvoice.TotalGstamount = SupplierInvoiceDetail.TotalGstamount;
                    supplierInvoice.Roundoff = SupplierInvoiceDetail.Roundoff;
                }
                Context.SupplierInvoices.Update(supplierInvoice);
                Context.SaveChanges();
                model.code = 200;
                model.message = "Supplier Invoice Updated Successfully!";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return model;
        }
    }
}
