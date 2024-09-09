using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.UserModels
{
    public class UserRoleModel
    {
        public int RoleId { get; set; }

        public string? Role { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsDelete { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
