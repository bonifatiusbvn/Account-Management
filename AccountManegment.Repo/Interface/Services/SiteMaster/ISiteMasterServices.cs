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
        Task<IEnumerable<SiteMasterModel>> GetSiteNameList();
    }
}
