using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.DataTableParameters;
using AccountManagement.DBContext.Models.ViewModels.FormPermissionMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Interfaces.Authentication;
using Azure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Repository.AuthenticationRepository
{
    public class UserAuthentication : IAuthentication
    {
        public UserAuthentication(DbaccManegmentContext context)
        {
            Context = context;
        }

        public DbaccManegmentContext Context { get; }

        public async Task<UserResponceModel> ActiveDeactiveUsers(Guid UserId)
        {
            UserResponceModel response = new UserResponceModel();
            var GetUserdta = Context.Users.Where(a => a.Id == UserId).FirstOrDefault();

            if (GetUserdta != null)
            {

                if (GetUserdta.IsActive == true)
                {
                    GetUserdta.IsActive = false;
                    Context.Users.Update(GetUserdta);
                    Context.SaveChanges();
                    response.Code = 200;
                    response.Data = GetUserdta;
                    response.Message = "User" + " " + GetUserdta.UserName + " " + "Is Deactive Succesfully";
                }

                else
                {
                    GetUserdta.IsActive = true;
                    Context.Users.Update(GetUserdta);
                    Context.SaveChanges();
                    response.Code = 200;
                    response.Data = GetUserdta;
                    response.Message = "User" + " " + GetUserdta.UserName + " " + "Is Active Succesfully";
                }


            }
            return response;
        }

        public async Task<UserResponceModel> CreateUser(UserViewModel CreateUser)
        {
            UserResponceModel response = new UserResponceModel();

            try
            {
                bool isEmailAlreadyExists = Context.Users.Any(x => x.Email == CreateUser.Email || x.UserName == CreateUser.UserName);

                if (isEmailAlreadyExists)
                {
                    response.Message = "User with this UserName or email already exists";
                    response.Code = (int)HttpStatusCode.Conflict;
                }
                else
                {
                    var model = new User()
                    {
                        Id = Guid.NewGuid(),
                        FirstName = CreateUser.FirstName,
                        LastName = CreateUser.LastName,
                        UserName = CreateUser.UserName,
                        Email = CreateUser.Email,
                        PhoneNo = CreateUser.PhoneNo,
                        Password = CreateUser.Password,
                        RoleId = CreateUser.Role,
                        IsActive = true,
                        SiteId = CreateUser.SiteId,
                        IsDeleted = false,
                        CreatedBy = CreateUser.CreatedBy,
                        CreatedOn = DateTime.Now,
                    };

                    Context.Users.Add(model);
                    Context.SaveChanges();

                    response.Code = (int)HttpStatusCode.OK;
                    response.Message = "User Created Successfully";
                }
            }
            catch (Exception ex)
            {

                response.Code = (int)HttpStatusCode.InternalServerError;
                response.Message = "An error occurred while creating the user";
            }

            return response;
        }

        public async Task<UserResponceModel> DeleteUserDetails(Guid UserId)
        {
            UserResponceModel response = new UserResponceModel();
            var GetUserdata = Context.Users.Where(a => a.Id == UserId).FirstOrDefault();

            if (GetUserdata != null)
            {
                GetUserdata.IsDeleted = true;
                Context.Users.Update(GetUserdata);
                Context.SaveChanges();
                response.Code = 200;
                response.Data = GetUserdata;
                response.Message = "User is Deleted Successfully";
            }
            return response;
        }

        public async Task<LoginView> GetUserById(Guid UserId)
        {
            LoginView Userdata = new LoginView();
            try
            {
                Userdata = (from e in Context.Users.Where(x => x.Id == UserId)
                            join r in Context.UserRoles on e.RoleId equals r.RoleId
                            join p in Context.Sites on e.SiteId equals p.SiteId
                            select new LoginView
                            {
                                Id = e.Id,
                                UserName = e.UserName,
                                FirstName = e.FirstName,
                                LastName = e.LastName,
                                Email = e.Email,
                                Password = e.Password,
                                IsActive = e.IsActive,
                                PhoneNo = e.PhoneNo,
                                RoleId = e.RoleId,
                                RoleName = r.Role,
                                SiteName = p.SiteName,
                                SiteId = e.SiteId,
                            }).First();
                return Userdata;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<LoginView>> GetUsersList(string? searchText, string? searchBy, string? sortBy)
        {
            try
            {
                IEnumerable<LoginView> userList = (from e in Context.Users
                                                   join r in Context.UserRoles on e.RoleId equals r.RoleId
                                                   join p in Context.Sites on e.SiteId equals p.SiteId
                                                   where e.IsDeleted == false
                                                   select new LoginView
                                                   {
                                                       Id = e.Id,
                                                       FirstName = e.FirstName,
                                                       LastName = e.LastName,
                                                       UserName = e.UserName,
                                                       Email = e.Email,
                                                       PhoneNo = e.PhoneNo,
                                                       IsActive = e.IsActive,
                                                       RoleName = r.Role,
                                                       SiteName = p.SiteName,
                                                       SiteId = e.SiteId,
                                                   });


                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    userList = userList.Where(u =>
                        u.UserName.ToLower().Contains(searchText) ||
                        u.Email.ToLower().Contains(searchText) ||
                        u.PhoneNo.ToLower().Contains(searchText) ||
                        u.FirstName.ToLower().Contains(searchText) ||
                        u.LastName.ToLower().Contains(searchText)

                    );
                }

                if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(searchBy))
                {
                    searchText = searchText.ToLower();
                    switch (searchBy.ToLower())
                    {
                        case "username":
                            userList = userList.Where(u => u.UserName.ToLower().Contains(searchText));
                            break;
                        case "email":
                            userList = userList.Where(u => u.Email.ToLower().Contains(searchText));
                            break;
                        case "phone":
                            userList = userList.Where(u => u.PhoneNo.ToLower().Contains(searchText));
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
                        case "username":
                            if (sortOrder == "ascending")
                                userList = userList.OrderBy(u => u.UserName);
                            else if (sortOrder == "descending")
                                userList = userList.OrderByDescending(u => u.UserName);
                            break;
                        case "role":
                            if (sortOrder == "ascending")
                                userList = userList.OrderBy(u => u.RoleName);
                            else if (sortOrder == "descending")
                                userList = userList.OrderByDescending(u => u.RoleName);
                            break;
                        case "active":
                            if (sortOrder == "ascending")
                                userList = userList.OrderBy(u => u.IsActive);
                            else if (sortOrder == "descending")
                                userList = userList.OrderByDescending(u => u.IsActive);
                            break;
                        case "email":
                            if (sortOrder == "ascending")
                                userList = userList.OrderBy(u => u.Email);
                            else if (sortOrder == "descending")
                                userList = userList.OrderByDescending(u => u.Email);
                            break;
                        case "phone":
                            if (sortOrder == "ascending")
                                userList = userList.OrderBy(u => u.PhoneNo);
                            else if (sortOrder == "descending")
                                userList = userList.OrderByDescending(u => u.PhoneNo);
                            break;
                        default:

                            break;
                    }
                }

                return userList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LoginResponseModel> LoginUser(LoginRequest Loginrequest)
        {
            LoginResponseModel response = new LoginResponseModel();
            try
            {
                var tblUser = await Context.Users.SingleOrDefaultAsync(p => p.UserName == Loginrequest.UserName);
                if (tblUser != null)
                {
                    if (tblUser.IsActive == true)
                    {
                        if (tblUser.Password == Loginrequest.Password)
                        {
                            LoginView userModel = new LoginView();
                            userModel.UserName = tblUser.UserName;
                            userModel.Id = tblUser.Id;
                            userModel.RoleId = tblUser.RoleId;
                            userModel.FullName = tblUser.FirstName + " " + tblUser.LastName;
                            userModel.FirstName = tblUser.FirstName;
                            response.Data = userModel;
                            response.Code = (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            response.Message = "Your Password Is Wrong";
                        }
                    }
                    else
                    {
                        response.Code = (int)HttpStatusCode.Forbidden;
                        response.Message = "Your Deactive Contact Your Admin";
                        return response;
                    }
                }
                else
                {
                    response.Message = "User Not Exist";
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public async Task<ApiResponseModel> RolewisePermission(RolewiseFormPermissionModel RolePermission)
        {
            ApiResponseModel response = new ApiResponseModel();

            try
            {

                var model = new RolewiseFormPermission()
                {

                    RoleId = RolePermission.RoleId,
                    FormId = RolePermission.FormId,
                    IsViewAllow = RolePermission.IsViewAllow,
                    IsEditAllow = RolePermission.IsEditAllow,
                    IsDeleteAllow = RolePermission.IsDeleteAllow,
                    CreatedBy = RolePermission.CreatedBy,
                    CreatedOn = DateTime.Now,
                };

                Context.RolewiseFormPermissions.Add(model);
                Context.SaveChanges();

                response.code = (int)HttpStatusCode.OK;
                response.message = "Permissions Given Successfully";

            }
            catch (Exception ex)
            {

                response.code = (int)HttpStatusCode.InternalServerError;
                response.message = "An error occurred while creating the user";
            }

            return response;
        }

        public async Task<UserResponceModel> UpdateUserDetails(UserViewModel UpdateUser)
        {
            try
            {
                UserResponceModel response = new UserResponceModel();
                var Userdata = await Context.Users.FirstOrDefaultAsync(a => a.Id == UpdateUser.Id);
                if (Userdata != null)
                {
                    Userdata.Id = UpdateUser.Id;
                    Userdata.FirstName = UpdateUser.FirstName;
                    Userdata.LastName = UpdateUser.LastName;
                    Userdata.UserName = UpdateUser.UserName;
                    Userdata.Password = UpdateUser.Password;
                    Userdata.Email = UpdateUser.Email;
                    Userdata.PhoneNo = UpdateUser.PhoneNo;
                    Userdata.RoleId = UpdateUser.Role;
                    Userdata.PhoneNo = UpdateUser.PhoneNo;
                    Context.Users.Update(Userdata);
                    Context.SaveChanges();
                }
                response.Code = (int)HttpStatusCode.OK;
                response.Message = "User Data Updated Successfully";
                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
