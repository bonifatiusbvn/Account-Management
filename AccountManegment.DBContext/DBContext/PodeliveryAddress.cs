﻿using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class PodeliveryAddress
{
    public int Aid { get; set; }

    public Guid Poid { get; set; }

    public string Address { get; set; } = null!;
}