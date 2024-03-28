using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.FormMaster
{
    public class FormMasterModel
    {
        public int FormId { get; set; }

        public string? FormGroup { get; set; }

        public string FormName { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
