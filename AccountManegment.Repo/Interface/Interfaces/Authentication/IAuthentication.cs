using AccountManagement.DBContext.Models.ViewModels.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Interfaces.Authentication
{
    public interface IAuthentication
    {
        Task<UserResponceModel> UserSingUp(UserModel AddEmployee);
    }
}
