using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.FormPermissionMaster
{
    public class UserwiseFormPermissionModel
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public int FormId { get; set; }

        public bool IsAddAllow { get; set; }

        public bool IsViewAllow { get; set; }

        public bool IsEditAllow { get; set; }

        public bool IsDeleteAllow { get; set; }

        public bool? IsApproved { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public string? FullName { get; set; }
        public string? FormName { get; set; }

    }
}
