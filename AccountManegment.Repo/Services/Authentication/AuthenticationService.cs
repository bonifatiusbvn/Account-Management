using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Services.AuthenticationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        public Task<UserResponceModel> UserSingUp(UserModel AddEmployee)
        {
            throw new NotImplementedException();
        }
    }
}
