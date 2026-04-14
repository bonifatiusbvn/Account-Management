using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.SiteMaster
{
    public class UserSiteListModel
    {
        public Guid SiteId { get; set; }
        public string SiteName { get; set; } = null!;
    }

    public class UserCompanyListModel
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
    }
}
