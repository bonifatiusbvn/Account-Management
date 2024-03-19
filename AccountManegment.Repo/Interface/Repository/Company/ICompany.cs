using AccountManagement.DBContext.Models.ViewModels.CompanyModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Repository.Company
{
    public interface ICompany
    {
        Task<CompanyModel> CreateCompany();
    }
}
