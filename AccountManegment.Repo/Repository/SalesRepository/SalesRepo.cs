using AccountManagement.API;
using AccountManagement.DBContext.Models.ViewModels.InvoiceMaster;
using AccountManagement.DBContext.Models.ViewModels.SalesMaster;
using AccountManagement.Repository.Interface.Repository.Sales;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Repository.SalesRepository
{
    public class SalesRepo : ISalesInvoice
    {
        public SalesRepo(DbaccManegmentContext context)
        {
            Context = context;
        }
        public DbaccManegmentContext Context { get; }

        public async Task<SalesInvoiceListView> GetSalesList(string searchText, string searchBy, string sortBy)
        {
            try
            {
                var supplierDataQuery = from a in Context.SalesInvoices
                                        join e in Context.SalesInvoiceDetails on a.Id equals e.RefSalesInvoiceId
                                        join b in Context.SupplierMasters on a.SupplierId equals b.SupplierId
                                        join c in Context.Companies on a.CompanyId equals c.CompanyId
                                        join d in Context.Sites on a.SiteId equals d.SiteId
                                        join f in Context.ItemMasters on e.ItemId equals f.ItemId

                                        select new
                                        {
                                            Invoice = new SalesInvoiceMasterModel
                                            {
                                                Id = a.Id,
                                                SalesInvoiceNo = a.SalesInvoiceNo,
                                                SiteId = a.SiteId,
                                                SupplierId = a.SupplierId,
                                                TotalAmount = a.TotalAmount,
                                                TotalDiscount = a.TotalDiscount,
                                                TotalGstamount = a.TotalGstamount,
                                                Description = a.Description,
                                                Tds = a.Tds,
                                                CompanyId = a.CompanyId,
                                                Date = a.Date,
                                                CompanyName = c.CompanyName,
                                                SiteName = d.SiteName,
                                                SupplierName = b.SupplierName,
                                                CreatedOn = a.CreatedOn,
                                                IsApproved = a.IsApproved,
                                                SupplierInvoiceNo = a.SupplierInvoiceNo,
                                            },
                                            Item = new SalesInvoiceDetailsModel
                                            {
                                                RefSalesInvoiceId = e.RefSalesInvoiceId,
                                                ItemId = e.ItemId,
                                                ItemName = f.ItemName
                                            }
                                        };

                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    supplierDataQuery = supplierDataQuery.Where(u =>
                        u.Invoice.SiteName.ToLower().Contains(searchText) ||
                        u.Invoice.CompanyName.ToLower().Contains(searchText) ||
                        u.Invoice.SupplierName.ToLower().Contains(searchText) ||
                        u.Invoice.SupplierInvoiceNo.ToLower().Contains(searchText) ||
                        u.Invoice.TotalAmount.ToString().Contains(searchText) ||
                        u.Item.ItemName.ToLower().Contains(searchText)
                    );
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    string sortOrder = sortBy.StartsWith("Ascending") ? "ascending" : "descending";
                    string field = sortBy.Substring(sortOrder.Length);

                    switch (field.ToLower())
                    {
                        case "date":
                            supplierDataQuery = sortOrder == "ascending"
                                ? supplierDataQuery.OrderBy(u => u.Invoice.Date)
                                : supplierDataQuery.OrderByDescending(u => u.Invoice.Date);
                            break;
                    }
                }
                else
                {
                    supplierDataQuery = supplierDataQuery.OrderByDescending(u => u.Invoice.Date);
                }

                var supplierData = await supplierDataQuery.ToListAsync();

                var groupedByRefInvoiceId = supplierData
                    .GroupBy(x => x.Item.RefSalesInvoiceId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.Item).ToList()
                    );

                var invoicesGroupedById = supplierData
                    .GroupBy(x => x.Invoice.Id)
                    .Select(g => new
                    {
                        Invoice = g.First().Invoice,
                        Items = groupedByRefInvoiceId.ContainsKey(g.Key)
                            ? groupedByRefInvoiceId[g.Key]
                            : new List<SalesInvoiceDetailsModel>()
                    })
                    .ToList();

                return new SalesInvoiceListView
                {
                    SalesInvoiceList = invoicesGroupedById.Select(g => g.Invoice).ToList(),
                    SalesInvoiceItemList = invoicesGroupedById.ToDictionary(g => g.Invoice.Id, g => g.Items.AsEnumerable())
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
