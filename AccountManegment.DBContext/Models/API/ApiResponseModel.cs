using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.API
{
    public class ApiResponseModel
    {
        public int code { get; set; }
        public dynamic data { get; set; }
        public string message { get; set; }
    }
}
