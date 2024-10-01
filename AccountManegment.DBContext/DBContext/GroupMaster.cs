﻿using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class GroupMaster
{
    public int Id { get; set; }

    public Guid GroupId { get; set; }

    public string GroupName { get; set; } = null!;

    public Guid SiteId { get; set; }

    public string? GroupAddress { get; set; }

    public DateTime? CreatedOn { get; set; }

    public virtual Site Site { get; set; } = null!;
}
