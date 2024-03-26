using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.UserModels
{
    public class LoginView
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }

        public string PhoneNo { get; set; }
        public int RoleId { get; set; }

        public bool IsActive { get; set; }
        public string RoleName { get; set; }

        public DateTime CreatedOn { get; set; }
        public string? SiteName { get; set; }
        public Guid? SiteId { get; set; }
        public bool? IsDeleted { get; set; }
    }
    public class LoginResponseModel
    {
        public string Message { get; set; }

        public int Code { get; set; }

        public LoginView Data { get; set; }

    }


}
