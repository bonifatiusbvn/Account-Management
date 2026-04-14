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


        public Task<IEnumerable<UserwiseFormPermissionModel>> GetUserwiseFormPermissionList()
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserwiseFormPermissionModel>> GetUserwiseFormPermissionById(Guid UserId)
        {
            return await FormPermissionMaster.GetUserwiseFormPermissionById(UserId);
        }

        public async Task<ApiResponseModel> UpdateMultipleUserwiseFormPermission(List<UserwiseFormPermissionModel> UpdatedUserwiseFormPermissions)
        {
            return await FormPermissionMaster.UpdateMultipleUserwiseFormPermission(UpdatedUserwiseFormPermissions);
        }
    }
}
