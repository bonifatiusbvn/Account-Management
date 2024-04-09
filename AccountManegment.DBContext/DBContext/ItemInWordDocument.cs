using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class ItemInWordDocument
{
    public int Id { get; set; }

    public Guid RefInWordId { get; set; }

    public string? DocumentName { get; set; }

    public virtual ItemInword RefInWord { get; set; } = null!;
}
