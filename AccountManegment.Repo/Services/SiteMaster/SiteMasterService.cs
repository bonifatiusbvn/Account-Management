using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Repository.SiteMaster;
using AccountManagement.Repository.Interface.Services.SiteMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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

        public async Task<ApiResponseModel> DeleteSite(Guid SiteId)
        {
            return await SiteMaster.DeleteSite(SiteId);
        }

        public async Task<IEnumerable<SiteMasterModel>> GetSiteNameList()
        {
            return await SiteMaster.GetSiteNameList();
        }
        public async Task<IEnumerable<SiteAddressModel>> GetSiteAddressList(Guid SiteId)
        {
            return await SiteMaster.GetSiteAddressList(SiteId);
        }

        public async Task<ApiResponseModel> AddSiteGroupDetails(GroupMasterModel GroupDetails)
        {
            return await SiteMaster.AddSiteGroupDetails(GroupDetails);
        }

        public async Task<IEnumerable<GroupMasterModel>> GetGroupNameListBySiteId(Guid SiteId)
        {
            return await SiteMaster.GetGroupNameListBySiteId(SiteId);
        }
        public async Task<IEnumerable<SiteGroupModel>> GetGroupNameList()
        {
            return await SiteMaster.GetGroupNameList();
        }
        public async Task<ApiResponseModel> DeleteSiteGroupDetails(int Id)
        {
            return await SiteMaster.DeleteSiteGroupDetails(Id);
        }
        public async Task<GroupMasterModel> GetGroupDetailsByGroupName(int Id)
        {
            return await SiteMaster.GetGroupDetailsByGroupName(Id);
        }
        public async Task<ApiResponseModel> UpdateSiteGroupMaster(GroupMasterModel groupDetails)
        {
            return await SiteMaster.UpdateSiteGroupMaster(groupDetails);
        }
    }
}
