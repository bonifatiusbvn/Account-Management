using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.Repository.Interface.Repository.PurchaseOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Repository.PurchaseOrderRepository
{
    public class PurchaseOrderRepo : IPurchaseOrder
    {
        public PurchaseOrderRepo(DbaccManegmentContext context)
        {
            Context = context;
        }
        public DbaccManegmentContext Context { get; }

        public async Task<ApiResponseModel> AddPurchaseOrderDetails(PurchaseOrderView PurchaseOrderDetails)
        {
            ApiResponseModel responseModel = new ApiResponseModel();
            try
            {
                var PurchaseOrder = new PurchaseOrder()
                {
                    Id = Guid.NewGuid(),
                    SiteId=PurchaseOrderDetails.SiteId,
                    FromSupplierId = PurchaseOrderDetails.FromSupplierId,
                    ToCompanyId = PurchaseOrderDetails.ToCompanyId,
                    TotalAmount = PurchaseOrderDetails.TotalAmount,
                    Description = PurchaseOrderDetails.Description,
                    DeliveryShedule = PurchaseOrderDetails.DeliveryShedule,
                    TotalPrice = PurchaseOrderDetails.TotalPrice,
                    TotalDiscount = PurchaseOrderDetails.TotalDiscount,
                    TotalGstamount = PurchaseOrderDetails.TotalGstamount,
                    BillingAddress = PurchaseOrderDetails.BillingAddress,
                    CreatedBy = PurchaseOrderDetails.CreatedBy,
                    CreatedOn = DateTime.Now,
                };
                responseModel.code=(int)HttpStatusCode.OK;
                responseModel.message = "Purchase Order Inserted Successfully";
                Context.PurchaseOrders.Add(PurchaseOrder);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return responseModel;
        }

        public string CheckPONo()
        {
            try
            {
                var LastPO = Context.PurchaseOrders.OrderByDescending(e => e.CreatedOn).FirstOrDefault();
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

                string PurchaseOrderId;
                if (LastPO == null)
                {

                    PurchaseOrderId = $"DMInfra/PO/{(lastYear % 100).ToString("D2")}-{(currentYear % 100).ToString("D2")}/001";
                }
                else
                {
                    if (LastPO.Poid.Length >= 19)
                    {

                        int PrNumber = int.Parse(LastPO.Poid.Substring(18)) + 1;
                        PurchaseOrderId = $"DMInfra/PO/{(lastYear % 100).ToString("D2")}-{(currentYear % 100).ToString("D2")}/" + PrNumber.ToString("D3");
                    }
                    else
                    {
                        throw new Exception("Purchase Order Id does not have the expected format.");
                    }
                }
                return PurchaseOrderId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<ApiResponseModel> DeletePurchaseOrderDetails(Guid POId)
        {
            throw new NotImplementedException();
        }

        public async Task<PurchaseOrderView> GetPurchaseOrderDetailsById(Guid POId)
        {
            PurchaseOrderView PurchaseOrder = new PurchaseOrderView();
            try
            {
                PurchaseOrder=(from a in Context.PurchaseOrders.Where(x=>x.Id==POId)
                               join b in Context.SupplierMasters on a.FromSupplierId equals b.SupplierId
                               join c in Context.Companies on a.ToCompanyId equals c.CompanyId
                               join d in Context.Sites on a.SiteId equals d.SiteId
                               select new PurchaseOrderView
                               {
                                   Id = a.Id,
                                   SiteId = a.SiteId,
                                   SiteName=d.SiteName,
                                   FromSupplierId=a.FromSupplierId,
                                   SupplierName=b.SupplierName,
                                   ToCompanyId=a.ToCompanyId,
                                   CompanyName=c.CompanyName,
                                   TotalAmount=a.TotalAmount,
                                   Description = a.Description,
                                   DeliveryShedule = a.DeliveryShedule,
                                   TotalPrice = a.TotalPrice,
                                   TotalDiscount = a.TotalDiscount,
                                   TotalGstamount = a.TotalGstamount,
                                   BillingAddress = a.BillingAddress,
                                   CreatedBy = a.CreatedBy,
                                   CreatedOn = a.CreatedOn,
                               }).First();
                return PurchaseOrder;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<PurchaseOrderView>> GetPurchaseOrderList()
        {
            try
            {
                var PurchaseOrder = (from a in Context.PurchaseOrders
                                     join b in Context.SupplierMasters on a.FromSupplierId equals b.SupplierId
                                     join c in Context.Companies on a.ToCompanyId equals c.CompanyId
                                     join d in Context.Sites on a.SiteId equals d.SiteId
                                     select new PurchaseOrderView
                                     {
                                         Id = a.Id,
                                         SiteId = a.SiteId,
                                         SiteName=d.SiteName,
                                         FromSupplierId = a.FromSupplierId,
                                         SupplierName = b.SupplierName,
                                         ToCompanyId = a.ToCompanyId,
                                         CompanyName=c.CompanyName,
                                         TotalAmount = a.TotalAmount,
                                         Description = a.Description,
                                         DeliveryShedule = a.DeliveryShedule,
                                         TotalPrice = a.TotalPrice,
                                         TotalDiscount = a.TotalDiscount,
                                         TotalGstamount = a.TotalGstamount,
                                         BillingAddress = a.BillingAddress,
                                         CreatedBy = a.CreatedBy,
                                         CreatedOn = a.CreatedOn,
                                     });
                return PurchaseOrder;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApiResponseModel> InsertMultiplePurchaseOrderDetails(List<PurchaseOrderMasterView> PurchaseOrderDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                foreach (var item in PurchaseOrderDetails)
                {
                    var PurchaseOrder = new PurchaseOrder()
                    {

                        Id = Guid.NewGuid(),
                        SiteId = item.SiteId,
                        FromSupplierId = item.FromSupplierId,
                        ToCompanyId = item.ToCompanyId,
                        TotalAmount = item.TotalAmount,
                        Description = item.Description,
                        DeliveryShedule = item.DeliveryShedule,
                        TotalPrice = item.TotalPrice,
                        TotalDiscount = item.TotalDiscount,
                        TotalGstamount = item.TotalGstamount,
                        BillingAddress = item.BillingAddress,
                        CreatedBy = item.CreatedBy,
                        CreatedOn = DateTime.Now,
                    };

                    var PurchaseOrderDetail = new PurchaseOrderDetail()
                    {
                        PorefId = item.Id,
                        Item = item.Item,
                        UnitTypeId = item.UnitTypeId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Discount = item.Discount,
                        Gst = item.Gst,
                        Gstamount = item.Gstamount,
                        CreatedBy = item.CreatedBy,
                        CreatedOn = DateTime.Now,
                    };
                    var PurchaseAddress = new PodeliveryAddress()
                    {
                        Poid = item.Id,
                        Address = item.BillingAddress,
                    };
                    Context.PurchaseOrders.Add(PurchaseOrder);
                    Context.PurchaseOrderDetails.Add(PurchaseOrderDetail);
                }

                await Context.SaveChangesAsync();
                response.code = (int)HttpStatusCode.OK;
                response.message = "Purchase Order Inserted Successfully";
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.message = "Error creating orders: " + ex.Message;
            }
            return response;
        }
        public async Task<ApiResponseModel> UpdatePurchaseOrderDetails(PurchaseOrderView PurchaseOrderDetails)
        {
            ApiResponseModel responseModel = new ApiResponseModel();
            var PurchaseOrder=Context.PurchaseOrders.Where(e=>e.Id==PurchaseOrderDetails.Id).FirstOrDefault();
            try
            {
                if(PurchaseOrder!=null)
                {
                    PurchaseOrder.Id = PurchaseOrderDetails.Id;
                    PurchaseOrder.SiteId = PurchaseOrderDetails.SiteId;
                    PurchaseOrder.FromSupplierId = PurchaseOrderDetails.FromSupplierId;
                    PurchaseOrder.ToCompanyId = PurchaseOrderDetails.ToCompanyId;
                    PurchaseOrder.TotalAmount = PurchaseOrderDetails.TotalAmount;
                    PurchaseOrder.Description = PurchaseOrderDetails.Description;
                    PurchaseOrder.DeliveryShedule = PurchaseOrderDetails.DeliveryShedule;
                    PurchaseOrder.TotalPrice = PurchaseOrderDetails.TotalPrice;
                    PurchaseOrder.TotalDiscount = PurchaseOrderDetails.TotalDiscount;
                    PurchaseOrder.TotalGstamount = PurchaseOrderDetails.TotalGstamount;
                    PurchaseOrder.BillingAddress = PurchaseOrderDetails.BillingAddress;
                    PurchaseOrder.CreatedBy = PurchaseOrderDetails.CreatedBy;
                    PurchaseOrder.CreatedOn = PurchaseOrderDetails.CreatedOn;
                };
                responseModel.code = (int)HttpStatusCode.OK;
                responseModel.message = "Purchase Order Updated Successfully";
                Context.PurchaseOrders.Update(PurchaseOrder);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return responseModel;
        }
    }
}
