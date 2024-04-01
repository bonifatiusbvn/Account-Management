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
                    Id = Guid.NewGuid(),
                    InvoiceId = SupplierInvoiceDetail.InvoiceId,
                    SiteId = SupplierInvoiceDetail.SiteId,
                    SupplierId = SupplierInvoiceDetail.SupplierId,
                    CompanyId = SupplierInvoiceDetail.CompanyId,
                    TotalAmount = SupplierInvoiceDetail.TotalAmount,
                    Description = SupplierInvoiceDetail.Description,
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

        public async Task<ApiResponseModel> DeleteSupplierInvoice(Guid Id)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var SupplierInvoice = Context.SupplierInvoices.Where(a => a.Id == Id).FirstOrDefault();
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

        public async Task<SupplierInvoiceModel> GetSupplierInvoiceById(Guid Id)
        {

            SupplierInvoiceModel supplierList = new SupplierInvoiceModel();
            try
            {
                supplierList = (from a in Context.SupplierInvoices.Where(x => x.Id == Id)
                                join b in Context.SupplierMasters on a.SupplierId equals b.SupplierId
                                join c in Context.Companies on a.CompanyId equals c.CompanyId
                                join d in Context.Sites on a.SiteId equals d.SiteId
                                select new SupplierInvoiceModel
                                {
                                    Id = a.Id,
                                    InvoiceId = a.InvoiceId,
                                    //SiteId = a.SiteId,
                                    SupplierId = a.SupplierId,
                                    TotalAmount = a.TotalAmount,
                                    TotalDiscount = a.TotalDiscount,
                                    TotalGstamount = a.TotalGstamount,
                                    Description = a.Description,
                                    Roundoff = a.Roundoff,
                                    CompanyId = a.CompanyId,
                                    CompanyName = c.CompanyName,
                                    SiteName = d.SiteName,
                                    SupplierName = b.SupplierName
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
                                    join b in Context.SupplierMasters on a.SupplierId equals b.SupplierId
                                    join c in Context.Companies on a.CompanyId equals c.CompanyId
                                    join d in Context.Sites on a.SiteId equals d.SiteId
                                    select new SupplierInvoiceModel
                                    {
                                        Id = a.Id,
                                        InvoiceId = a.InvoiceId,
                                        SiteId = a.SiteId,
                                        SupplierId = a.SupplierId,
                                        TotalAmount = a.TotalAmount,
                                        TotalDiscount = a.TotalDiscount,
                                        TotalGstamount = a.TotalGstamount,
                                        Description = a.Description,
                                        Roundoff = a.Roundoff,
                                        CompanyId = a.CompanyId,
                                        CompanyName = c.CompanyName,
                                        SiteName = d.SiteName,
                                        SupplierName = b.SupplierName,
                                        CreatedOn = a.CreatedOn,
                                    });

                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    supplierList = supplierList.Where(u =>
                        u.SiteName.ToLower().Contains(searchText) ||
                        u.CompanyName.ToLower().Contains(searchText)
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
                            supplierList = supplierList.Where(u => u.CompanyName.ToLower().Contains(searchText));
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

        public async Task<ApiResponseModel> UpdateSupplierInvoice(SupplierInvoiceModel SupplierInvoiceDetail)
        {
            ApiResponseModel model = new ApiResponseModel();
            var supplierInvoice = Context.SupplierInvoices.Where(e => e.Id == SupplierInvoiceDetail.Id).FirstOrDefault();
            try
            {
                if (supplierInvoice != null)
                {
                    supplierInvoice.Id = SupplierInvoiceDetail.Id;
                    supplierInvoice.SiteId = SupplierInvoiceDetail.SiteId;
                    supplierInvoice.SupplierId = SupplierInvoiceDetail.SupplierId;
                    supplierInvoice.CompanyId = SupplierInvoiceDetail.CompanyId;
                    supplierInvoice.TotalAmount = SupplierInvoiceDetail.TotalAmount;
                    supplierInvoice.Description = SupplierInvoiceDetail.Description;
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

        public async Task<ApiResponseModel> InsertMultipleSupplierItemDetails(List<SupplierInvoiceMasterView> SupplierItemDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {

                var firstOrderDetail = SupplierItemDetails.First();

                    var supplierInvoice = new SupplierInvoice()
                    {
                        Id = Guid.NewGuid(),
                        InvoiceId = firstOrderDetail.InvoiceId,
                        SiteId = firstOrderDetail.SiteId,
                        SupplierId = firstOrderDetail.SupplierId,
                        CompanyId = firstOrderDetail.CompanyId,
                        Description = firstOrderDetail.Description,
                        TotalDiscount = firstOrderDetail.TotalDiscount,
                        TotalGstamount = firstOrderDetail.TotalGstamount,
                        TotalAmount = firstOrderDetail.TotalAmount,
                        PaymentStatus = firstOrderDetail.PaymentStatus,
                        Roundoff = firstOrderDetail.Roundoff,
                        IsPayOut = false,
                        CreatedBy = firstOrderDetail.CreatedBy,
                        CreatedOn = DateTime.Now,
                    };
                    Context.SupplierInvoices.Add(supplierInvoice);

                foreach (var item in SupplierItemDetails)
                {
                    var supplierInvoiceDetail = new SupplierInvoiceDetail()
                    {
                        RefInvoiceId = supplierInvoice.Id, 
                        Item = item.Item,
                        UnitTypeId = item.UnitTypeId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        DiscountPer = item.DiscountPer,
                        Gst = item.Gst,
                        Gstper = item.Gstper,
                        TotalAmount = item.TotalAmount,
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

        public string CheckSupplierInvoiceNo()
        {
            try
            {
                var LastPO = Context.SupplierInvoices.OrderByDescending(e => e.CreatedOn).FirstOrDefault();
                var currentDate = DateTime.Now;

                int currentYear;
                int lastYear;
                if (currentDate.Month > 4)
                {

                    currentYear = currentDate.Year + 1;
                    lastYear = currentDate.Year;
                }
                else
                {

                    currentYear = currentDate.Year;
                    lastYear = currentDate.Year - 1;
                }

                string SupplierInvoiceId;
                if (LastPO == null)
                {

                    SupplierInvoiceId = $"DMInfra/Invoice/{(lastYear % 100).ToString("D2")}-{(currentYear % 100).ToString("D2")}/001";
                }
                else
                {
                    if (LastPO.InvoiceId.Length >= 25)
                    {

                        int PrNumber = int.Parse(LastPO.InvoiceId.Substring(24)) + 1;
                        SupplierInvoiceId = $"DMInfra/Invoice/{(lastYear % 100).ToString("D2")}-{(currentYear % 100).ToString("D2")}/" + PrNumber.ToString("D3");
                    }
                    else
                    {
                        throw new Exception("Supplier Invoice Id does not have the expected format.");
                    }
                }
                return SupplierInvoiceId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
