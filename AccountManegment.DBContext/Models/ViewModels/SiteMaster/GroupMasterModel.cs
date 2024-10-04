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
        public Guid GroupId { get; set; }
        public List<SiteNameList>? SiteList { get; set; }
    }
    public class SiteNameList
    {
        public Guid SiteId { get; set; }
        public string? SiteName { get; set; }
    }
    public class SiteGroupModel
    {
        public int Id { get; set; }
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = null!;
        public string? GroupAddress { get; set; }
        public Guid SiteId { get; set; }
        public string? SiteName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<GroupAddressList>? GroupAddressList { get; set; }
        public List<SiteNameList>? SiteIdList { get; set; }
    }
    public class GroupAddressList
    {
        public string? GroupAddress { get; set; }
    }
}
