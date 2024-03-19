using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.DataTableParameters
{
    public class DataTableRequstModel
    {
        public string? draw { get; set; }
        public string? start { get; set; }
        public string? lenght { get; set; }
        public string? sortColumn { get; set; }
        public string? sortColumnDir { get; set; }
        public string? searchValue { get; set; }
        public int pageSize { get; set; }
        public int skip { get; set; }

    }

    public class jsonData
    {
        public string? draw { get; set; }
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
        public dynamic data { get; set; }


    }

}
