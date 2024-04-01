using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.FormPermissionMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using AccountManagement.Repository.Interface.Interfaces.Authentication;
using AccountManagement.Repository.Interface.Repository.FormPermissionMaster;
using AccountManagement.Repository.Interface.Services.FormPermissionMasterService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Services.FormPermissionMaster
{
    public class FormPermissionMasterService : IFormPermissionMasterService
    {
        public FormPermissionMasterService(IFormPermissionMaster formpermissionMaster)
        {
            FormPermissionMaster = formpermissionMaster;
        }
        public IFormPermissionMaster FormPermissionMaster { get; }
        public async Task<ApiResponseModel> CreateRolewiseFormPermission(RolewiseFormPermissionModel formPermission)
        {
            return await FormPermissionMaster.CreateRolewiseFormPermission(formPermission);
        }

        public async Task<RolewiseFormPermissionModel> GetRolewiseFormPermissionById(int formPermissionId)
        {
            return await FormPermissionMaster.GetRolewiseFormPermissionById(formPermissionId);
        }

        public async Task<IEnumerable<RolewiseFormPermissionModel>> GetRolewiseFormPermissionList()
        {
            return await FormPermissionMaster.GetRolewiseFormPermissionList();
        }

        public async Task<ApiResponseModel> InsertMultipleRolewiseFormPermission(List<RolewiseFormPermissionModel> InsertRolewiseFormPermission)
        {
            return await FormPermissionMaster.InsertMultipleRolewiseFormPermission(InsertRolewiseFormPermission);
        }

        public async Task<ApiResponseModel> UpdateRolewiseFormPermission(RolewiseFormPermissionModel updateFormPermission)
        {
            return await FormPermissionMaster.UpdateRolewiseFormPermission(updateFormPermission);
        }

        public async Task<List<RolewiseFormPermissionModel>> GetRolewiseFormListById(int RoleId)
        {
            return await FormPermissionMaster.GetRolewiseFormListById(RoleId);
        }
       public async Task<ApiResponseModel> UpdateMultipleRolewiseFormPermission(List<RolewiseFormPermissionModel> UpdatedRolewiseFormPermissions)
        {
            return await FormPermissionMaster.UpdateMultipleRolewiseFormPermission(UpdatedRolewiseFormPermissions);
        }
    }
}
