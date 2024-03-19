using AccountManagement.API;
using AccountManagement.DBContext.Models.ViewModels.CompanyModels;
using AccountManagement.Repository.Interface.Repository.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Repository.CompanyRepository
{
    public class Company : ICompany
    {

        public Task<CompanyModel> CreateCompany()
        {
            throw new NotImplementedException();
        }
    }
}
