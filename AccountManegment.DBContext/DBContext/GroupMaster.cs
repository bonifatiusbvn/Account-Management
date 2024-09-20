using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class GroupMaster
{
    public int Id { get; set; }

    public string GroupName { get; set; } = null!;

    public Guid SiteId { get; set; }
}
