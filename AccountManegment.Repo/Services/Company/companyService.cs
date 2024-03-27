using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.CompanyModels;
using AccountManagement.Repository.Interface.Repository.Company;
using AccountManagement.Repository.Interface.Services.CompanyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Services.Company
{
    public class CompanyService : ICompanyService
    {

        public CompanyService(ICompany company)
        {
            Company = company;
        }

        public ICompany Company { get; }

        public async Task<ApiResponseModel> AddCompany(CompanyModel AddCompany)
        {
            return await Company.AddCompany(AddCompany);
        }

        public async Task<ApiResponseModel> DeleteCompanyDetails(Guid CompanyId)
        {
            return await Company.DeleteCompanyDetails(CompanyId);
        }

        public async Task<IEnumerable<CompanyModel>> GetAllCompany(string? searchText, string? searchBy, string? sortBy)
        {
            return await Company.GetAllCompany(searchText, searchBy, sortBy);
        }

        public async Task<CompanyModel> GetCompnaytById(Guid CompanyId)
        {
            return await Company.GetCompnaytById(CompanyId);
        }

        public async Task<ApiResponseModel> UpdateCompany(CompanyModel UpdateCompany)
        {
            return await Company.UpdateCompany(UpdateCompany);
        }
    }
}
