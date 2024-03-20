using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Repository.SiteMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
                var SiteMaster = new Site()
                {
                    SiteId = Guid.NewGuid(),
                    SiteName = SiteDetails.SiteName,
                    IsActive = SiteDetails.IsActive,
                    ContectPersonName = SiteDetails.ContectPersonName,
                    ContectPersonPhoneNo = SiteDetails.ContectPersonPhoneNo,
                    Address = SiteDetails.Address,
                    Area = SiteDetails.Area,
                    CityId = SiteDetails.CityId,
                    StateId = SiteDetails.StateId,
                    Country= SiteDetails.Country,
                    Pincode = SiteDetails.Pincode,
                    ShippingAddress = SiteDetails.ShippingAddress,
                    ShippingArea = SiteDetails.ShippingArea,
                    ShippingCityId = SiteDetails.ShippingCityId,
                    ShippingStateId = SiteDetails.ShippingStateId,
                    ShippingCountry= SiteDetails.ShippingCountry,
                    ShippingPincode= SiteDetails.ShippingPincode,
                    CreatedBy = SiteDetails.CreatedBy,
                    CreatedOn = DateTime.Now,
                };
                response.code = (int)HttpStatusCode.OK;
                response.message = "SiteDetails Successfully Inserted";
                Context.Sites.Add(SiteMaster);
                Context.SaveChanges();
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
                            join b in Context.Cities on a.CityId equals b.CityId
                            join c in Context.States on a.StateId equals c.StatesId
                            join d in Context.Countries on a.Country equals d.CountryId
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
                                ShippingCityName = b.CityName,
                                ShippingStateId = a.ShippingStateId,
                                ShippingStateName = c.StatesName,
                                ShippingCountry = a.ShippingCountry,
                                ShippingCountryName = d.CountryName,
                                ShippingPincode = a.ShippingPincode,
                                CreatedBy = a.CreatedBy,
                                CreatedOn = a.CreatedOn,
                            }).First();
                return SiteList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<SiteMasterModel>> GetSiteList()
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
                                   CityName=b.CityName,
                                   StateId = a.StateId,
                                   StateName=c.StatesName,
                                   Country = a.Country,
                                   CountryName=d.CountryName,
                                   Pincode = a.Pincode,
                                   ShippingAddress = a.ShippingAddress,
                                   ShippingArea = a.ShippingArea,
                                   ShippingCityId = a.ShippingCityId,
                                   ShippingCityName=g.CityName,
                                   ShippingStateId = a.ShippingStateId,
                                   ShippingStateName=f.StatesName,
                                   ShippingCountry = a.ShippingCountry,
                                   ShippingCountryName=e.CountryName,
                                   ShippingPincode = a.ShippingPincode,
                                   CreatedBy = a.CreatedBy,
                                   CreatedOn = a.CreatedOn,
                               });
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
                    SiteMaster.SiteId = SiteDetails.SiteId;
                    SiteMaster.SiteName = SiteDetails.SiteName;
                    SiteMaster.IsActive = SiteDetails.IsActive;
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
                    SiteMaster.CreatedBy = SiteDetails.CreatedBy;
                    SiteMaster.CreatedOn = SiteDetails.CreatedOn;
                }
                Context.Sites.Update(SiteMaster);
                Context.SaveChanges();
                model.code = 200;
                model.message = "SiteDetails Updated Successfully!";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return model;
        }
    }
}
