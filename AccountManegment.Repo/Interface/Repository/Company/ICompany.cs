using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.CompanyModels;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Repository.Company
{
    public interface ICompany
    {
        Task<ApiResponseModel> AddCompany(CompanyModel AddCompany);
        Task<IEnumerable<CompanyModel>> GetAllCompany(string? searchText, string? searchBy, string? sortBy);

        Task<CompanyModel> GetCompnaytById(Guid CompanyId);

        Task<ApiResponseModel> UpdateCompany(CompanyModel UpdateCompany);
        Task<ApiResponseModel> DeleteCompanyDetails(Guid CompanyId);
        Task<IEnumerable<CompanyModel>> GetCompanyNameList();
    }
}
