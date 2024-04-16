﻿using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.PurchaseRequest;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.Repository.Interface.Repository.PurchaseRequest;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Repository.PurchaseRequestRepository
{
    public class PurchaseRequestRepo : IPurchaseRequest
    {
        public PurchaseRequestRepo(DbaccManegmentContext context)
        {
            Context = context;
        }

        public DbaccManegmentContext Context { get; }

        public string CheckPRNo()
        {
            try
            {
                var LastPr = Context.PurchaseRequests.OrderByDescending(e => e.CreatedOn).FirstOrDefault();
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

                string PurchaseRequestId;
                if (LastPr == null)
                {

                    PurchaseRequestId = $"DMInfra/PR/{(lastYear % 100).ToString("D2")}-{(currentYear % 100).ToString("D2")}/001";
                }
                else
                {
                    if (LastPr.PrNo.Length >= 19)
                    {

                        int PrNumber = int.Parse(LastPr.PrNo.Substring(18)) + 1;
                        PurchaseRequestId = $"DMInfra/PR/{(lastYear % 100).ToString("D2")}-{(currentYear % 100).ToString("D2")}/" + PrNumber.ToString("D3");
                    }
                    else
                    {
                        throw new Exception("PurchaseRequest Id does not have the expected format.");
                    }
                }
                return PurchaseRequestId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<ApiResponseModel> AddPurchaseRequestDetails(PurchaseRequestModel PurchaseRequestDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var purchaseRequest = new PurchaseRequest()
                {
                    Pid = Guid.NewGuid(),
                    PrNo = PurchaseRequestDetails.PrNo,
                    Item = PurchaseRequestDetails.Item,
                    ItemId = PurchaseRequestDetails.ItemId,
                    UnitTypeId = PurchaseRequestDetails.UnitTypeId,
                    Quantity = PurchaseRequestDetails.Quantity,
                    SiteId = PurchaseRequestDetails.SiteId,
                    IsApproved = PurchaseRequestDetails.IsApproved,
                    CreatedBy = PurchaseRequestDetails.CreatedBy,
                    CreatedOn = DateTime.Now,
                };
                response.code = (int)HttpStatusCode.OK;
                response.message = "PurchaseRequest successfully created..!";
                Context.PurchaseRequests.Add(purchaseRequest);
                Context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

        public async Task<ApiResponseModel> DeletePurchaseRequest(Guid PurchaseId)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var purchaseDetails = Context.PurchaseRequests.Where(a => a.Pid == PurchaseId).FirstOrDefault();
                if (PurchaseId != null)
                {
                    Context.PurchaseRequests.Remove(purchaseDetails);
                    response.message = "PurchaseRequest" + " " + purchaseDetails.Item + " " + " item is successfully removed..!";
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

        public async Task<PurchaseRequestModel> GetPurchaseRequestDetailsById(Guid PurchaseId)
        {
            PurchaseRequestModel purchaseRequestList = new PurchaseRequestModel();
            try
            {
                purchaseRequestList = (from a in Context.PurchaseRequests.Where(x => x.Pid == PurchaseId)
                                       join b in Context.UnitMasters on a.UnitTypeId equals b.UnitId
                                       join c in Context.Sites on a.SiteId equals c.SiteId
                                       select new PurchaseRequestModel
                                       {
                                           Pid = a.Pid,
                                           PrNo = a.PrNo,
                                           Item = a.Item,
                                           ItemId = a.ItemId,
                                           UnitTypeId = a.UnitTypeId,
                                           Quantity = a.Quantity,
                                           UnitName = b.UnitName,
                                           IsApproved = a.IsApproved,
                                           SiteId = a.SiteId,
                                           SiteName = c.SiteName,
                                           CreatedBy = a.CreatedBy,
                                           CreatedOn = a.CreatedOn,
                                       }).First();
                return purchaseRequestList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<PurchaseRequestModel>> GetPurchaseRequestList(string? searchText, string? searchBy, string? sortBy, Guid? siteId)
        {
            try
            {
                var PurchaseRequestList = from a in Context.PurchaseRequests
                                          join b in Context.UnitMasters on a.UnitTypeId equals b.UnitId
                                          join c in Context.Sites on a.SiteId equals c.SiteId
                                          where siteId == null || a.SiteId == siteId
                                          select new PurchaseRequestModel
                                          {
                                              Pid = a.Pid,
                                              Item = a.Item,
                                              ItemId = a.ItemId,
                                              PrNo = a.PrNo,
                                              Quantity = a.Quantity,
                                              UnitTypeId = a.UnitTypeId,
                                              UnitName = b.UnitName,
                                              SiteId = a.SiteId,
                                              SiteName = c.SiteName,
                                              CreatedBy = a.CreatedBy,
                                              CreatedOn = a.CreatedOn,
                                              IsApproved = a.IsApproved
                                          };

                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    PurchaseRequestList = PurchaseRequestList.Where(u =>
                        u.Item.ToLower().Contains(searchText) ||
                        u.UnitName.ToLower().Contains(searchText) ||
                        u.Quantity.ToString().Contains(searchText)
                    );
                }

                if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
                {
                    searchText = searchText.ToLower();
                    switch (searchBy.ToLower())
                    {
                        case "unitname":
                            PurchaseRequestList = PurchaseRequestList.Where(u => u.UnitName.ToLower().Contains(searchText));
                            break;
                        case "itemname":
                            PurchaseRequestList = PurchaseRequestList.Where(u => u.Item.ToLower().Contains(searchText));
                            break;
                        default:

                            break;
                    }
                }

                if (string.IsNullOrEmpty(sortBy))
                {

                    PurchaseRequestList = PurchaseRequestList.OrderByDescending(u => u.CreatedOn);
                }
                else
                {
                    string sortOrder = sortBy.StartsWith("Ascending") ? "ascending" : "descending";
                    string field = sortBy.Substring(sortOrder.Length); // Remove the "Ascending" or "Descending" part

                    switch (field.ToLower())
                    {
                        case "item":
                            PurchaseRequestList = sortOrder == "ascending" ? PurchaseRequestList.OrderBy(u => u.Item) : PurchaseRequestList.OrderByDescending(u => u.Item);
                            break;
                        case "createdon":
                            PurchaseRequestList = sortOrder == "ascending" ? PurchaseRequestList.OrderBy(u => u.CreatedOn) : PurchaseRequestList.OrderByDescending(u => u.CreatedOn);
                            break;
                        default:

                            break;
                    }
                }

                return await PurchaseRequestList.ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<ApiResponseModel> UpdatePurchaseRequestDetails(PurchaseRequestModel PurchaseRequestDetails)
        {

            ApiResponseModel model = new ApiResponseModel();
            var PurchaseRequestData = Context.PurchaseRequests.Where(e => e.Pid == PurchaseRequestDetails.Pid).FirstOrDefault();
            try
            {
                if (PurchaseRequestData != null)
                {
                    PurchaseRequestData.Pid = PurchaseRequestDetails.Pid;
                    PurchaseRequestData.PrNo = PurchaseRequestDetails.PrNo;
                    PurchaseRequestData.Item = PurchaseRequestDetails.Item;
                    PurchaseRequestData.ItemId = PurchaseRequestDetails.ItemId;
                    PurchaseRequestData.Quantity = PurchaseRequestDetails.Quantity;
                    PurchaseRequestData.UnitTypeId = PurchaseRequestDetails.UnitTypeId;
                    PurchaseRequestData.SiteId = PurchaseRequestDetails.SiteId;

                }
                Context.PurchaseRequests.Update(PurchaseRequestData);
                Context.SaveChanges();
                model.code = 200;
                model.message = "PurchaseRequest details successfully updated..!";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return model;
        }
        public async Task<ApiResponseModel> PurchaseRequestIsApproved(Guid PurchaseId)
        {

            ApiResponseModel response = new ApiResponseModel();
            var purchaseRequestData = Context.PurchaseRequests.Where(a => a.Pid == PurchaseId).FirstOrDefault();

            if (purchaseRequestData != null)
            {

                if (purchaseRequestData.IsApproved == true)
                {
                    purchaseRequestData.IsApproved = false;
                    Context.PurchaseRequests.Update(purchaseRequestData);
                    Context.SaveChanges();
                    response.code = 200;
                    response.data = purchaseRequestData;
                    response.message = "PurchaseRequest is successfully unapproved!";
                }

                else
                {
                    purchaseRequestData.IsApproved = true;
                    Context.PurchaseRequests.Update(purchaseRequestData);
                    Context.SaveChanges();
                    response.code = 200;
                    response.data = purchaseRequestData;
                    response.message = "PurchaseRequest is successfully approved..!";
                }
            }
            return response;
        }
    }
}
