using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.FormPermissionMaster;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Services.FormPermissionMasterService
{
    public interface IFormPermissionMasterService
    {

        Task<IEnumerable<UserwiseFormPermissionModel>> GetUserwiseFormPermissionList();
        Task<List<UserwiseFormPermissionModel>> GetUserwiseFormPermissionById(Guid UserId);
        Task<ApiResponseModel> UpdateMultipleUserwiseFormPermission(List<UserwiseFormPermissionModel> UpdatedUserwiseFormPermissions);
    }
}
