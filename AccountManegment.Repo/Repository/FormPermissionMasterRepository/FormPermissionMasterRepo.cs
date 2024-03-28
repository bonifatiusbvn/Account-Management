using AccountManagement.API;
using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.FormPermissionMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Repository.FormPermissionMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Repository.FormPermissionMasterRepository
{
    public class FormPermissionMasterRepo : IFormPermissionMaster
    {
        public FormPermissionMasterRepo(DbaccManegmentContext context)
        {
            Context = context;
        }
        public DbaccManegmentContext Context { get; }

        public async Task<ApiResponseModel> CreateRolewiseFormPermission(RolewiseFormPermissionModel formPermission)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                var formPermissiondata = new RolewiseFormPermission()
                {
                    Id = formPermission.Id,
                    RoleId = formPermission.RoleId,
                    FormId = formPermission.FormId,
                    IsViewAllow = formPermission.IsViewAllow,
                    IsEditAllow = formPermission.IsEditAllow,
                    IsDeleteAllow = formPermission.IsDeleteAllow,
                    UserId = formPermission.UserId,
                    CreatedBy = formPermission.CreatedBy,
                    CreatedOn = DateTime.Now,   
                };
                Context.RolewiseFormPermissions.Add(formPermissiondata);
                Context.SaveChanges();

                response.code = (int)HttpStatusCode.OK;
                response.message = "Rolewise Permission Inserted Successfully!";
            }
            catch (Exception ex)
            {
                response.code = (int)HttpStatusCode.InternalServerError;
                response.message = "An error occurred while inserting the rolewise permission";
            }
            return response;
        }    

        public async Task<RolewiseFormPermissionModel> GetRolewiseFormPermissionById(int formPermissionId)
        {
            RolewiseFormPermissionModel model = new RolewiseFormPermissionModel();
            try
            {
                model = (from e in Context.RolewiseFormPermissions.Where(x => x.Id == formPermissionId)
                            join r in Context.UserRoles on e.RoleId equals r.RoleId
                            join u in Context.Users on e.UserId equals u.Id
                            join f in Context.Forms on e.FormId equals f.FormId
                            select new RolewiseFormPermissionModel
                            {
                                Id = e.Id,
                                Role = r.Role,
                                RoleId = e.RoleId,
                                FormId = e.FormId,
                                FormName = f.FormName,
                                UserId = e.UserId,
                                FullName = u.FirstName + " " + u.LastName,
                                IsViewAllow = e.IsViewAllow,
                                IsEditAllow = e.IsEditAllow,
                                IsDeleteAllow = e.IsDeleteAllow,
                                CreatedBy = e.CreatedBy,
                                CreatedOn = e.CreatedOn

                            }).First();
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<RolewiseFormPermissionModel>> GetRolewiseFormPermissionList()
        {
            try
            {
                IEnumerable<RolewiseFormPermissionModel> FormPermissionList = from a in Context.RolewiseFormPermissions
                                                                  join r in Context.UserRoles on a.RoleId equals r.RoleId
                                                                  join u in Context.Users on a.UserId equals u.Id
                                                                  join f in Context.Forms on a.FormId equals f.FormId
                                                                  select new RolewiseFormPermissionModel
                                                                  {
                                                                      Id = a.Id,
                                                                      RoleId= a.RoleId,
                                                                      Role = r.Role,
                                                                      FormId = a.FormId,
                                                                      FormName = f.FormName,
                                                                      UserId = a.UserId,
                                                                      IsViewAllow = a.IsViewAllow,
                                                                      IsEditAllow = a.IsEditAllow,
                                                                      IsDeleteAllow = a.IsDeleteAllow,
                                                                      FullName = u.FirstName + " " + u.LastName,
                                                                      CreatedBy= a.CreatedBy,
                                                                      CreatedOn = a.CreatedOn

                                                                  };
                return FormPermissionList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ApiResponseModel> UpdateRolewiseFormPermission(RolewiseFormPermissionModel updateFormPermission)
        {
            ApiResponseModel model = new ApiResponseModel();
            var formPermissionData = Context.RolewiseFormPermissions.Where(e => e.Id == updateFormPermission.Id).FirstOrDefault();
            try
            {
                if (updateFormPermission != null)
                {
                    formPermissionData.Id = updateFormPermission.Id;
                    formPermissionData.RoleId = updateFormPermission.RoleId;
                    formPermissionData.FormId = updateFormPermission.FormId;
                    formPermissionData.IsViewAllow = updateFormPermission.IsViewAllow;
                    formPermissionData.IsEditAllow = updateFormPermission.IsEditAllow;
                    formPermissionData.IsDeleteAllow = updateFormPermission.IsDeleteAllow;
                }
                Context.RolewiseFormPermissions.Update(formPermissionData);
                Context.SaveChanges();
                model.code = 200;
                model.message = "Rolewise Form Permission Updated Successfully!";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return model;
        }

        public async Task<ApiResponseModel> InsertMultipleRolewiseFormPermission(List<RolewiseFormPermissionModel> InsertRolewiseFormPermission)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                foreach (var item in InsertRolewiseFormPermission)
                {
                    var rolewisePermission = new RolewiseFormPermission()
                    {
                        Id = item.Id,
                        RoleId = item.RoleId,
                        FormId = item.FormId,
                        IsViewAllow = item.IsViewAllow,
                        IsEditAllow = item.IsEditAllow,
                        IsDeleteAllow = item.IsDeleteAllow,
                        UserId = item.UserId,
                        CreatedBy = item.CreatedBy,
                        CreatedOn = DateTime.Now,
                    };
                    Context.RolewiseFormPermissions.Add(rolewisePermission);
                }

                await Context.SaveChangesAsync();
                response.code = 200;
                response.message = "Rolewise Permission Created successfully!";
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.message = "Error creating Rolewise Permission";
            }
            return response;
        }

        public async Task<List<RolewiseFormPermissionModel>> GetRolewiseFormList(int RoleId)
        {
            var UserData = new List<RolewiseFormPermissionModel>();
            var data = await (from e in Context.RolewiseFormPermissions.Where(x => x.RoleId == RoleId)
                     join r in Context.UserRoles on e.RoleId equals r.RoleId
                     join u in Context.Users on e.UserId equals u.Id
                     join f in Context.Forms on e.FormId equals f.FormId
                     select new 
                     {
                          e.Id,
                          r.Role,
                          e.RoleId,
                          e.FormId,
                          f.FormName,
                         e.UserId,
                         u.FirstName,
                         u.LastName,
                         e.IsViewAllow,
                         e.IsEditAllow,
                         e.IsDeleteAllow,
                         e.CreatedBy,
                         e.CreatedOn,

                     }).ToListAsync();


            if (data != null)
            {
                foreach (var item in data)
                {
                    UserData.Add(new RolewiseFormPermissionModel()
                    {
                        Id = item.Id,
                        Role = item.Role,
                        RoleId = item.RoleId,
                        FormId = item.FormId,
                        FormName = item.FormName,
                        UserId = item.UserId,
                        FullName = item.FirstName + " " + item.LastName,
                        IsViewAllow = item.IsViewAllow,
                        IsEditAllow = item.IsEditAllow,
                        IsDeleteAllow = item.IsDeleteAllow,
                        CreatedBy = item.CreatedBy,
                        CreatedOn = item.CreatedOn
                    });
                }
            }
            return UserData;
        }
    }
}
