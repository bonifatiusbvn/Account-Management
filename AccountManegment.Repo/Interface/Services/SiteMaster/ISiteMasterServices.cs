using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Services.SiteMaster
{
    public interface ISiteMasterServices
    {
        Task<IEnumerable<SiteMasterModel>> GetSiteList(string? searchText, string? searchBy, string? sortBy);
        Task<ApiResponseModel> AddSiteDetails(SiteMasterModel SiteDetails);
        Task<SiteMasterModel> GetSiteDetailsById(Guid SiteId);
        Task<ApiResponseModel> UpdateSiteDetails(SiteMasterModel SiteDetails);
        Task<ApiResponseModel> ActiveDeactiveSite(Guid SiteId);
        Task<ApiResponseModel> DeleteSite(Guid SiteId);
        Task<IEnumerable<SiteMasterModel>> GetSiteNameList();
        Task<IEnumerable<SiteAddressModel>> GetSiteAddressList(Guid SiteId);
        Task<ApiResponseModel> AddSiteGroupDetails(SiteGroupModel GroupDetails);
        Task<IEnumerable<GroupMasterModel>> GetGroupNameListBySiteId(Guid SiteId);
        Task<IEnumerable<SiteGroupModel>> GetGroupNameList();
        Task<ApiResponseModel> DeleteSiteGroupDetails(Guid GroupId);
        Task<SiteGroupModel> GetGroupDetailsByGroupName(Guid GroupId);
        Task<ApiResponseModel> UpdateSiteGroupMaster(SiteGroupModel groupDetails);
    }
}
