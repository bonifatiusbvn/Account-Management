using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.DataTableParameters;
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
        Task<LoginResponseModel> LoginUser(LoginRequest LoginUserRequest);
        Task<IEnumerable<LoginView>> GetUsersList(string? searchText, string? searchBy, string? sortBy);

        Task<LoginView> GetUserById(Guid UserId);

        Task<UserResponceModel> CreateUser(UserViewModel CreateUser);
        Task<UserResponceModel> UpdateUserDetails(UserViewModel UpdateUser);

        Task<UserResponceModel> ActiveDeactiveUsers(Guid UserId);
        Task<UserResponceModel> DeleteUserDetails(Guid UserId);

        Task<ApiResponseModel> RolewisePermission(RolewisePermissionModel RolePermission);


    }
}
