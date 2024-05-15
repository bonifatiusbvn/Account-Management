using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.DataTableParameters;
using AccountManagement.DBContext.Models.ViewModels.FormPermissionMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Interfaces.Authentication;
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

        public AuthenticationService(IAuthentication authentication)
        {
            Authentication = authentication;
        }

        public IAuthentication Authentication { get; }

        public async Task<UserResponceModel> ActiveDeactiveUsers(Guid UserId)
        {
            return await Authentication.ActiveDeactiveUsers(UserId);
        }

        public Task<string> AuthenticateUser(LoginRequest login)
        {
            return Authentication.AuthenticateUser(login);
        }

        public async Task<UserResponceModel> CreateUser(UserViewModel CreateUser)
        {
            return await Authentication.CreateUser(CreateUser);
        }

        public async Task<UserResponceModel> DeleteUserDetails(Guid UserId)
        {
            return await Authentication.DeleteUserDetails(UserId);
        }

        public string GenerateToken(UserViewModel model)
        {
            return Authentication.GenerateToken(model);     
        }

        public async Task<LoginView> GetUserById(Guid UserId)
        {
            return await Authentication.GetUserById(UserId);
        }


        public async Task<IEnumerable<LoginView>> GetUsersList(string? searchText, string? searchBy, string? sortBy)
        {
            return await Authentication.GetUsersList(searchText, searchBy, sortBy);
        }

        public async Task<LoginResponseModel> LoginUser(LoginRequest LoginUser)
        {
            return await Authentication.LoginUser(LoginUser);
        }

        public async Task<ApiResponseModel> RolewisePermission(RolewiseFormPermissionModel RolePermission)
        {
            return await Authentication.RolewisePermission(RolePermission);
        }

        public async Task<UserResponceModel> UpdateUserDetails(UserViewModel UpdateUser)
        {
            return await Authentication.UpdateUserDetails(UpdateUser);
        }
    }
}
