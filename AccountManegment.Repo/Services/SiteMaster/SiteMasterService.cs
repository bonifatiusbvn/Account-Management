using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Repository.SiteMaster;
using AccountManagement.Repository.Interface.Services.SiteMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Services.SiteMaster
{
    public class SiteMasterService : ISiteMasterServices
    {
        private readonly ISiteMaster SiteMaster;
        public SiteMasterService(ISiteMaster siteMaster)
        {
            SiteMaster = siteMaster;
        }
        public async Task<ApiResponseModel> AddSiteDetails(SiteMasterModel SiteDetails)
        {
            return await SiteMaster.AddSiteDetails(SiteDetails);
        }

        public async Task<SiteMasterModel> GetSiteDetailsById(Guid SiteId)
        {
            return await SiteMaster.GetSiteDetailsById(SiteId);
        }

        public async Task<IEnumerable<SiteMasterModel>> GetSiteList(string? searchText, string? searchBy, string? sortBy)
        {
            return await SiteMaster.GetSiteList(searchText, searchBy, sortBy);
        }

        public async Task<ApiResponseModel> UpdateSiteDetails(SiteMasterModel SiteDetails)
        {
            return await SiteMaster.UpdateSiteDetails(SiteDetails);
        }
        public async Task<ApiResponseModel> ActiveDeactiveSite(Guid SiteId)
        {
            return await SiteMaster.ActiveDeactiveSite(SiteId);
        }

        public async Task<IEnumerable<SiteMasterModel>> GetSiteNameList()
        {
            return await SiteMaster.GetSiteNameList();
        }
    }
}
