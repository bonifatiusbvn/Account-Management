using System;
using System.Collections.Generic;

namespace AccountManagement.API;

public partial class ItemInword
{
    public Guid InwordId { get; set; }

    public Guid SiteId { get; set; }

    public Guid ItemId { get; set; }

    public string Item { get; set; } = null!;

    public int UnitTypeId { get; set; }

    public decimal Quantity { get; set; }

    public string? DocumentName { get; set; }

    public DateTime? Date { get; set; }

    public string? VehicleNumber { get; set; }

    public string? ReceiverName { get; set; }

    public bool? IsApproved { get; set; }

    public bool? IsDeleted { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual ICollection<ItemInWordDocument> ItemInWordDocuments { get; set; } = new List<ItemInWordDocument>();

    public virtual ItemMaster ItemNavigation { get; set; } = null!;
}
