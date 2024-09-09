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
                    CreatedBy = formPermission.CreatedBy,
                    CreatedOn = DateTime.Now,
                };
                Context.RolewiseFormPermissions.Add(formPermissiondata);
                Context.SaveChanges();

                response.code = (int)HttpStatusCode.OK;
                response.message = "Rolewise permission inserted successfully.";
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
                         join u in Context.Users on e.CreatedBy equals u.Id
                         join f in Context.Forms on e.FormId equals f.FormId
                         where f.IsActive == true
                         select new RolewiseFormPermissionModel
                         {
                             Id = e.Id,
                             Role = r.Role,
                             RoleId = e.RoleId,
                             FormId = e.FormId,
                             FormName = f.FormGroup,
                             FullName = u.FirstName + " " + u.LastName,
                             IsViewAllow = e.IsViewAllow,
                             IsEditAllow = e.IsEditAllow,
                             IsDeleteAllow = e.IsDeleteAllow,
                             IsApproved = e.IsApproved,
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
                                                                              join u in Context.Users on a.CreatedBy equals u.Id
                                                                              join f in Context.Forms on a.FormId equals f.FormId
                                                                              where f.IsActive == true
                                                                              orderby f.OrderId ascending
                                                                              select new RolewiseFormPermissionModel
                                                                              {
                                                                                  Id = a.Id,
                                                                                  RoleId = a.RoleId,
                                                                                  Role = r.Role,
                                                                                  FormId = a.FormId,
                                                                                  FormName = f.FormGroup,
                                                                                  IsViewAllow = a.IsViewAllow,
                                                                                  IsEditAllow = a.IsEditAllow,
                                                                                  IsDeleteAllow = a.IsDeleteAllow,
                                                                                  FullName = u.FirstName + " " + u.LastName,
                                                                                  IsApproved = a.IsApproved,
                                                                                  CreatedBy = a.CreatedBy,
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
                    formPermissionData.IsApproved = updateFormPermission.IsApproved;
                }
                Context.RolewiseFormPermissions.Update(formPermissionData);
                Context.SaveChanges();
                model.code = 200;
                model.message = "Rolewiseform permission successfully updated.";
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
                        IsApproved = item.IsApproved,
                        CreatedBy = item.CreatedBy,
                        CreatedOn = DateTime.Now,
                    };
                    Context.RolewiseFormPermissions.Add(rolewisePermission);
                }

                await Context.SaveChangesAsync();
                response.code = 200;
                response.message = "Rolewise permission successfully created.";
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.message = "Error creating rolewise permission";
            }
            return response;
        }

        public async Task<List<RolewiseFormPermissionModel>> GetRolewiseFormListById(int RoleId)
        {
            var UserData = new List<RolewiseFormPermissionModel>();
            var data = await (from e in Context.RolewiseFormPermissions.Where(x => x.RoleId == RoleId)
                              join r in Context.UserRoles on e.RoleId equals r.RoleId
                              join f in Context.Forms on e.FormId equals f.FormId
                              where f.IsActive == true && f.FormName != "Dashboard"
                              orderby f.OrderId ascending
                              select new RolewiseFormPermissionModel
                              {
                                  Id = e.Id,
                                  Role = r.Role,
                                  RoleId = e.RoleId,
                                  FormId = e.FormId,
                                  FormName = f.FormName,
                                  IsViewAllow = e.IsViewAllow,
                                  IsEditAllow = e.IsEditAllow,
                                  IsDeleteAllow = e.IsDeleteAllow,
                                  IsAddAllow = e.IsAddAllow,
                                  CreatedBy = e.CreatedBy,
                                  CreatedOn = e.CreatedOn,
                                  IsApproved = e.IsApproved,
                              }).ToListAsync();


            if (data.Count != 0)
            {
                UserData.AddRange(data);
            }
            return UserData;
        }

        public async Task<ApiResponseModel> UpdateMultipleRolewiseFormPermission(List<RolewiseFormPermissionModel> UpdatedRolewiseFormPermissions)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                foreach (var updatedPermission in UpdatedRolewiseFormPermissions)
                {
                    var existingPermissions = await Context.RolewiseFormPermissions
                        .Where(rp => rp.RoleId == updatedPermission.RoleId && rp.FormId == updatedPermission.FormId)
                        .ToListAsync();

                    if (existingPermissions.Any())
                    {
                        foreach (var Item in existingPermissions)
                        {
                            Item.IsAddAllow = updatedPermission.IsAddAllow;
                            Item.IsViewAllow = updatedPermission.IsViewAllow;
                            Item.IsEditAllow = updatedPermission.IsEditAllow;
                            Item.IsDeleteAllow = updatedPermission.IsDeleteAllow;
                            Item.IsApproved = updatedPermission.IsApproved;
                            Context.Entry(Item).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        response.code = 404;
                        response.message = $"Permissions with roleid {updatedPermission.RoleId} and formid {updatedPermission.FormId} not found.";
                        return response;
                    }
                }

                await Context.SaveChangesAsync();
                response.code = 200;
                response.message = "Rolewise permissions successfully updated.";
            }
            catch (Exception ex)
            {
                response.code = 400;
                response.message = "Error updating rolewise Permissions";
            }
            return response;
        }

        public async Task<ApiResponseModel> CreateUserRole(UserRoleModel roleDetails)
        {
            ApiResponseModel response = new ApiResponseModel();
            try
            {
                bool isRoleAlreadyExists = Context.UserRoles.Any(x => x.Role == roleDetails.Role);
                if (isRoleAlreadyExists)
                {
                    var RoleDetail = await Context.UserRoles.SingleOrDefaultAsync(x => x.Role == roleDetails.Role);
                    if (RoleDetail.IsDelete == null || RoleDetail.IsDelete == false)
                    {
                        response.message = "Role already exists";
                        response.code = (int)HttpStatusCode.NotFound;
                    }
                    else
                    {
                        var GetRoledata = Context.UserRoles.Where(a => a.Role == roleDetails.Role).FirstOrDefault();
                        GetRoledata.IsDelete = false;
                        Context.UserRoles.Update(GetRoledata);
                        await Context.SaveChangesAsync();
                        response.data = roleDetails;
                        response.message = "Role added successfully!";
                    }
                }
                else
                {

                    var rolemodel = new UserRole()
                    {
                        Role = roleDetails.Role,
                        IsActive = true,
                        IsDelete = false,
                        CreatedBy = roleDetails.CreatedBy,
                        CreatedOn = DateTime.Now,
                    };


                    Context.UserRoles.Add(rolemodel);
                    await Context.SaveChangesAsync();


                    var forms = Context.Forms.ToList();
                    var roleWiseFormPermissions = new List<RolewiseFormPermission>();

                    foreach (var form in forms)
                    {
                        var permissions = new RolewiseFormPermission
                        {
                            RoleId = rolemodel.RoleId,
                            FormId = form.FormId,
                            IsAddAllow = true,
                            IsViewAllow = true,
                            IsEditAllow = true,
                            IsDeleteAllow = true,
                            CreatedOn = DateTime.Now,
                            CreatedBy = roleDetails.CreatedBy,
                        };
                        roleWiseFormPermissions.Add(permissions);
                    }

                    Context.RolewiseFormPermissions.AddRange(roleWiseFormPermissions);
                    await Context.SaveChangesAsync();

                    response.message = "Role added successfully!";
                    response.data = rolemodel;
                }
            }
            catch (Exception ex)
            {
                response.code = (int)HttpStatusCode.InternalServerError;
                response.message = "Error in creating role: " + ex.Message;
            }
            return response;
        }

        public Task<UserResponceModel> ActiveDeactiveRole(int roleId)
        {
            throw new NotImplementedException();
        }

        public Task<UserResponceModel> DeleteRole(int roleId)
        {
            throw new NotImplementedException();
        }
    }
}
