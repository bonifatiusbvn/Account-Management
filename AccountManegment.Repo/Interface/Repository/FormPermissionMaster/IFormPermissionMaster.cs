using AccountManagement.DBContext.Models.API;
using AccountManagement.DBContext.Models.ViewModels.FormPermissionMaster;
using AccountManagement.DBContext.Models.ViewModels.PurchaseOrder;
using AccountManagement.DBContext.Models.ViewModels.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Repository.Interface.Repository.FormPermissionMaster
{
    public interface IFormPermissionMaster
    {
        Task<ApiResponseModel> CreateRolewiseFormPermission(RolewiseFormPermissionModel formPermission);

        Task<IEnumerable<RolewiseFormPermissionModel>> GetRolewiseFormPermissionList();

        Task<RolewiseFormPermissionModel> GetRolewiseFormPermissionById(int formPermissionId);

        Task<ApiResponseModel> UpdateRolewiseFormPermission(RolewiseFormPermissionModel updateFormPermission);

        Task<ApiResponseModel> InsertMultipleRolewiseFormPermission(List<RolewiseFormPermissionModel> InsertRolewiseFormPermission);

        Task<List<RolewiseFormPermissionModel>> GetRolewiseFormListById(int RoleId);

        Task<ApiResponseModel> UpdateMultipleRolewiseFormPermission(List<RolewiseFormPermissionModel> UpdatedRolewiseFormPermissions);

    }
}
