using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.DataTableParameters;
using AccountManagement.DBContext.Models.ViewModels.FormPermissionMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Interfaces.Authentication;
using AccountManegments.Web.Models;
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
using System.Threading.Tasks.Dataflow;

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
            var GetSite = Context.Sites.FirstOrDefault(e => e.SiteId == GetUserdta.SiteId && e.IsDeleted == false);

            if (GetUserdta != null && GetSite != null)
            {
                if (GetSite.IsActive)
                {
                    if (GetUserdta.IsActive)
                    {
                        GetUserdta.IsActive = false;
                        Context.Users.Update(GetUserdta);
                        await Context.SaveChangesAsync();
                        response.Code = 200;
                        response.Data = GetUserdta;
                        response.Message = "User" + " " + GetUserdta.UserName + " " + "is deactive succesfully";
                    }
                    else
                    {
                        GetUserdta.IsActive = true;
                        Context.Users.Update(GetUserdta);
                        await Context.SaveChangesAsync();
                        response.Code = 200;
                        response.Data = GetUserdta;
                        response.Message = "User" + " " + GetUserdta.UserName + " " + "is active succesfully";
                    }
                }
                else
                {
                    response.Message = "This user's site is inactive.";
                    response.Code = 400;
                }
            }
            else
            {
                response.Message = "This user's site is deleted.";
                response.Code = 400;
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
            var Activesite = Context.Sites.Where(a => a.SiteId == GetUserdata.SiteId && a.IsActive == false).FirstOrDefault();

            if (GetUserdata != null && GetUserdata.IsActive == false)
            {
                if (Activesite != null)
                {
                    GetUserdata.IsDeleted = true;
                    Context.Users.Update(GetUserdata);
                    Context.SaveChanges();
                    response.Code = 200;
                    response.Data = GetUserdata;
                    response.Message = "User is deleted successfully";
                }
                else
                {
                    response.Message = "This user's site is active so user can't delete.";
                    response.Code = 400;
                }

            }
            else
            {
                response.Message = "Active user can't delete.";
                response.Code = 400;
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
                                SiteName = e.SiteId == null ? null : Context.Sites.Where(a => a.SiteId == e.SiteId).FirstOrDefault().SiteName,
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
                                                       SiteName = e.SiteId == null ? null : Context.Sites.Where(a => a.SiteId == e.SiteId).FirstOrDefault().SiteName,
                                                       SiteId = e.SiteId,
                                                       CreatedOn = e.CreatedOn,
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

                if (string.IsNullOrEmpty(sortBy))
                {
                    userList = userList.OrderByDescending(u => u.CreatedOn);
                }
                else
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
                        case "firstname":
                            if (sortOrder == "ascending")
                                userList = userList.OrderBy(u => u.FirstName);
                            else if (sortOrder == "descending")
                                userList = userList.OrderByDescending(u => u.FirstName);
                            break;
                        case "createdon":
                            if (sortOrder == "ascending")
                                userList = userList.OrderBy(u => u.CreatedOn);
                            else if (sortOrder == "descending")
                                userList = userList.OrderByDescending(u => u.CreatedOn);
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

        public async Task<LoginResponseModel> LoginUser(LoginRequest loginRequest)
        {
            LoginResponseModel response = new LoginResponseModel();
            try
            {
                var tblUser = await (from u in Context.Users
                                     join r in Context.UserRoles on u.RoleId equals r.RoleId
                                     where u.UserName == loginRequest.UserName
                                     select new
                                     {
                                         User = u,
                                         Role = r.Role,
                                     }).FirstOrDefaultAsync();

                if (tblUser != null)
                {
                    if (tblUser.User.IsActive)
                    {
                        if (tblUser.User.SiteId != null)
                        {
                            var userData = await (from u in Context.Users
                                                  join s in Context.Sites on u.SiteId equals s.SiteId
                                                  where u.UserName == loginRequest.UserName
                                                  select new
                                                  {
                                                      User = u,
                                                      SiteName = s.SiteName,
                                                  }).FirstOrDefaultAsync();

                            if (tblUser.User.Password == loginRequest.Password)
                            {
                                LoginView userModel = new LoginView();
                                userModel.UserName = tblUser.User.UserName;
                                userModel.Id = tblUser.User.Id;
                                userModel.RoleId = tblUser.User.RoleId;
                                userModel.RoleName = tblUser.Role;
                                userModel.FullName = tblUser.User.FirstName + " " + tblUser.User.LastName;
                                userModel.FirstName = tblUser.User.FirstName;
                                userModel.SiteName = userData.SiteName;
                                userModel.SiteId = tblUser.User.SiteId;
                                response.Data = userModel;
                                response.Code = (int)HttpStatusCode.OK;

                                List<FromPermission> fromPermissionData = await (from rp in Context.RolewiseFormPermissions
                                                                                 join f in Context.Forms on rp.FormId equals f.FormId
                                                                                 where rp.RoleId == userModel.RoleId && f.IsActive
                                                                                 orderby f.OrderId ascending
                                                                                 select new FromPermission
                                                                                 {
                                                                                     FormName = f.FormName,
                                                                                     GroupName = f.FormGroup,
                                                                                     Controller = f.Controller,
                                                                                     Action = f.Action,
                                                                                     Add = rp.IsAddAllow,
                                                                                     View = rp.IsViewAllow,
                                                                                     Edit = rp.IsEditAllow,
                                                                                     Delete = rp.IsDeleteAllow,
                                                                                 }).ToListAsync();

                                userModel.FromPermissionData = fromPermissionData;
                            }
                            else
                            {
                                response.Message = "Your password is incorrect";
                            }
                        }
                        else
                        {
                            if (tblUser.User.Password == loginRequest.Password)
                            {
                                LoginView userModel = new LoginView();
                                userModel.UserName = tblUser.User.UserName;
                                userModel.Id = tblUser.User.Id;
                                userModel.RoleId = tblUser.User.RoleId;
                                userModel.RoleName = tblUser.Role;
                                userModel.FullName = tblUser.User.FirstName + " " + tblUser.User.LastName;
                                userModel.FirstName = tblUser.User.FirstName;
                                userModel.SiteId = tblUser.User.SiteId;
                                response.Data = userModel;
                                response.Code = (int)HttpStatusCode.OK;

                                List<FromPermission> fromPermissionData = await (from rp in Context.RolewiseFormPermissions
                                                                                 join f in Context.Forms on rp.FormId equals f.FormId
                                                                                 where rp.RoleId == userModel.RoleId && f.IsActive
                                                                                 orderby f.OrderId ascending
                                                                                 select new FromPermission
                                                                                 {
                                                                                     FormName = f.FormName,
                                                                                     GroupName = f.FormGroup,
                                                                                     Controller = f.Controller,
                                                                                     Action = f.Action,
                                                                                     Add = rp.IsAddAllow,
                                                                                     View = rp.IsViewAllow,
                                                                                     Edit = rp.IsEditAllow,
                                                                                     Delete = rp.IsDeleteAllow,
                                                                                 }).ToListAsync();

                                userModel.FromPermissionData = fromPermissionData;
                            }
                            else
                            {
                                response.Message = "Your password is incorrect";
                            }
                        }
                    }
                    else
                    {
                        response.Code = (int)HttpStatusCode.Forbidden;
                        response.Message = "Your account is inactive. Please contact your administrator.";
                        return response;
                    }
                }
                else
                {
                    response.Message = "User does not exist";
                    response.Code = (int)HttpStatusCode.NotFound;
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while processing your request.";
                response.Code = (int)HttpStatusCode.InternalServerError;
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
                response.message = "Permissions given successfully";

            }
            catch (Exception ex)
            {

                response.code = (int)HttpStatusCode.InternalServerError;
                response.message = "An error occurred while creating the permissions to user";
            }

            return response;
        }

        public async Task<UserResponceModel> UpdateUserDetails(UserViewModel UpdateUser)
        {
            UserResponceModel response = new UserResponceModel();
            try
            {
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
                    Userdata.SiteId = UpdateUser.SiteId;
                    Context.Users.Update(Userdata);
                    Context.SaveChanges();
                    response.Code = (int)HttpStatusCode.OK;
                    response.Message = "User data updated successfully";
                }
                else
                {
                    response.Code = 400;
                    response.Message = "Error in updated user data";
                }
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = "An error occurred while updating user data:" + ex;
            }
            return response;
        }
    }
}
