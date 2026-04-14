using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.SiteMaster
{
    public class SiteAddressModel
    {
        public int Aid { get; set; }

        public Guid SiteId { get; set; }

        public string Address { get; set; } = null!;

        public bool? IsDeleted { get; set; }
        public int? StateCode { get; set; }
    }
}
