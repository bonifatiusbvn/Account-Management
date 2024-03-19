using AccountManagement.DBContext.Models.DataTableParameters;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Services.AuthenticationService
{
    public interface IAuthenticationService
    {
        Task<IEnumerable<LoginView>> GetUsersList();
        Task<LoginResponseModel> LoginUser(LoginRequest LoginUserRequest);
        Task<LoginView> GetUserById(Guid UserId);
        Task<UserResponceModel> CreateUser(UserViewModel CreateUser);
        Task<UserResponceModel> UpdateUserDetails(UserViewModel UpdateUser);

    }
}
