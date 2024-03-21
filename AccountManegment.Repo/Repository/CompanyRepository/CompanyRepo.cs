using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.CompanyModels;
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
                var company = new API.Company()
                {
                    CompanyId = Guid.NewGuid(),
                    CompanyName = AddCompany.CompanyName,
                    Gstno= AddCompany.Gstno,
                    PanNo= AddCompany.PanNo,
                    Address = AddCompany.Address,
                    Area = AddCompany.Area,
                    CityId = AddCompany.CityId,
                    StateId = AddCompany.StateId,
                    Country = AddCompany.Country,
                    CreatedOn = DateTime.Now,
                    Pincode = AddCompany.Pincode,
                    CreatedBy = AddCompany.CreatedBy,

                };
                response.code = (int)HttpStatusCode.OK;
                response.message = "Company Successfully Inserted";
                Context.Companies.Add(company);
                Context.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
            return response;
        }

        public async Task<IEnumerable<CompanyModel>> GetAllCompany(string? searchText, string? searchBy, string? sortBy)
        {
            try
            {
                IEnumerable<CompanyModel> company = Context.Companies.ToList().Select(a => new CompanyModel
                {
                    CompanyId = a.CompanyId,
                    CompanyName = a.CompanyName,
                    Gstno = a.Gstno,
                    PanNo=a.PanNo,
                    Address = a.Address,
                    Area = a.Area,
                    CityId = a.CityId,
                    StateId = a.StateId,
                    Country = a.Country,
                    Pincode =a.Pincode,
                });
                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    company = company.Where(u =>
                        u.CompanyName.ToLower().Contains(searchText) ||
                        u.Gstno.ToLower().Contains(searchText) ||
                        u.PanNo.ToLower().Contains(searchText) 
                    );
                }

                if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
                {
                    searchText = searchText.ToLower();
                    switch (searchBy.ToLower())
                    {
                        case "CompanyName":
                            company = company.Where(u => u.CompanyName.ToLower().Contains(searchText));
                            break;
                        case "GstNo":
                            company = company.Where(u => u.Gstno.ToLower().Contains(searchText));
                            break;
                        case "PanNo":
                            company = company.Where(u => u.PanNo.ToLower().Contains(searchText));
                            break;
                        default:

                            break;
                    }
                }

                if (!string.IsNullOrEmpty(sortBy))
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
                        case "gstno":
                            if (sortOrder == "ascending")
                                company = company.OrderBy(u => u.Gstno);
                            else if (sortOrder == "descending")
                                company = company.OrderByDescending(u => u.Gstno);
                            break;
                        case "panno":
                            if (sortOrder == "ascending")
                                company = company.OrderBy(u => u.PanNo);
                            else if (sortOrder == "descending")
                                company = company.OrderByDescending(u => u.PanNo);
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

        public async Task<CompanyModel> GetCompnaytById(Guid CompanyId)
        {
            var company = await Context.Companies.SingleOrDefaultAsync(x => x.CompanyId == CompanyId);
            CompanyModel model = new CompanyModel
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                Gstno=company.Gstno,
                PanNo=company.PanNo,
                Address = company.Address,
                Area = company.Area,
                CityId = company.CityId,
                StateId = company.StateId,
                Country = company.Country,
                Pincode = company.Pincode,
            };
            return model;
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
                model.message = "Company Updated Successfully!";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return model;
        }
    }     
    
}
