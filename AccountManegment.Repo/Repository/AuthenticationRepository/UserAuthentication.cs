using AccountManagement.API;
using AccountManagement.DBContext.Models.DataTableParameters;
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
                        RoleId = 3,
                        IsActive = true,
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
                                PhoneNo = e.PhoneNo,
                                RoleName = r.Role,

                            }).First();
                return Userdata;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<jsonData> GetUsersList(DataTableRequstModel UsersList)
        {
            var GetUsersList = from e in Context.Users
                               join r in Context.UserRoles on e.RoleId equals r.RoleId
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
                               };

            if (!string.IsNullOrEmpty(UsersList.sortColumn) && !string.IsNullOrEmpty(UsersList.sortColumnDir))
            {

                var property = typeof(LoginView).GetProperty(UsersList.sortColumn);
                if (property != null)
                {
                    var parameter = Expression.Parameter(typeof(LoginView), "x");
                    var propertyAccess = Expression.Property(parameter, property);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);
                    string methodName = UsersList.sortColumnDir.ToLower() == "asc" ? "OrderBy" : "OrderByDescending";
                    var resultExp = Expression.Call(typeof(Queryable), methodName,
                                                    new Type[] { typeof(LoginView), property.PropertyType },
                                                    GetUsersList.Expression, Expression.Quote(orderByExp));
                    GetUsersList = GetUsersList.Provider.CreateQuery<LoginView>(resultExp);
                }
            }

            if (!string.IsNullOrEmpty(UsersList.searchValue))
            {
                GetUsersList = GetUsersList.Where(e => e.UserName.Contains(UsersList.searchValue) || e.RoleName.Contains(UsersList.searchValue));
            }

            int totalRecord = await GetUsersList.CountAsync();
            var cData = await GetUsersList.Skip(UsersList.skip).Take(UsersList.pageSize).ToListAsync();

            jsonData jsonData = new jsonData
            {
                draw = UsersList.draw,
                recordsFiltered = totalRecord,
                recordsTotal = totalRecord,
                data = cData
            };

            return jsonData;
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
                        if (tblUser.UserName == Loginrequest.UserName && tblUser.Password == Loginrequest.Password)
                        {
                            LoginView userModel = new LoginView();
                            userModel.UserName = tblUser.UserName;
                            userModel.Id = tblUser.Id;
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
                    Userdata.Email = UpdateUser.Email;
                    Userdata.PhoneNo = UpdateUser.PhoneNo;
                    Context.Users.Update(Userdata);
                    await Context.SaveChangesAsync();
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
