using AccountManagement.API;
using AccountManagement.DBContext.Models.DataTableParameters;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Interfaces.Authentication;
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



    }
}
