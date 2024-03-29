﻿using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class UserRole
{
    public int RoleId { get; set; }

    public string Role { get; set; } = null!;

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }
}
