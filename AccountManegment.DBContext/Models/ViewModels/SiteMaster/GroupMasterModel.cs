using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.DBContext.Models.ViewModels.SiteMaster
{
    public class GroupMasterModel
    {
        public int Id { get; set; }

        public string GroupName { get; set; } = null!;

        public List<SiteNameList>? SiteList { get; set; }
    }
    public class SiteNameList
    {
        public Guid SiteId { get; set; }
    }
}
