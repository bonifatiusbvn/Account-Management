using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.CompanyModels;
using AccountManagement.DBContext.Models.ViewModels.ItemMaster;
using AccountManagement.DBContext.Models.ViewModels.SiteMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Repository.Company;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AccountManagement.Repository.Repository.CompanyRepository
{
    public class CompanyRepo : ICompany
    {
        public CompanyRepo(DbaccManegmentContext context)
        {
            Context = context;
        }

        public DbaccManegmentContext Context { get; }

        public async Task<ApiResponseModel> AddCompany(CompanyModel AddCompany)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var company = new Company()
                {
                    CompanyId = Guid.NewGuid(),
                    CompanyName = AddCompany.CompanyName,
                    Gstno = AddCompany.Gstno,
                    PanNo = AddCompany.PanNo,
                    Address = AddCompany.Address,
                    Area = AddCompany.Area,
                    CityId = AddCompany.CityId,
                    StateId = AddCompany.StateId,
                    Country = AddCompany.Country,
                    CreatedOn = DateTime.Now,
                    Pincode = AddCompany.Pincode,
                    CreatedBy = AddCompany.CreatedBy,
                    IsDelete = false,

                };
                response.code = (int)HttpStatusCode.OK;
                response.message = "Company successfully created.";
                Context.Companies.Add(company);
                Context.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
            return response;
        }

        public async Task<ApiResponseModel> DeleteCompanyDetails(Guid CompanyId)
        {
            ApiResponseModel response = new ApiResponseModel();
            var company = Context.Companies.Where(a => a.CompanyId == CompanyId).FirstOrDefault();

            if (company != null)
            {

                company.IsDelete = true;
                Context.Companies.Update(company);
                Context.SaveChanges();
                response.code = 200;
                response.message = "Company is successfully deleted.";
            }
            return response;
        }

        public async Task<IEnumerable<CompanyModel>> GetAllCompany(string? searchText, string? searchBy, string? sortBy)
        {
            try
            {
                IEnumerable<CompanyModel> company = Context.Companies.Where(c => c.IsDelete == false).ToList().Select(a => new CompanyModel
                {
                    CompanyId = a.CompanyId,
                    CompanyName = a.CompanyName,
                    Gstno = a.Gstno,
                    PanNo = a.PanNo,
                    Address = a.Address,
                    Area = a.Area,
                    CityId = a.CityId,
                    StateId = a.StateId,
                    Country = a.Country,
                    Pincode = a.Pincode,
                    CreatedOn = a.CreatedOn,
                });
                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    company = company.Where(u =>
                        u.CompanyName.ToLower().Contains(searchText) ||
                        u.Gstno.ToString().Contains(searchText) ||
                        u.PanNo.ToLower().Contains(searchText)
                    );
                }
                if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
                {
                    searchText = searchText.ToLower();
                    switch (searchBy.ToLower())
                    {
                        case "companyname":
                            company = company.Where(u => u.CompanyName.ToLower().Contains(searchText));
                            break;
                        case "gstno":
                            company = company.Where(u => u.Gstno.ToString().Contains(searchText));
                            break;
                        case "panno":
                            company = company.Where(u => u.PanNo.ToLower().Contains(searchText));
                            break;
                        default:

                            break;
                    }
                }

                if (string.IsNullOrEmpty(sortBy))
                {
                    company = company.OrderByDescending(u => u.CreatedOn);
                }
                else
                {
                    string sortOrder = sortBy.StartsWith("Ascending") ? "ascending" : "descending";
                    string field = sortBy.Substring(sortOrder.Length);

                    switch (field.ToLower())
                    {
                        case "companyname":
                            if (sortOrder == "ascending")
                                company = company.OrderBy(u => u.CompanyName);
                            else if (sortOrder == "descending")
                                company = company.OrderByDescending(u => u.CompanyName);
                            break;
                        case "createdon":
                            if (sortOrder == "ascending")
                                company = company.OrderBy(u => u.CreatedOn);
                            else if (sortOrder == "descending")
                                company = company.OrderByDescending(u => u.CreatedOn);
                            break;
                        default:
                            break;
                    }
                }
                return company;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<CompanyModel>> GetCompanyNameList()
        {
            try
            {
                IEnumerable<CompanyModel> CompanyName = Context.Companies.Where(c => c.IsDelete == false).ToList().Select(a => new CompanyModel
                {
                    CompanyId = a.CompanyId,
                    CompanyName = a.CompanyName,
                });
                return CompanyName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CompanyModel> GetCompnaytById(Guid CompanyId)
        {
            CompanyModel company = new CompanyModel();
            try
            {
                company = (from a in Context.Companies.Where(x => x.CompanyId == CompanyId)
                           join b in Context.Cities on a.CityId equals b.CityId
                           join c in Context.States on a.StateId equals c.StatesId
                           join d in Context.Countries on a.Country equals d.CountryId
                           select new CompanyModel
                           {
                               CompanyId = a.CompanyId,
                               CompanyName = a.CompanyName,
                               Gstno = a.Gstno,
                               PanNo = a.PanNo,
                               Address = a.Address,
                               Area = a.Area,
                               CityId = a.CityId,
                               CityName = b.CityName,
                               StateId = a.StateId,
                               StateName = c.StatesName,
                               CountryName = d.CountryName,
                               Country = a.Country,
                               Pincode = a.Pincode,
                               CreatedBy = a.CreatedBy,
                               CreatedOn = a.CreatedOn,
                               FullAddress = a.Address + "-" + a.Area + "," + b.CityName + "," + c.StatesName + "-" + a.Pincode
                           }).First();
                return company;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ApiResponseModel> UpdateCompany(CompanyModel UpdateCompany)
        {
            ApiResponseModel model = new ApiResponseModel();
            var company = Context.Companies.Where(e => e.CompanyId == UpdateCompany.CompanyId).FirstOrDefault();
            try
            {
                if (company != null)
                {
                    company.CompanyId = UpdateCompany.CompanyId;
                    company.CompanyName = UpdateCompany.CompanyName;
                    company.Gstno = UpdateCompany.Gstno;
                    company.PanNo = UpdateCompany.PanNo;
                    company.Address = UpdateCompany.Address;
                    company.Area = UpdateCompany.Area;
                    company.CityId = UpdateCompany.CityId;
                    company.StateId = UpdateCompany.StateId;
                    company.Country = UpdateCompany.Country;
                    company.Pincode = UpdateCompany.Pincode;
                }
                Context.Companies.Update(company);
                Context.SaveChanges();
                model.code = 200;
                model.message = "Company successfully updated.";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return model;
        }
    }

}
