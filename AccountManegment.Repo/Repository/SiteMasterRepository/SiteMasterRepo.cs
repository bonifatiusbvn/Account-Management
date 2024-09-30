﻿using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Repository.PurchaseRequest;
using AccountManagement.Repository.Interface.Repository.SiteMaster;
using AccountManagement.Repository.Services.SiteMaster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace AccountManagement.Repository.Repository.SiteMasterRepository
{
    public class SiteMasterRepo : ISiteMaster
    {
        public SiteMasterRepo(DbaccManegmentContext context)
        {
            Context = context;
        }
        public DbaccManegmentContext Context { get; }

        public async Task<ApiResponseModel> AddSiteDetails(SiteMasterModel SiteDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var existingSite = Context.Sites.FirstOrDefault(x => x.SiteName == SiteDetails.SiteName);
                if (existingSite == null)
                {
                    var SiteMaster = new Site()
                    {
                        SiteId = Guid.NewGuid(),
                        SiteName = SiteDetails.SiteName,
                        IsActive = true,
                        ContectPersonName = SiteDetails.ContectPersonName,
                        ContectPersonPhoneNo = SiteDetails.ContectPersonPhoneNo,
                        Address = SiteDetails.Address,
                        Area = SiteDetails.Area,
                        CityId = SiteDetails.CityId,
                        StateId = SiteDetails.StateId,
                        Country = SiteDetails.Country,
                        Pincode = SiteDetails.Pincode,
                        ShippingAddress = SiteDetails.ShippingAddress,
                        ShippingArea = SiteDetails.ShippingArea,
                        ShippingCityId = SiteDetails.ShippingCityId,
                        ShippingStateId = SiteDetails.ShippingStateId,
                        ShippingCountry = SiteDetails.ShippingCountry,
                        ShippingPincode = SiteDetails.ShippingPincode,
                        IsDeleted = false,
                        CreatedBy = SiteDetails.CreatedBy,
                        CreatedOn = DateTime.Now,
                    };
                    Context.Sites.Add(SiteMaster);

                    if (SiteDetails.SiteShippingAddresses != null)
                    {
                        foreach (var item in SiteDetails.SiteShippingAddresses)
                        {
                            var shippingAddress = new SiteAddress()
                            {
                                SiteId = SiteMaster.SiteId,
                                Address = item.Address,
                                IsDeleted = false,
                            };
                            Context.SiteAddresses.Add(shippingAddress);
                        }
                    }
                    Context.SaveChanges();
                    response.code = (int)HttpStatusCode.OK;
                    response.message = "Site is successfully created.";
                }
                else
                {
                    response.code = 400;
                    response.message = "Site already exists.";
                }
            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

        public async Task<SiteMasterModel> GetSiteDetailsById(Guid SiteId)
        {
            SiteMasterModel SiteList = new SiteMasterModel();
            try
            {
                SiteList = (from a in Context.Sites.Where(x => x.SiteId == SiteId)
                            join b in Context.Cities on a.CityId equals b.CityId into CityJoin
                            from city in CityJoin.DefaultIfEmpty()
                            join sc in Context.Cities on a.ShippingCityId equals sc.CityId into ShippingCityJoin
                            from shippingCity in ShippingCityJoin.DefaultIfEmpty()
                            join c in Context.States on a.StateId equals c.StatesId
                            join d in Context.Countries on a.Country equals d.CountryId
                            join shippingState in Context.States on a.ShippingStateId equals shippingState.StatesId
                            join shippingCountry in Context.Countries on a.ShippingCountry equals shippingCountry.CountryId
                            select new SiteMasterModel
                            {
                                SiteId = a.SiteId,
                                SiteName = a.SiteName,
                                IsActive = a.IsActive,
                                ContectPersonName = a.ContectPersonName,
                                ContectPersonPhoneNo = a.ContectPersonPhoneNo,
                                Address = a.Address,
                                Area = a.Area,
                                CityId = a.CityId,
                                CityName = city.CityName,
                                StateId = a.StateId,
                                StateName = c.StatesName,
                                Country = a.Country,
                                CountryName = d.CountryName,
                                Pincode = a.Pincode,
                                ShippingAddress = a.ShippingAddress,
                                ShippingArea = a.ShippingArea,
                                ShippingCityId = a.ShippingCityId,
                                ShippingCityName = shippingCity.CityName,
                                ShippingStateId = a.ShippingStateId,
                                ShippingStateName = shippingState.StatesName,
                                ShippingCountry = a.ShippingCountry,
                                ShippingCountryName = shippingCountry.CountryName,
                                ShippingPincode = a.ShippingPincode,
                                CreatedBy = a.CreatedBy,
                                CreatedOn = a.CreatedOn,
                                StateCode = shippingState != null ? shippingState.StateCode : null,
                            }).First();



                List<SiteAddressModel> siteAddress = (from shippingAddress in Context.SiteAddresses.Where(a => a.SiteId == SiteId)
                                                      select new SiteAddressModel
                                                      {
                                                          Aid = shippingAddress.Aid,
                                                          SiteId = shippingAddress.SiteId,
                                                          Address = shippingAddress.Address,
                                                      }).ToList();

                SiteList.SiteShippingAddresses = siteAddress;

                return SiteList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<SiteMasterModel>> GetSiteList(string? searchText, string? searchBy, string? sortBy)
        {
            try
            {
                var SiteList = (from a in Context.Sites
                                join b in Context.Cities on a.CityId equals b.CityId
                                join c in Context.States on a.StateId equals c.StatesId
                                join d in Context.Countries on a.Country equals d.CountryId
                                join e in Context.Countries on a.ShippingCountry equals e.CountryId
                                join f in Context.States on a.ShippingStateId equals f.StatesId
                                join g in Context.Cities on a.ShippingCityId equals g.CityId
                                where a.IsDeleted == false
                                select new SiteMasterModel
                                {
                                    SiteId = a.SiteId,
                                    SiteName = a.SiteName,
                                    IsActive = a.IsActive,
                                    ContectPersonName = a.ContectPersonName,
                                    ContectPersonPhoneNo = a.ContectPersonPhoneNo,
                                    Address = a.Address,
                                    Area = a.Area,
                                    CityId = a.CityId,
                                    CityName = b.CityName,
                                    StateId = a.StateId,
                                    StateName = c.StatesName,
                                    Country = a.Country,
                                    CountryName = d.CountryName,
                                    Pincode = a.Pincode,
                                    ShippingAddress = a.ShippingAddress,
                                    ShippingArea = a.ShippingArea,
                                    ShippingCityId = a.ShippingCityId,
                                    ShippingCityName = g.CityName,
                                    ShippingStateId = a.ShippingStateId,
                                    ShippingStateName = f.StatesName,
                                    ShippingCountry = a.ShippingCountry,
                                    ShippingCountryName = e.CountryName,
                                    ShippingPincode = a.ShippingPincode,
                                    CreatedBy = a.CreatedBy,
                                    CreatedOn = a.CreatedOn,
                                });

                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    SiteList = SiteList.Where(u =>
                        u.SiteName.ToLower().Contains(searchText) ||
                        u.ContectPersonName.ToLower().Contains(searchText)
                    );
                }

                if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
                {
                    searchText = searchText.ToLower();
                    switch (searchBy.ToLower())
                    {
                        case "sitename":
                            SiteList = SiteList.Where(u => u.SiteName.ToLower().Contains(searchText));
                            break;
                        case "contactpersonname":
                            SiteList = SiteList.Where(u => u.ContectPersonName.ToLower().Contains(searchText));
                            break;
                        default:

                            break;
                    }
                }

                if (string.IsNullOrEmpty(sortBy))
                {
                    SiteList = SiteList.OrderByDescending(u => u.CreatedOn);
                }
                else
                {
                    string sortOrder = sortBy.StartsWith("Ascending") ? "ascending" : "descending";
                    string field = sortBy.Substring(sortOrder.Length);

                    switch (field.ToLower())
                    {
                        case "sitename":
                            if (sortOrder == "ascending")
                                SiteList = SiteList.OrderBy(u => u.SiteName);
                            else if (sortOrder == "descending")
                                SiteList = SiteList.OrderByDescending(u => u.SiteName);
                            break;
                        case "createdon":
                            if (sortOrder == "ascending")
                                SiteList = SiteList.OrderBy(u => u.CreatedOn);
                            else if (sortOrder == "descending")
                                SiteList = SiteList.OrderByDescending(u => u.CreatedOn);
                            break;
                        default:

                            break;
                    }
                }

                return SiteList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ApiResponseModel> UpdateSiteDetails(SiteMasterModel SiteDetails)
        {
            ApiResponseModel model = new ApiResponseModel();
            var SiteMaster = Context.Sites.Where(e => e.SiteId == SiteDetails.SiteId).FirstOrDefault();
            try
            {
                if (SiteMaster != null)
                {
                    SiteMaster.SiteName = SiteDetails.SiteName;
                    SiteMaster.ContectPersonName = SiteDetails.ContectPersonName;
                    SiteMaster.ContectPersonPhoneNo = SiteDetails.ContectPersonPhoneNo;
                    SiteMaster.Address = SiteDetails.Address;
                    SiteMaster.Area = SiteDetails.Area;
                    SiteMaster.CityId = SiteDetails.CityId;
                    SiteMaster.StateId = SiteDetails.StateId;
                    SiteMaster.Country = SiteDetails.Country;
                    SiteMaster.Pincode = SiteDetails.Pincode;
                    SiteMaster.ShippingAddress = SiteDetails.ShippingAddress;
                    SiteMaster.ShippingArea = SiteDetails.ShippingArea;
                    SiteMaster.ShippingCityId = SiteDetails.ShippingCityId;
                    SiteMaster.ShippingStateId = SiteDetails.ShippingStateId;
                    SiteMaster.ShippingCountry = SiteDetails.ShippingCountry;
                    SiteMaster.ShippingPincode = SiteDetails.ShippingPincode;
                }

                Context.Sites.Update(SiteMaster);

                if (SiteDetails.SiteShippingAddresses == null || SiteDetails.SiteShippingAddresses.Count == 0)
                {
                    var siteAddressesToRemove = Context.SiteAddresses.Where(a => a.SiteId == SiteDetails.SiteId).ToList();
                    Context.SiteAddresses.RemoveRange(siteAddressesToRemove);
                }
                else
                {
                    foreach (var item in SiteDetails.SiteShippingAddresses)
                    {
                        var existingSiteAddress = Context.SiteAddresses.FirstOrDefault(a => a.SiteId == SiteMaster.SiteId && a.Address == item.Address);
                        if (existingSiteAddress != null)
                        {
                            existingSiteAddress.Address = item.Address;
                            existingSiteAddress.SiteId = SiteMaster.SiteId;
                            Context.SiteAddresses.Update(existingSiteAddress);
                        }
                        else
                        {
                            var shippingAddress = new SiteAddress()
                            {
                                SiteId = SiteMaster.SiteId,
                                Address = item.Address,
                                IsDeleted = false,
                            };
                            Context.SiteAddresses.Add(shippingAddress);
                        }
                    }

                    var siteAddressIds = SiteDetails.SiteShippingAddresses.Select(a => a.Address).ToList();
                    var siteAddressesToRemove = Context.SiteAddresses.Where(a => a.SiteId == SiteDetails.SiteId && !siteAddressIds.Contains(a.Address)).ToList();
                    Context.SiteAddresses.RemoveRange(siteAddressesToRemove);
                }

                Context.SaveChanges();

                model.code = 200;
                model.message = "Site details successfully updated.";
            }
            catch (Exception ex)
            {
                model.code = 500;
                model.message = "An error occurred while updating site details: " + ex.Message;
            }
            return model;
        }

        public async Task<ApiResponseModel> ActiveDeactiveSite(Guid SiteId)
        {
            ApiResponseModel response = new ApiResponseModel();
            var Getsitedata = Context.Sites.Where(a => a.SiteId == SiteId).FirstOrDefault();
            var activeUsersCount = Context.Users.Count(e => e.SiteId == SiteId && e.IsActive == true);

            if (Getsitedata != null)
            {
                if (activeUsersCount == 0)
                {
                    if (Getsitedata.IsActive)
                    {
                        Getsitedata.IsActive = false;
                        Context.Sites.Update(Getsitedata);
                        await Context.SaveChangesAsync();
                        response.code = 200;
                        response.data = Getsitedata;
                        response.message = "Site " + Getsitedata.SiteName + " is successfully deactivated.";
                    }
                    else
                    {
                        Getsitedata.IsActive = true;
                        Context.Sites.Update(Getsitedata);
                        await Context.SaveChangesAsync();
                        response.code = 200;
                        response.data = Getsitedata;
                        response.message = "Site " + Getsitedata.SiteName + " is successfully activated.";
                    }
                }
                else
                {
                    response.message = "This site has active users so it can't deactive.";
                    response.code = 400;
                }
            }
            return response;
        }


        public async Task<ApiResponseModel> DeleteSite(Guid SiteId)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var siteDetails = Context.Sites.Where(a => a.SiteId == SiteId).FirstOrDefault();
                var activeUsersCount = Context.Users.Count(e => e.SiteId == SiteId && e.IsActive == true && e.IsDeleted == false);
                var Purchaserequest = Context.PurchaseRequests.Where(b => b.SiteId == SiteId).ToList();
                var inwardchallan = Context.ItemInwords.Where(c => c.SiteId == SiteId).ToList();

                if (siteDetails != null && siteDetails.IsActive == false)
                {
                    if (activeUsersCount == 0)
                    {
                        siteDetails.IsDeleted = true;
                        Context.Sites.Update(siteDetails);
                        foreach (var request in Purchaserequest)
                        {
                            request.IsDeleted = true;
                            Context.PurchaseRequests.Update(request);
                        }

                        foreach (var challan in inwardchallan)
                        {
                            challan.IsDeleted = true;
                            Context.ItemInwords.Update(challan);
                        }
                        Context.SaveChanges();
                        response.code = 200;
                        response.data = siteDetails;
                        response.message = "Site is deleted successfully";
                    }
                    else
                    {
                        response.message = "This site has active users so it can't delete.";
                        response.code = 400;
                    }

                }
                else
                {
                    response.message = "Active site can't delete.";
                    response.code = 400;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<SiteMasterModel>> GetSiteNameList()
        {
            try
            {
                IEnumerable<SiteMasterModel> SiteName = Context.Sites.Where(e => e.IsActive == true).ToList().Select(a => new SiteMasterModel
                {
                    SiteId = a.SiteId,
                    SiteName = a.SiteName,
                });
                return SiteName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<SiteAddressModel>> GetSiteAddressList(Guid SiteId)
        {
            try
            {
                var data = await (from a in Context.SiteAddresses
                                  join b in Context.Sites on a.SiteId equals b.SiteId
                                  join c in Context.States on b.StateId equals c.StatesId
                                  where (a.SiteId == SiteId && a.IsDeleted != true)
                                  select new SiteAddressModel
                                  {
                                      Aid = a.Aid,
                                      SiteId = a.SiteId,
                                      Address = a.Address,
                                      IsDeleted = a.IsDeleted,
                                      StateCode = c.StateCode,
                                  }).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ApiResponseModel> AddSiteGroupDetails(SiteGroupModel GroupDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                bool isGroupAlreadyExists = Context.GroupMasters.Any(x => x.GroupName == GroupDetails.GroupName);
                {
                    if (isGroupAlreadyExists == true)
                    {
                        response.message = "This group already exists.";
                        response.code = (int)HttpStatusCode.NotFound;
                    }
                    else
                    {
                        var groupModel = new GroupMaster()
                        {
                            GroupId = Guid.NewGuid(),
                            GroupName = GroupDetails.GroupName,
                            SiteId = GroupDetails.SiteId,
                            GroupAddress = GroupDetails.GroupAddress
                        };
                        Context.GroupMasters.Add(groupModel);
                        await Context.SaveChangesAsync();
                        response.code = 200;
                        response.message = "Group is added successfully!";

                    }
                }
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<IEnumerable<GroupMasterModel>> GetGroupNameListBySiteId(Guid SiteId)
        {
            try
            {
                IEnumerable<GroupMasterModel> GroupList = Context.GroupMasters.Where(e => e.SiteId == SiteId).ToList().Select(a => new GroupMasterModel
                {
                    Id = a.Id,
                    GroupName = a.GroupName,
                    GroupId = a.GroupId,
                });
                return GroupList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<SiteGroupModel>> GetGroupNameList()
        {
            try
            {
                var SiteGroupList = (from a in Context.GroupMasters
                                     group new { a } by new { a.GroupName, a.GroupId } into g
                                     select new SiteGroupModel
                                     {
                                         GroupName = g.Key.GroupName,
                                         GroupId = g.Key.GroupId,
                                     });
                return SiteGroupList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ApiResponseModel> DeleteSiteGroupDetails(Guid GroupId)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var grouplist = Context.GroupMasters.Where(x => x.GroupId == GroupId).FirstOrDefault();
                if (grouplist != null)
                {
                    Context.GroupMasters.Remove(grouplist);
                    await Context.SaveChangesAsync();
                    response.code = 200;
                    response.message = "Site group is deleted successfully.";
                }
                else
                {
                    response.code = 400;
                    response.message = "Site group is not found.";
                }
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.message = "There is some error in deleting site group";
            }
            return response;
        }

        public async Task<SiteGroupModel> GetGroupDetailsByGroupName(Guid GroupId)
        {
            try
            {
                var groupDetails = await (from g in Context.GroupMasters
                                          join s in Context.Sites on g.SiteId equals s.SiteId
                                          where g.GroupId == GroupId
                                          select new SiteGroupModel
                                          {
                                              GroupName = g.GroupName,
                                              GroupId = g.GroupId,
                                              SiteId = g.SiteId,
                                              SiteName = s.SiteName,
                                              GroupAddress = g.GroupAddress,
                                          }).FirstOrDefaultAsync();

                return groupDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ApiResponseModel> UpdateSiteGroupMaster(SiteGroupModel groupDetails)
        {
            ApiResponseModel model = new ApiResponseModel();

            try
            {
                var existingRecords = await Context.GroupMasters
                    .Where(e => e.GroupId == groupDetails.GroupId)
                    .FirstOrDefaultAsync();

                if (existingRecords != null)
                {
                    existingRecords.GroupName = groupDetails.GroupName;
                    existingRecords.SiteId = groupDetails.SiteId;
                    existingRecords.GroupAddress = groupDetails.GroupAddress;

                    Context.GroupMasters.Update(existingRecords);

                    await Context.SaveChangesAsync();

                    model.code = 200;
                    model.message = "Site group master updated successfully.";
                }
                else
                {
                    model.code = 400;
                    model.message = "Group not found.";
                }
            }
            catch (Exception ex)
            {
                model.code = 500;
                model.message = "An error occurred while updating site group details: " + ex.Message;
            }

            return model;
        }
    }
}
