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

        public string ProfileImage { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string FullName { get; set; }
        public int Role { get; set; }
    }
    public class LoginResponseModel
    {
        public string Message { get; set; }

        public int Code { get; set; }

        public LoginView Data { get; set; }

    }


}
