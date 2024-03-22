using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class User
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNo { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }
    public Guid? SiteId { get; set; }

    public Guid? SiteId { get; set; }

    public bool IsActive { get; set; }
    public bool? IsDeleted { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }
}
